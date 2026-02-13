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
        int statusCode,
        string? requestBody = null,
        string? responseJson = null)
    {
        try
        {
            // ✅ Verificação defensiva
            if (response?.Response == null)
            {
             
                return;
            }

            var code = response.Response.Code;
            var responseDesc = response.Response.Description;
            
            var jobId = _backgroundJobClient.Enqueue<SaveAuditLogJob>(job =>
                job.ExecuteAsync(
                    code,
                    requestId,
                    responseDesc,
                    operation,
                    requestBody,
                    responseJson,
                    ipAddress
                ));

         
        }
        catch (Exception ex)
        {
       
        }
    }
}
