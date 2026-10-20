using MediatR;
using WebApiApplication.Commands.Users.Notifiactions;
using WebApiApplication.Interfaces;
using WebApiDatabase.Models;

namespace WebApiApplication.Commands.Users;

public sealed class DeleteUserCommand : IRequest
{
    public required int Id { get; set; }
}

internal sealed class DeleteUserCommandHandler(IEntityService<int, User> userService, IMediator mediator) : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = (await userService.ReadAsync(x => x.Id == request.Id)).First();
        await userService.DeleteAsync(request.Id, cancellationToken);
        await mediator.Publish(new DeleteUserNotification() { User = user });
    }
}