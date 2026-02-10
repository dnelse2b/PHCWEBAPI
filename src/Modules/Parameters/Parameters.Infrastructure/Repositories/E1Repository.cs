using Microsoft.EntityFrameworkCore;
using Parameters.Domain.Entities;
using Parameters.Domain.Repositories;
using Parameters.Infrastructure.Persistence;

namespace Parameters.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório E1
/// </summary>
public class E1Repository : IE1Repository
{
    private readonly ParametersDbContext _context;

    public E1Repository(ParametersDbContext context)
    {
        _context = context;
    }

    public async Task<E1?> GetByStampAsync(string e1Stamp, CancellationToken cancellationToken = default)
    {
        return await _context.E1
            .Include(e => e.E4)
            .FirstOrDefaultAsync(e => e.E1Stamp == e1Stamp, cancellationToken);
    }

    public async Task<E1?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.E1
            .Include(e => e.E4)
            .FirstOrDefaultAsync(e => e.Code == code, cancellationToken);
    }

    public async Task<IEnumerable<E1>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = _context.E1.Include(e => e.E4).AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(e => e.Active);
        }

        return await query.OrderBy(e => e.Code).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<E1>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.E1
            .Include(e => e.E4)
            .OrderBy(e => e.Code)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(string e1Stamp, CancellationToken cancellationToken = default)
    {
        return await _context.E1.AnyAsync(e => e.E1Stamp == e1Stamp, cancellationToken);
    }

    public async Task<E1> AddAsync(E1 e1, CancellationToken cancellationToken = default)
    {
        await _context.E1.AddAsync(e1, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return e1;
    }

    public async Task UpdateAsync(E1 e1, CancellationToken cancellationToken = default)
    {
        _context.E1.Update(e1);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(E1 e1, CancellationToken cancellationToken = default)
    {
        _context.E1.Remove(e1);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.E1.CountAsync(cancellationToken);
    }
}
