using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebApiDatabase.Interfaces;
using WebApiDatabase.Models;

namespace WebApiDatabase.Repositories;

public sealed class UserRepository(ContextDB context) : IRead<int, User>, IWrite<int, User>
{
    public IQueryable<User> Read(Expression<Func<User, bool>>? predicate = null, (string Property, bool Asc)? order = null, int? skip = null, int? take = null)
    {
        var query = 
            predicate == null ?
        context.Users.AsNoTracking() : context.Users.AsNoTracking().Where(predicate);

        if (order.HasValue)
        {
            query = order.Value.Property switch
            {
                nameof(User.Login) => order.Value.Asc? query.OrderBy(x=>x.Login): query.OrderByDescending(x=>x.Login),
                _ => query
            };
        }

        if (skip.HasValue) query = query.Skip(skip.Value);
        if (take.HasValue) query = query.Take(take.Value);

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
}
