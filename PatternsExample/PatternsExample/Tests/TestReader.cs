using PatternsExample.Application;
using PatternsExample.Infrastructure;
using PatternsExample.Infrastructure.Interfaces;
using System.Text;

namespace PatternsExample.Tests;

internal sealed class TestReader(IReadDB<int, MessageDB>? service)
{
    public async Task TestDBReaderAsync(Action<MessageDB> strategyOutput)
    {
        if (service == null) return;

        foreach (var messageDB in service.Read())
        {
            strategyOutput.Invoke(messageDB);
        }
    }
}
