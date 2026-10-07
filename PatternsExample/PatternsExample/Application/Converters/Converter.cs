using PatternsExample.Domain.Models;
using PatternsExample.Infrastructure;

namespace PatternsExample.Application.Converters;

internal static class Converter
{
    extension(Message<string> message)
    {
        public string TextProperty => message.Value;
        public MessageDB ToDB() => new MessageDB() { Text = message.Value };
    }

    extension(MessageDB message)
    {
        public string TextProperty => message.Text;
        public Message<string> ToMessage() => new Message<string>() { Value = message.Text };
    }
}
