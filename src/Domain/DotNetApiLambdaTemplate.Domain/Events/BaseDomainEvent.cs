using DotNetApiLambdaTemplate.Domain.Events;

namespace DotNetApiLambdaTemplate.Domain.Events;

/// <summary>
/// Base implementation for domain events
/// </summary>
public abstract class BaseDomainEvent : IDomainEvent
{
    /// <summary>
    /// Unique identifier for the event
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// When the event occurred
    /// </summary>
    public DateTime OccurredOn { get; }

    /// <summary>
    /// The type of the event
    /// </summary>
    public string EventType { get; }

    protected BaseDomainEvent()
    {
        Id = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        EventType = GetType().Name;
    }
}
