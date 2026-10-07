namespace PatternsExample.Infrastructure.Interfaces;

internal interface IWriteDB<TKey, T>
{
    /// <summary>
    /// Add data
    /// </summary>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddAsync(T value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Udpate data
    /// </summary>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateAsync(T value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete data
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteAsync(TKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Save changes
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SaveAsync(CancellationToken cancellationToken = default);
}
