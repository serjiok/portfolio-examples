using PatternsExample.Domain;
using PatternsExample.Domain.Models;

namespace PatternsExample.Application.Cases;

internal sealed class ConsoleWriterCase : IMessageWriter<Message<string>>
{
    public Task Write(Message<string> message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine(message.Value);
        return Task.CompletedTask;
    }
}
