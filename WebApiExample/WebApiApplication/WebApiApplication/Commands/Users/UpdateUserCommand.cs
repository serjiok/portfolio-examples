using MediatR;
using WebApiApplication.Commands.Users.Notifiactions;
using WebApiApplication.Interfaces;
using WebApiDatabase.Models;

namespace WebApiApplication.Commands.Users;

public sealed class UpdateUserCommand : IRequest
{
    public required User User { get; set; }
}

internal sealed class UpdateUserCommandHandler(IEntityService<int, User> userService, IMediator mediator) : IRequestHandler<UpdateUserCommand>
{
    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        await userService.UpdateAsync(request.User, cancellationToken);
        await mediator.Publish(new UpdateUserNotification() { User = request.User });
    }
}
