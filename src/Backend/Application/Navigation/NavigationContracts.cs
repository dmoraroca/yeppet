namespace Zuppeto.Application.Navigation;

public sealed record NavigationMenuItemDto(
    string Key,
    string Label,
    string? Route,
    IReadOnlyCollection<NavigationMenuItemDto> Children);
