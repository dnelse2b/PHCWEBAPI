using Audit.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Audit.Application.Jobs;

/// <summary>
/// ✅ Hangfire Job para salvar audit logs de forma assíncrona com retry automático
/// </summary>
public class SaveAuditLogJob
{
    private readonly IAuditLogRepository _repository;
    private readonly ILogger<SaveAuditLogJob> _logger;

    public SaveAuditLogJob(
        IAuditLogRepository repository,
        ILogger<SaveAuditLogJob> logger)
    {
        _repository = repository;
        _logger = logger;
    }

  
    public async Task ExecuteAsync(
        string code,
        string? requestId,
        string responseDesc,
        string operation,
        string? requestBody = null,
        string? responseJson = null,
        string? ipAddress = null)
    {
        try
        {
   
            var auditLog = new Domain.Entities.AuditLog(
                code: code,
                requestId: requestId,
                responseDesc: responseDesc,
                operation: operation,
                content: requestBody,      
                responseText: responseJson,
                ip: ipAddress
            );

            await _repository.AddAsync(auditLog);

          
        }
        catch (Exception ex)
        {
            var errodto = new {
                code,
                requestId,
                responseDesc,
                operation,
                requestBody,
                responseJson,
                ipAddress,
                exception = ex.Message}
            ;
            _logger.LogError(ex, "Error saving audit log {@ErrorDetails}", errodto);
            Debug.Print($"ERROR ON SAVE LOG {new
            {
                code,
                requestId,
                responseDesc,
                operation,
                requestBody,
                responseJson,
                ipAddress,
                exception = ex.Message
            }}");
            // ✅ Re-throw para o Hangfire tentar novamente
            throw;
        }
    }
}
