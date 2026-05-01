using Zuppeto.Application.Admin;
using Zuppeto.Domain.Navigation;

namespace Zuppeto.Application.Factories;

public interface IMenuItemDefinitionFactory
{
    MenuItemDefinition Create(SaveMenuRequest request);
}
