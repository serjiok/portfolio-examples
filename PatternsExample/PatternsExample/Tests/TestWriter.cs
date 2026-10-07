using PatternsExample.Application.Factories;
using PatternsExample.Domain.Models;

namespace PatternsExample.Tests;

internal sealed class TestWriter(Fabric fabric)
{
    public async Task TestWriteConsoleText(string text)
    {
        var message = new Message<string>() { Value = text };

        var console = fabric.GetConsoleWritter();
        await console.Write(message);
    }

    public async Task TestWriteDBText(string text)
    {
        var message = new Message<string>() { Value = text };

        var db = fabric.GetBDWritter();
        await db.Write(message);
        Console.WriteLine("Write in db ok!");
    }
}
