using MediatR;
using WebApiDatabase.Models;

namespace WebApiApplication.Commands.Users.Notifiactions;

public class UserNotification : INotification
{
    public required User User { get; set; }
}

public sealed class AddUserNotification : UserNotification { }
public sealed class UpdateUserNotification : UserNotification { }
public sealed class DeleteUserNotification : UserNotification { }