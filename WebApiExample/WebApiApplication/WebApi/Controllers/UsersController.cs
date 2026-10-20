using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiApplication.Commands.Users;
using WebApiApplication.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class UsersController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [AllowAnonymous]
    [HttpPut(nameof(Add))]
    public async Task<IActionResult> Add(UserWithPassword userWithPassword, CancellationToken cancellationToken = default)
    {
        var user = mapper.Map<WebApiDatabase.Models.User>(userWithPassword);
        await mediator.Send(new AddUserCommand() { User = user }, cancellationToken);
        return Ok();
    }

    [Authorize]
    [HttpGet(nameof(Get))]
    public async Task<User[]> Get(string? property, bool? asc, int? skip, int? take, CancellationToken cancellationToken = default)
    {
        var users = await mediator.Send(new GetUsersCommand() { Property = property, Asc = asc, Skip = skip, Take = take }, cancellationToken);

        var dto = mapper.Map<User[]>(users);
        return dto;
    }

    [Authorize]
    [HttpPost(nameof(Update))]
    public async Task<IActionResult> Update(UserWithPassword user, CancellationToken cancellationToken = default)
    {
        var db = mapper.Map<WebApiDatabase.Models.User>(user);
        await mediator.Send(new UpdateUserCommand() { User = db }, cancellationToken);
        return Ok();
    }

    [Authorize]
    [HttpDelete(nameof(Delete))]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        await mediator.Send(new DeleteUserCommand() { Id = id }, cancellationToken);
        return Ok();
    }
}
