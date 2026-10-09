using MediatR;
using WebApiApplication.Interfaces;
using WebApiDatabase.Models;

namespace WebApiApplication.Commands;

public sealed class LoginCommand : IRequest<bool>
{
    public required string Login { get; init; } 
    public required string Password { get; init; }
}

public sealed class LoginCommandHandler(IEntityService<int, User> userService) : IRequestHandler<LoginCommand, bool>
{
    public Task<bool> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return userService.Any(x => x.Login == request.Login && x.Password == request.Password, cancellationToken: cancellationToken);
    }
}
