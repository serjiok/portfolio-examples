using MyMediatr.Interfaces;

namespace ExampleMyMediatr.Commands;

internal class RequestCommand : IRequest
{
    public required int Value { get; init; }
}

internal sealed class RequestCommandHandler : IRequestHandler<RequestCommand>
{
    public Task Handle(RequestCommand request, CancellationToken cancellationToken = default)
    {
        Console.WriteLine(request.Value);
        return Task.CompletedTask;
    }
}