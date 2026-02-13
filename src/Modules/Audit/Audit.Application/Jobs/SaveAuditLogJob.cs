using Audit.Domain.Repositories;
using Microsoft.Extensions.Logging;

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

    /// <summary>
    /// Executa o job de salvar audit log
    /// ⚠️ Todos os parâmetros devem ser serializáveis (primitivos ou DTOs simples)
    /// </summary>
    public async Task ExecuteAsync(
        string code,
        string? requestId,
        string responseDesc,
        string operation,
        string? content = null,
        string? responseText = null,
        string? ipAddress = null)
    {
        try
        {
          

            var auditLog = new Domain.Entities.AuditLog(
                code: code,
                requestId: requestId,
                responseDesc: responseDesc,
                operation: operation,
                content: content,
                responseText: responseText,
                ip: ipAddress
            );

            await _repository.AddAsync(auditLog);

       
        }
        catch (Exception ex)
        {
          

            // ✅ Re-throw para o Hangfire tentar novamente
            throw;
        }
    }
}
