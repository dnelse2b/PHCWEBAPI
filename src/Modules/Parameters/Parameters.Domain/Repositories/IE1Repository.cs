using Parameters.Domain.Entities;

namespace Parameters.Domain.Repositories;

/// <summary>
/// Repository interface para a entidade Parametro (E1)
/// </summary>
public interface IE1Repository
{
    Task<E1?> GetByStampAsync(string e1Stamp, CancellationToken cancellationToken = default);
    Task<E1?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<E1>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<E1>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string e1Stamp, CancellationToken cancellationToken = default);
    Task<E1> AddAsync(E1 e1, CancellationToken cancellationToken = default);
    Task UpdateAsync(E1 e1, CancellationToken cancellationToken = default);
    Task DeleteAsync(E1 e1, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}
