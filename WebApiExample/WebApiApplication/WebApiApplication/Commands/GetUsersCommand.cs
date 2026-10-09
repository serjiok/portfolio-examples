using MediatR;
using System.Linq.Expressions;
using WebApiApplication.Interfaces;
using WebApiDatabase.Models;

namespace WebApiApplication.Commands;

public sealed class GetUsersCommand : IRequest<User[]>
{
    public Expression<Func<User, bool>>? Predicate {  get; init; }
    public string? Property { get; init; }
    public bool? Asc {  get; init; }
    public int? Skip { get; init; }
    public int? Take { get; init; }
}

internal sealed class GetUsersCommandHandler(IEntityService<int, User> userService) : IRequestHandler<GetUsersCommand, User[]>
{
    public Task<User[]> Handle(GetUsersCommand request, CancellationToken cancellationToken)
    {
        (string, bool)? order = string.IsNullOrEmpty(request.Property) ? null : (request.Property, request.Asc ?? false);
        return userService.ReadAsync(request.Predicate, order, request.Skip, request.Take, cancellationToken);
    }
}