using Zuppeto.Application.Admin;
using Zuppeto.Domain.Navigation;

namespace Zuppeto.Application.Factories;

public sealed class MenuItemDefinitionFactory : IMenuItemDefinitionFactory
{
    public MenuItemDefinition Create(SaveMenuRequest request)
    {
        return new MenuItemDefinition(
            request.Key.Trim(),
            request.Label.Trim(),
            string.IsNullOrWhiteSpace(request.Route) ? null : request.Route.Trim(),
            string.IsNullOrWhiteSpace(request.ParentKey) ? null : request.ParentKey.Trim(),
            request.SortOrder,
            request.IsActive);
    }
}
