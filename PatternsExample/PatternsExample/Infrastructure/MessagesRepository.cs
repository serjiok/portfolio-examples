using Microsoft.EntityFrameworkCore;
using PatternsExample.Infrastructure.Interfaces;
using System.Linq.Expressions;

namespace PatternsExample.Infrastructure;

internal sealed class MessagesRepository(ContextDB context) : IReadDB<int, MessageDB>, IWriteDB<int, MessageDB>
{
    public IQueryable<MessageDB> Read(Expression<Func<MessageDB, bool>>? predicate = null)
    {
        var query = context.Messages.AsNoTracking();
        return predicate == null ? query : query.Where(predicate);
    }

    public Task AddAsync(MessageDB value, CancellationToken cancellationToken = default)
        => context.Messages.AddAsync(value, cancellationToken).AsTask();

    public async Task DeleteAsync(int key, CancellationToken cancellationToken = default)
    {
        var item = await Read(x => x.Id == key).SingleOrDefaultAsync(cancellationToken);
        if (item == null) return;
        
        context.Messages.Remove(item);
    }

    public Task UpdateAsync(MessageDB value, CancellationToken cancellationToken = default)
    {
        context.Messages.Update(value);
        return Task.CompletedTask;
    }

    public Task SaveAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}
