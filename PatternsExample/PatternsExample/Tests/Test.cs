using Microsoft.Extensions.DependencyInjection;
using PatternsExample.Application;

namespace PatternsExample.Tests;

internal class Test
{
    public static async Task Run(IServiceProvider serviceProvider)
    {
        var testWriter = serviceProvider.GetRequiredService<TestWriter>();
        
        var message = $"test - {Random.Shared.Next()}";

        await testWriter.TestWriteConsoleText(message);
        await testWriter.TestWriteDBText(message);

        var testReader = serviceProvider.GetRequiredService<TestReader>();
        await testReader.TestDBReaderAsync(WriterStrategies.WriteConsole);
        await testReader.TestDBReaderAsync(WriterStrategies.WriteFile);
    }
}
