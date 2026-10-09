using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebApiApplication.Interfaces;
using WebApiDatabase.Interfaces;
using WebApiDatabase.Models;

namespace WebApiApplication.Services;

public sealed class UserService(IWrite<int, User> writeDB, IRead<int, User> readDB) : IEntityService<int, User>
{
    public Task<User[]> ReadAsync(Expression<Func<User, bool>>? predicate = null, (string Property, bool Asc)? order = null, int? skip = null, int? take = null, CancellationToken cancellationToken = default)
        => readDB.Read(predicate, order, skip, take).ToArrayAsync(cancellationToken);

    public Task<int> CountAsync(Expression<Func<User, bool>>? predicate, CancellationToken cancellationToken = default)
        => readDB.Read(predicate).CountAsync(cancellationToken);

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
}
