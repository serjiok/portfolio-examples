using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiApplication.Commands;
using WebApiApplication.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class UsersController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [AllowAnonymous]
    [HttpPut(nameof(Registration))]
    public async Task<IActionResult> Registration(UserWithPassword userWithPassword, CancellationToken cancellationToken = default)
    {
        var user = mapper.Map<WebApiDatabase.Models.User>(userWithPassword);
        await mediator.Send(new AddUserCommand() { User = user });
        return Ok();
    }

    [Authorize]
    [HttpGet(nameof(Get))]
    public async Task<User[]> Get(string? property, bool? asc, int? skip, int? take, CancellationToken cancellationToken = default)
    {
        var users = await mediator.Send(new GetUsersCommand() { Property = property, Asc = asc, Skip = skip, Take = take });

        var dto = mapper.Map<User[]>(users);
        return dto;
    }
}
