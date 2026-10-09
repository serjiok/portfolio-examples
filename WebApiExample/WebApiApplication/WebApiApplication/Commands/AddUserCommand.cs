using MediatR;
using WebApiApplication.Interfaces;
using WebApiDatabase.Models;

namespace WebApiApplication.Commands;

public sealed class AddUserCommand : IRequest
{
    public required User User { get; set; }
}

internal sealed class AddUserCommandHandler(IEntityService<int, User> userService) : IRequestHandler<AddUserCommand>
{
    public Task Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        return userService.AddAsync(request.User, cancellationToken);
    }
}