using MediatR;
using WebApiApplication.Commands.Users.Notifiactions;
using WebApiApplication.Interfaces;
using WebApiDatabase.Models;

namespace WebApiApplication.Commands.Users;

public sealed class AddUserCommand : IRequest
{
    public required User User { get; set; }
}

internal sealed class AddUserCommandHandler(IEntityService<int, User> userService, IMediator mediator) : IRequestHandler<AddUserCommand>
{
    public async Task Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        await userService.AddAsync(request.User, cancellationToken);
        await mediator.Publish(new AddUserNotification() { User = request.User });
    }
}