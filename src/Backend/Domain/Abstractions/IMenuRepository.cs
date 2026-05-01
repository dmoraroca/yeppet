using Zuppeto.Domain.Navigation;

namespace Zuppeto.Domain.Abstractions;

public interface IMenuRepository
{
    Task<IReadOnlyCollection<MenuItemDefinition>> GetDefinitionsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<MenuRoleAssignment>> GetAssignmentsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<MenuItemDefinition>> GetMenuItemsByRoleAsync(
        string roleKey,
        CancellationToken cancellationToken = default);

    Task SaveDefinitionAsync(
        MenuItemDefinition definition,
        CancellationToken cancellationToken = default);

    Task ReplaceMenuRolesAsync(
        string menuKey,
        IReadOnlyCollection<string> roles,
        CancellationToken cancellationToken = default);

    Task<bool> HasChildMenusAsync(string parentKey, CancellationToken cancellationToken = default);

    Task<bool> TryDeleteByKeyAsync(string key, CancellationToken cancellationToken = default);
}
