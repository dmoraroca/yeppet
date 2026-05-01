namespace Zuppeto.Application.Events;

public interface IDomainEvent
{
    DateTimeOffset OccurredAtUtc { get; }
}
