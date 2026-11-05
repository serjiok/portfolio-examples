using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
using WebApiDatabase.Interfaces;
using WebApiDatabase.Models;

namespace WebApiDatabase.Repositories;

public sealed class UserRepository(ContextDB context) : IRead<int, User>, IWrite<int, User>
{
    public IQueryable<User> Read(Expression<Func<User, bool>>? predicate = null)
    {
        var query = 
            predicate == null ?
        context.Users.AsNoTracking() : context.Users.AsNoTracking().Where(predicate);
        return query;
    }

    public Task AddAsync(User value, CancellationToken cancellationToken = default) => context.AddAsync(value, cancellationToken).AsTask();

    public async Task DeleteAsync(int key, CancellationToken cancellationToken = default)
    {
        var item = await Read(x => x.Id == key).SingleOrDefaultAsync(cancellationToken);
        if (item == null) return;

        context.Users.Remove(item);
    }

    public Task UpdateAsync(User value, CancellationToken cancellationToken = default)
    {
        context.Users.Update(value);
        return Task.CompletedTask;
    }

    public Task SaveAsync(CancellationToken cancellationToken = default) => context.SaveChangesAsync(cancellationToken);

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken) => context.Database.BeginTransactionAsync(cancellationToken);
}
