using MediatR;
using Microsoft.Extensions.Logging;

namespace WebApiApplication.Commands.Users.Notifiactions;

internal sealed class DeleteUserNotificationHandler(ILogger<DeleteUserNotificationHandler>? logger) : INotificationHandler<DeleteUserNotification>
{
    public Task Handle(DeleteUserNotification notification, CancellationToken cancellationToken)
    {
        logger?.LogInformation("User {name} deleted", notification.User.Login);
        return Task.CompletedTask;
    }
}
