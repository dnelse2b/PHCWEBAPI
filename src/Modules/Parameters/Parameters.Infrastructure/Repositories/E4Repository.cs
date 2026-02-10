using Microsoft.EntityFrameworkCore;
using Parameters.Domain.Entities;
using Parameters.Domain.Repositories;
using Parameters.Infrastructure.Persistence;

namespace Parameters.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório E4
/// </summary>
public class E4Repository : IE4Repository
{
    private readonly ParametersDbContext _context;

    public E4Repository(ParametersDbContext context)
    {
        _context = context;
    }

    public async Task<E4?> GetByStampAsync(string e4Stamp, CancellationToken cancellationToken = default)
    {
        return await _context.E4
            .FirstOrDefaultAsync(e => e.E4Stamp == e4Stamp, cancellationToken);
    }

    public async Task<IEnumerable<E4>> GetAllByE1StampAsync(string e1Stamp, CancellationToken cancellationToken = default)
    {
        return await _context.E4
            .Where(e => e.E4Stamp == e1Stamp)
            .OrderBy(e => e.Sequence)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<E4>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.E4
            .OrderBy(e => e.Sequence)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(string e4Stamp, CancellationToken cancellationToken = default)
    {
        return await _context.E4.AnyAsync(e => e.E4Stamp == e4Stamp, cancellationToken);
    }

    public async Task<E4> AddAsync(E4 e4, CancellationToken cancellationToken = default)
    {
        await _context.E4.AddAsync(e4, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return e4;
    }

    public async Task UpdateAsync(E4 e4, CancellationToken cancellationToken = default)
    {
        _context.E4.Update(e4);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(E4 e4, CancellationToken cancellationToken = default)
    {
        _context.E4.Remove(e4);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
