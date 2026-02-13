using Shared.Kernel.Responses;
using Microsoft.Extensions.Logging;
using Hangfire;
using Audit.Application.Jobs;

namespace Audit.Application.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly ILogger<AuditLogService> _logger;

    public AuditLogService(
        IBackgroundJobClient backgroundJobClient,
        ILogger<AuditLogService> logger)
    {
        _backgroundJobClient = backgroundJobClient;
        _logger = logger;
    }

    public void LogResponseAsync(
        ResponseDTO response,
        string? requestId,
        string operation,
        string? ipAddress,
        string? userAgent,
        int statusCode)
    {
        try
        {
            var code = response.Response.Code;
            var responseDesc = response.Response.Description;
            var content = response.Content?.ToString();
            var responseText = response.Data?.ToString();

            var jobId = _backgroundJobClient.Enqueue<SaveAuditLogJob>(job =>
                job.ExecuteAsync(
                    code,
                    requestId,
                    responseDesc,
                    operation,
                    content,
                    responseText,
                    ipAddress
                ));

           
        }
        catch (Exception ex)
        {
            // ✅ Log do erro mas NÃO propaga (não deve quebrar o request)
            _logger.LogError(ex,
                "[AUDIT] ❌ Failed to enqueue job: RequestId={RequestId}, Operation={Operation}",
                requestId, operation);
        }
    }
}
