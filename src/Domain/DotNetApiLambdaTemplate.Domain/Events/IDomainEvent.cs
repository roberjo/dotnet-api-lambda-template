namespace DotNetApiLambdaTemplate.Domain.Events;

/// <summary>
/// Marker interface for domain events
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Unique identifier for the event
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// When the event occurred
    /// </summary>
    DateTime OccurredOn { get; }

    /// <summary>
    /// The type of the event
    /// </summary>
    string EventType { get; }
}
