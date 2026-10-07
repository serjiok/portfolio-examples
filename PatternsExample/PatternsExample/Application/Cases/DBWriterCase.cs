using PatternsExample.Application.Converters;
using PatternsExample.Domain;
using PatternsExample.Domain.Models;
using PatternsExample.Infrastructure;
using PatternsExample.Infrastructure.Interfaces;

namespace PatternsExample.Application.Cases;

internal sealed class DBWriterCase(IWriteDB<int, MessageDB> repository) : IMessageWriter<Message<string>>
{
    public async Task Write(Message<string> message, CancellationToken cancellationToken = default)
    {
        var messageDB = message.ToDB();
        await repository.AddAsync(messageDB, cancellationToken);
        await repository.SaveAsync(cancellationToken);
    }
}
