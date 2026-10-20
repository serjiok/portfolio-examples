using MediatR;
using Microsoft.Extensions.Logging;

namespace WebApiApplication.Commands.Users.Notifiactions;

internal sealed class UpdateUserNotification1Handler(ILogger<UpdateUserNotification1Handler>? logger) : INotificationHandler<UpdateUserNotification>
{
    public Task Handle(UpdateUserNotification notification, CancellationToken cancellationToken)
    {
        logger?.LogInformation("Advance action 1 from update user {name}", notification.User.Login);
        return Task.CompletedTask;
    }
}

internal sealed class UpdateUserNotification2Handler(ILogger<UpdateUserNotification2Handler>? logger) : INotificationHandler<UpdateUserNotification>
{
    public Task Handle(UpdateUserNotification notification, CancellationToken cancellationToken)
    {
        logger?.LogInformation("Advance action 2 from update user {name}", notification.User.Login);
        return Task.CompletedTask;
    }
}
