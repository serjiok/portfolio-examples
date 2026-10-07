using PatternsExample.Application.Converters;
using PatternsExample.Infrastructure;
using System.Text;

namespace PatternsExample.Application;

/// <summary>
/// For pattern strategy
/// </summary>
internal static class WriterStrategies
{
    public static void WriteConsole(MessageDB messageDB)
    {
        Console.WriteLine($"Message object: {messageDB.ToMessage().Value}");
        Console.WriteLine($"DB object     : {messageDB.Id} {messageDB.TextProperty}");
    }

    public static void WriteFile(MessageDB messageDB)
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.log");
        using (StreamWriter sw = new StreamWriter(filePath, true, Encoding.UTF8))
        {
            sw.WriteLine($"{DateTime.Now} Message object: {messageDB.ToMessage().TextProperty}");
            sw.WriteLine($"{DateTime.Now} DB object     : {messageDB.Id} {messageDB.Text}");
        }
    }
}
