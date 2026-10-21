using MyMediatr.Interfaces;

namespace ExampleMyMediatr.Commands;

internal class RequestResponseCommand : IRequest<string>
{
    public required string Value { get; init; }
}

internal sealed class RequestResponseCommandHandler : IRequestHandler<RequestResponseCommand, string>
{
    public Task<string> Handle(RequestResponseCommand request, CancellationToken cancellationToken = default)
    {
        var result = "result is " + request.Value;
        return Task.FromResult(result);
    }
}