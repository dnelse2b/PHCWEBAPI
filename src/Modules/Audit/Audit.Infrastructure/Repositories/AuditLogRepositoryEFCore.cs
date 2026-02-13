using Microsoft.EntityFrameworkCore;
using Audit.Domain.Entities;
using Audit.Domain.Repositories;
using Audit.Infrastructure.Persistence;

namespace Audit.Infrastructure.Repositories;


public class AuditLogRepositoryEFCore : IAuditLogRepository
{
    private readonly AuditDbContextEFCore _context;

    public AuditLogRepositoryEFCore(AuditDbContextEFCore context)
    {
        _context = context;
    }

    public void Add(AuditLog auditLog)
    {
        _context.AuditLogs.Add(auditLog);
        _context.SaveChanges();
    }

    public async Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        await _context.AuditLogs.AddAsync(auditLog, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuditLog>> GetByRequestIdAsync(string requestId, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .Where(a => a.RequestId == requestId)
            .OrderByDescending(a => a.Data)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuditLog>> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .Where(a => a.Code == code)
            .OrderByDescending(a => a.Data)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .Where(a => a.Data >= startDate && a.Data <= endDate)
            .OrderByDescending(a => a.Data)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuditLog>> GetByOperationAsync(string operation, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .Where(a => a.Operation == operation)
            .OrderByDescending(a => a.Data)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuditLog>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs.Take(10)
            .OrderByDescending(a => a.Data)
            .ToListAsync(cancellationToken);
    }

    public async Task<AuditLog?> GetByStampAsync(string stamp, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .FirstOrDefaultAsync(a => a.ULogsstamp == stamp, cancellationToken);
    }

    public async Task<IEnumerable<AuditLog>> GetByCorrelationIdAsync(string correlationId, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .Where(a => a.RequestId == correlationId)
            .OrderByDescending(a => a.Data)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// ✅ NOVO: Busca paginada com filtros aplicados NO BANCO
    /// </summary>
    public async Task<(IEnumerable<AuditLog> logs, int totalCount)> GetPagedAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? correlationId = null,
        string? operation = null,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        // ✅ Query base com AsNoTracking para performance
        var query = _context.AuditLogs.AsNoTracking();

        // ✅ Aplicar filtros NO BANCO (não em memória)
        if (startDate.HasValue)
        {
            query = query.Where(a => a.Data >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(a => a.Data <= endDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(correlationId))
        {
            query = query.Where(a => a.RequestId == correlationId);
        }

        if (!string.IsNullOrWhiteSpace(operation))
        {
            query = query.Where(a => a.Operation != null && a.Operation.Contains(operation));
        }

        // ✅ Contar total ANTES da paginação (query separada otimizada)
        var totalCount = await query.CountAsync(cancellationToken);

        // ✅ Aplicar ordenação e paginação
        var logs = await query
            .OrderByDescending(a => a.Data)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (logs, totalCount);
    }
}
