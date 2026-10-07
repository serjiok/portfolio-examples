using System.Linq.Expressions;

namespace PatternsExample.Infrastructure.Interfaces;

internal interface IReadDB<TKey, T>
{
    /// <summary>
    /// Read data
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IQueryable<T> Read(Expression<Func<T, bool>>? predicate = null);
}
