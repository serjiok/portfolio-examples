namespace MyMediatr.Interfaces;

public interface IMediator
{
    Task Publish(INotification request, CancellationToken cancellationToken = default);
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) where TResponse: notnull;
    Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : notnull, IRequest;
}
