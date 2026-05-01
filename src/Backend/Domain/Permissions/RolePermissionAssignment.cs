namespace Zuppeto.Domain.Permissions;

public sealed record RolePermissionAssignment(
    string Role,
    string PermissionKey);
