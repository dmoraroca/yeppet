using Zuppeto.Application.Commands;
using Zuppeto.Application.Results;
using Zuppeto.Application.Users;

namespace Zuppeto.Application.Admin.Commands;

public sealed record UpdateUserRoleCommand(Guid UserId, UpdateUserRoleRequest Request) : ICommand<Result<UserDto>>;
