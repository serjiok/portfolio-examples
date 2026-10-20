using MediatR;
using Microsoft.Extensions.Logging;

namespace WebApiApplication.Commands.Users.Notifiactions;

internal sealed class AddUserNotificationHandler(ILogger<AddUserNotificationHandler>? logger) : INotificationHandler<AddUserNotification>
{
    public Task Handle(AddUserNotification notification, CancellationToken cancellationToken)
    {
        logger?.LogInformation("New user {name}", notification.User.Login);
        return Task.CompletedTask;
    }
}
