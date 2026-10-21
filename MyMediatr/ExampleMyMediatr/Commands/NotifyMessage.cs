using MyMediatr.Interfaces;

namespace ExampleMyMediatr.Commands;

internal class NotifyMessage : INotification
{
    public required string Message { get; init; }
}

internal class NotifyMessage1Handler : INotificationHandler<NotifyMessage>
{
    public Task Handle(NotifyMessage request, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"1 {request.Message}");
        return Task.CompletedTask;
    }
}

internal class NotifyMessage2Handler : INotificationHandler<NotifyMessage>
{
    public Task Handle(NotifyMessage request, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"2 {request.Message}");
        return Task.CompletedTask;
    }
}