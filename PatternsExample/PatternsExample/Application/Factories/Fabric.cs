using PatternsExample.Application.Cases;
using PatternsExample.Domain;
using PatternsExample.Domain.Models;
using PatternsExample.Infrastructure;
using PatternsExample.Infrastructure.Interfaces;

namespace PatternsExample.Application.Factories;

/// <summary>
/// Fabric pattern
/// </summary>
/// <param name="writeDB"></param>
internal sealed class Fabric(IWriteDB<int, MessageDB> writeDB)
{
    public IMessageWriter<Message<string>> GetConsoleWritter()
    {
        ConsoleWriterCase consoleWriterCase = new ConsoleWriterCase();
        return consoleWriterCase;
    }

    public IMessageWriter<Message<string>> GetBDWritter()
    {
        DBWriterCase dBWriterCase = new DBWriterCase(writeDB);
        return dBWriterCase;
    }
}
