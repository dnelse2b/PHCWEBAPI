using Audit.Domain.Entities;

namespace Audit.Domain.Repositories;

/// <summary>
/// Interface do repositório de logs de auditoria
/// </summary>
public interface IAuditLogRepository
{
    /// <summary>
    /// Adiciona um novo log de auditoria (síncrono)
    /// </summary>
    void Add(AuditLog auditLog);

    /// <summary>
    /// Adiciona um novo log de auditoria (assíncrono)
    /// </summary>
    Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca logs por request ID (para correlação)
    /// </summary>
    Task<IEnumerable<AuditLog>> GetByRequestIdAsync(string requestId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca logs por código de resposta
    /// </summary>
    Task<IEnumerable<AuditLog>> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca logs por período
    /// </summary>
    Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca logs por operação
    /// </summary>
    Task<IEnumerable<AuditLog>> GetByOperationAsync(string operation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca todos os logs de auditoria
    /// </summary>
    Task<IEnumerable<AuditLog>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca log por stamp (identificador único)
    /// </summary>
    Task<AuditLog?> GetByStampAsync(string stamp, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca logs por Correlation ID
    /// </summary>
    Task<IEnumerable<AuditLog>> GetByCorrelationIdAsync(string correlationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// ✅ Busca paginada com filtros aplicados NO BANCO (otimizada)
    /// Retorna tupla com (logs, totalCount)
    /// </summary>
    /// <param name="startDate">Data inicial (opcional)</param>
    /// <param name="endDate">Data final (opcional)</param>
    /// <param name="correlationId">Correlation ID (opcional)</param>
    /// <param name="operation">Nome da operação (opcional)</param>
    /// <param name="pageNumber">Número da página (1-based)</param>
    /// <param name="pageSize">Tamanho da página</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Tupla com lista de logs e total de registros</returns>
    Task<(IEnumerable<AuditLog> logs, int totalCount)> GetPagedAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? correlationId = null,
        string? operation = null,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);
}
