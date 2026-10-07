namespace PatternsExample.Domain;

internal interface IMessageWriter<T> where T : notnull
{
    Task Write(T message, CancellationToken cancellationToken = default);
}
