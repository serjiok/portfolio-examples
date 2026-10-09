using System.Linq.Expressions;

namespace WebApiDatabase.Interfaces;

public interface IRead<TKey, T>
{
    /// <summary>
    /// Read data
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IQueryable<T> Read(Expression<Func<T, bool>>? predicate = null, (string Property, bool Asc)? order = null, int? skip = null, int? take = null);
}
