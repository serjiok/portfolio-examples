namespace WebApiApplication.Models;

public sealed class UserWithPassword : User
{
    public required string Password { get; set; }
}
