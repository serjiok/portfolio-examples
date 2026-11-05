using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApi.Services;
using WebApiApplication.Commands;
using WebApiApplication.Models;

namespace WebApi.Controllers;

[AllowAnonymous]
[ApiController]
public sealed class LoginController(IConfiguration configuration, IMediator mediator, ILogger<LoginController>? logger) : ControllerBase
{
    [HttpPost(nameof(Login))]
    public async Task<IActionResult> Login(LoginAndPassword loginAndPassword, CancellationToken cancellationToken = default)
    {
        var isSuccess = await mediator.Send(new LoginCommand() 
        { 
            Login = loginAndPassword.Login, 
            Password = loginAndPassword.Password 
        }, cancellationToken);

        if (isSuccess)
        {
            string token = TokenFactory.CreateToken(configuration, loginAndPassword.Login, ["user"]);
            logger?.LogInformation("Login in system jwt {login}", loginAndPassword.Login);
            return Ok(token);
        }
        return Forbid();
    }

    [AllowAnonymous]
    [HttpPost(nameof(LoginCookie))]
    public async Task<IActionResult> LoginCookie(LoginAndPassword loginAndPassword, CancellationToken cancellationToken = default)
    {
        var isSuccess = await mediator.Send(new LoginCommand()
        {
            Login = loginAndPassword.Login,
            Password = loginAndPassword.Password
        }, cancellationToken);

        if (isSuccess)
        {
            ClaimsIdentity claimsIdentity = new(TokenFactory.GetClaims(loginAndPassword.Login, ["user"]), CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(claimsIdentity);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
            };

            await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            authProperties);

            logger?.LogInformation("Login in system cookie {login}", loginAndPassword.Login);
            return Ok();
        }
        return Forbid();
    }
}
