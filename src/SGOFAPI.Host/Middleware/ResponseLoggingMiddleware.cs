using Audit.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Shared.Kernel.Responses;
using System.Text;
using System.Text.Json;

namespace PHCAPI.Host.Middleware;


public sealed class ResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ResponseLoggingMiddleware> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ResponseLoggingMiddleware(
        RequestDelegate next,
        ILogger<ResponseLoggingMiddleware> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _next = next;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path;
        var method = context.Request.Method;
        
        _logger.LogInformation("[MIDDLEWARE START] {Method} {Path}", method, path);
        
        var requestBody = await CaptureRequestBodyAsync(context.Request);
        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);
            await ProcessSuccessfulResponseAsync(context, requestBody, responseBody, originalBodyStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[MIDDLEWARE EXCEPTION] {Method} {Path} - Exception: {Message}", 
                method, path, ex.Message);
            await ProcessExceptionResponseAsync(context, requestBody, ex);
            context.Response.Body = originalBodyStream;
            throw;
        }
        finally
        {
            context.Response.Body = originalBodyStream;
            _logger.LogInformation("[MIDDLEWARE END] {Method} {Path} - Status: {StatusCode}", 
                method, path, context.Response.StatusCode);
        }
    }

    private async Task ProcessSuccessfulResponseAsync(
        HttpContext context,
        string? requestBody,
        MemoryStream responseBody,
        Stream originalBodyStream)
    {
        var responseText = await ReadResponseTextAsync(responseBody);
        await responseBody.CopyToAsync(originalBodyStream);

        if (string.IsNullOrWhiteSpace(responseText) || !IsJsonResponse(context)) return;

        var (responseDto, auditJson) = GetResponseForAudit(responseText, context);
        await LogAuditAsync(context, requestBody, responseDto, auditJson);
    }

    private (ResponseDTO response, string json) GetResponseForAudit(string responseText, HttpContext context)
    {
        // Tentativa 1: Deserializar como ResponseDTO
        var responseDto = TryDeserializeResponse(responseText);
        if (responseDto != null)
            return (responseDto, responseText);

        // Tentativa 2: Deserializar como objeto genérico
        var genericObject = TryDeserializeAsObject(responseText);
        if (genericObject != null)
        {
            var wrappedResponse = CreateGenericResponseWrapper(genericObject, context);
            return (wrappedResponse, responseText); // ✅ Preserva JSON original
        }

        // Fallback: Usar resposta raw como string
        var rawResponse = CreateRawResponseWrapper(responseText, context);
        return (rawResponse, responseText); // ✅ Preserva JSON original
    }

    private object? TryDeserializeAsObject(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<object>(json);
        }
        catch (JsonException ex)
        {
            _logger.LogDebug("[MIDDLEWARE] Failed to deserialize as generic object: {Message}", ex.Message);
            return null;
        }
    }

    private static ResponseDTO CreateGenericResponseWrapper(object data, HttpContext context)
    {
        var correlationId = ExtractCorrelationId(context.TraceIdentifier);
        
        return new ResponseDTO(
            response: new ResponseCodeDTO(
                code: "0000",
                description: "Success - Non-standard response structure",
                id: correlationId
            ),
            data: data,
            content: new
            {
                path = context.Request.Path.ToString(),
                originalStructure = "generic-object"
            }
        );
    }

    private static ResponseDTO CreateRawResponseWrapper(string rawJson, HttpContext context)
    {
        var correlationId = ExtractCorrelationId(context.TraceIdentifier);
        
        return new ResponseDTO(
            response: new ResponseCodeDTO(
                code: "0000",
                description: "Success - Raw JSON captured",
                id: correlationId
            ),
            data: new { rawResponse = rawJson },
            content: new
            {
                path = context.Request.Path.ToString(),
                originalStructure = "raw-json"
            }
        );
    }

    private async Task ProcessExceptionResponseAsync(
        HttpContext context,
        string? requestBody,
        Exception ex)
    {
        var internalAuditResponse = CreateExceptionAuditResponse(context, ex);
        var auditJson = SerializeResponse(internalAuditResponse);

        await LogAuditAsync(context, requestBody, internalAuditResponse, auditJson);
    }

    private async Task LogAuditAsync(
        HttpContext context,
        string? requestBody,
        ResponseDTO responseDto,
        string responseJson)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var auditLogService = scope.ServiceProvider.GetService<IAuditLogService>();

        if (auditLogService == null)
        {
            return;
        }

        auditLogService.LogResponseAsync(
            responseDto,
            context.TraceIdentifier,
            $"{context.Request.Method} {context.Request.Path}",
            context.Connection.RemoteIpAddress?.ToString(),
            context.Request.Headers.UserAgent.ToString(),
            context.Response.StatusCode,
            requestBody,
            responseJson
        );

    }

    private static async Task<string> ReadResponseTextAsync(MemoryStream responseBody)
    {
        responseBody.Seek(0, SeekOrigin.Begin);
        var text = await new StreamReader(responseBody).ReadToEndAsync();
        responseBody.Seek(0, SeekOrigin.Begin);
        return text;
    }

    private static bool IsJsonResponse(HttpContext context) =>
        context.Response.ContentType?.Contains("application/json", StringComparison.OrdinalIgnoreCase) ?? false;

    private ResponseDTO? TryDeserializeResponse(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<ResponseDTO>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (JsonException ex)
        {
           
            return null;
        }
    }

    private static ResponseDTO CreateExceptionAuditResponse(HttpContext context, Exception ex)
    {
        var requestId = context.TraceIdentifier;
        var correlationId = ExtractCorrelationId(requestId);

        return new ResponseDTO(
            response: new ResponseCodeDTO(
                code: "0007",
                description: "Internal error - Exception caught in middleware",
                id: correlationId
            ),
            data: new
            {
                exceptionType = ex.GetType().FullName,
                message = ex.Message,
                source = ex.Source,
                stackTrace = ex.StackTrace,
                innerException = ex.InnerException != null ? new
                {
                    type = ex.InnerException.GetType().FullName,
                    message = ex.InnerException.Message,
                    stackTrace = ex.InnerException.StackTrace
                } : null,
                targetSite = ex.TargetSite?.ToString()
            },
            content: new
            {
                path = context.Request.Path.ToString(),
                method = context.Request.Method,
                queryString = context.Request.QueryString.ToString(),
                timestamp = DateTime.UtcNow
            }
        );
    }

    private static string SerializeResponse(ResponseDTO response) =>
        JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

    private static decimal ExtractCorrelationId(string requestId)
    {
        var numericOnly = new string(requestId.Where(char.IsDigit).ToArray());
        return decimal.TryParse(numericOnly.Length > 15 ? numericOnly[..15] : numericOnly, out var id) ? id : 0;
    }

    private async Task<string?> CaptureRequestBodyAsync(HttpRequest request)
    {
        if (!ShouldCaptureRequestBody(request)) return null;

        try
        {
            request.EnableBuffering();
            request.Body.Position = 0;

            using var reader = new StreamReader(
                request.Body,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024,
                leaveOpen: true);

            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;

            return string.IsNullOrWhiteSpace(body) ? null : body;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    private static bool ShouldCaptureRequestBody(HttpRequest request) =>
        (HttpMethods.IsPost(request.Method) || 
         HttpMethods.IsPut(request.Method) || 
         HttpMethods.IsPatch(request.Method)) &&
        request.ContentLength is > 0;
}
