using System.Linq.Expressions;

namespace WebApiApplication.Interfaces;

public interface IEntityService<TKey, T>
{
    public Task<T[]> ReadAsync(Expression<Func<T, bool>>? predicate = null, (string Property, bool Asc)? order = null, int? skip = null, int? take = null, CancellationToken cancellationToken = default);

    public Task<int> Count(Expression<Func<T, bool>>? predicate, CancellationToken cancellationToken = default);

    public Task<bool> Any(Expression<Func<T, bool>>? predicate, CancellationToken cancellationToken = default);

    public Task AddAsync(T value, CancellationToken cancellationToken = default);

    public Task DeleteAsync(TKey key, CancellationToken cancellationToken = default);

    public Task UpdateAsync(T value, CancellationToken cancellationToken = default);
}
