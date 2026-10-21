namespace MyMediatr.Interfaces;

public interface INotification { }

public interface INotificationHandler<T> where T : INotification
{
    Task Handle(T request, CancellationToken cancellationToken = default);
}
