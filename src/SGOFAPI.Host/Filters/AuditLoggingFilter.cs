using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Audit.Application.Services;
using Shared.Kernel.Responses;

namespace PHCAPI.Host.Filters;

/// <summary>
/// ✅ Action Filter para log de auditoria nas respostas da API
/// Registrado globalmente em Program.cs
/// </summary>
public sealed class AuditLoggingFilter : IAsyncActionFilter
{
    private readonly IAuditLogService _auditLogService;
    private readonly ILogger<AuditLoggingFilter> _logger;

    public AuditLoggingFilter(
        IAuditLogService auditLogService,
        ILogger<AuditLoggingFilter> logger)
    {
        _auditLogService = auditLogService ?? throw new ArgumentNullException(nameof(auditLogService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        try
        {

            // ✅ Executar a action
            var executedContext = await next();


            // ✅ Apenas logar se a resposta for um ResponseDTO
            if (executedContext.Result is not ObjectResult objectResult)
            {
                return;
            }

            if (objectResult.Value is not ResponseDTO response)
            {
                return;
            }


            var httpContext = context.HttpContext;
            var request = httpContext.Request;

            var requestId = httpContext.TraceIdentifier;
            var operation = $"{request.Method} {request.Path}";
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = request.Headers.UserAgent.ToString();
            var statusCode = objectResult.StatusCode ?? httpContext.Response.StatusCode;

            // ✅ Chamar serviço (fire-and-forget seguro com scope próprio)
            _auditLogService.LogResponseAsync(
                response,
                requestId,
                operation,
                ipAddress,
                userAgent,
                statusCode
            );

        }
        catch (Exception ex)
        {
            // ✅ NUNCA deixar o filtro quebrar o pipeline
            _logger.LogError(ex, 
                "[FILTER] ❌ CRITICAL: Filter failed for {Path}. Error: {ErrorMessage}", 
                context.HttpContext.Request.Path, 
                ex.Message);
        }
    }
}

