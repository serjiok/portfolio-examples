using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services;
using WebApiApplication.Commands;
using WebApiApplication.Models;

namespace WebApi.Controllers;

[AllowAnonymous]
[ApiController]
public sealed class LoginController(IConfiguration configuration, IMediator mediator) : ControllerBase
{
    [HttpPost(nameof(Login))]
    public async Task<IActionResult> Login(LoginAndPassword loginAndPassword, CancellationToken cancellationToken = default)
    {
        var isSuccess = await mediator.Send(new LoginCommand() 
        { 
            Login = loginAndPassword.Login, 
            Password = loginAndPassword.Password 
        });

        if (isSuccess)
        {
            string token = TokenFactory.CreateToken(configuration, loginAndPassword.Login, ["user"]);
            return Ok(token);
        }
        return Forbid();
    }
}
