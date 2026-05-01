using Zuppeto.Application.Events;

namespace Zuppeto.Application.Admin.Events;

public sealed record UserRoleChangedEvent(Guid UserId, string OldRole, string NewRole) : IDomainEvent
{
    public DateTimeOffset OccurredAtUtc { get; } = DateTimeOffset.UtcNow;
}
