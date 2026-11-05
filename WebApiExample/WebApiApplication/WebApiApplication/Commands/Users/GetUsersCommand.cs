using MediatR;
using System.Linq.Expressions;
using WebApiApplication.Interfaces;
using WebApiDatabase.Models;

namespace WebApiApplication.Commands.Users;

public sealed class GetUsersCommand : IRequest<User[]>
{
    public int? Id { get; init; }
    public Expression<Func<User, bool>>? Predicate {  get; init; }
    public Models.Filter? Filter { get; init; }
}

internal sealed class GetUsersCommandHandler(IEntityService<int, User> userService) : IRequestHandler<GetUsersCommand, User[]>
{
    public Task<User[]> Handle(GetUsersCommand request, CancellationToken cancellationToken)
    {
        if (request.Id.HasValue)
            return userService.ReadAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        return userService.ReadAsync(request.Predicate, request.Filter, cancellationToken);
    }
}