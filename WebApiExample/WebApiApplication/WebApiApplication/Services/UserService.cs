using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
using WebApiApplication.Interfaces;
using WebApiDatabase.Interfaces;
using WebApiDatabase.Models;

namespace WebApiApplication.Services;

public sealed class UserService(IWrite<int, User> writeDB, IRead<int, User> readDB) : IEntityService<int, User>
{
    public Task<User[]> ReadAsync(Expression<Func<User, bool>>? predicate = null, Models.Filter? filter = null, CancellationToken cancellationToken = default)
        => readDB.Read(predicate).ApplyFilter(filter).ToArrayAsync(cancellationToken);

    public Task<int> CountAsync(Expression<Func<User, bool>>? predicate, Models.Filter? filter = null, CancellationToken cancellationToken = default)
        => readDB.Read(predicate).ApplyFilter(filter).CountAsync(cancellationToken);

    public Task<bool> AnyAsync(Expression<Func<User, bool>>? predicate, CancellationToken cancellationToken = default)
        => readDB.Read(predicate).AnyAsync(cancellationToken);

    public async Task AddAsync(User value, CancellationToken cancellationToken = default)
    {
        await writeDB.AddAsync(value, cancellationToken);
        await writeDB.SaveAsync(cancellationToken);
    }

    public async Task DeleteAsync(int key, CancellationToken cancellationToken = default)
    {
        await writeDB.DeleteAsync(key, cancellationToken);
        await writeDB.SaveAsync(cancellationToken);
    }

    public async Task UpdateAsync(User value, CancellationToken cancellationToken = default)
    {
        await writeDB.UpdateAsync(value, cancellationToken);
        await writeDB.SaveAsync(cancellationToken);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        => writeDB.BeginTransactionAsync(cancellationToken);

    
}

internal static class QueryExt
{
    public static IQueryable<User> ApplyFilter(this IQueryable<User> query, Models.Filter? filter)
    {
        if (filter == null) return query;

        if (filter.Property != null)
        {
            switch (filter.Property)
            {
                case nameof(User.Id):
                    {
                        query = filter.Asc == true ?
                           query.OrderBy(x => x.Id) :
                           query.OrderByDescending(x => x.Id);
                    }
                    break;
                case nameof(User.Login):
                    {
                        query = filter.Asc == true ?
                           query.OrderBy(x => x.Login) :
                           query.OrderByDescending(x => x.Login);
                    }
                    break;
            }
        }

        if (filter.Skip.HasValue) query = query.Skip(filter.Skip.Value);
        if (filter.Take.HasValue) query = query.Take(filter.Take.Value);

        if (!string.IsNullOrEmpty(filter.Search))
        {
            var find = filter.Search.ToLower();
            query = query.Where(x => x.Login.ToLower().Contains(find));
        }

        return query;
    }
}
