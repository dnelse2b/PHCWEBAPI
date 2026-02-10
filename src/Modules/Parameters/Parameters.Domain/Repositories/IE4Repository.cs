using Parameters.Domain.Entities;

namespace Parameters.Domain.Repositories;

/// <summary>
/// Repository interface para a entidade E4
/// </summary>
public interface IE4Repository
{
    Task<E4?> GetByStampAsync(string e4Stamp, CancellationToken cancellationToken = default);
    Task<IEnumerable<E4>> GetAllByE1StampAsync(string e1Stamp, CancellationToken cancellationToken = default);
    Task<IEnumerable<E4>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string e4Stamp, CancellationToken cancellationToken = default);
    Task<E4> AddAsync(E4 e4, CancellationToken cancellationToken = default);
    Task UpdateAsync(E4 e4, CancellationToken cancellationToken = default);
    Task DeleteAsync(E4 e4, CancellationToken cancellationToken = default);
}
