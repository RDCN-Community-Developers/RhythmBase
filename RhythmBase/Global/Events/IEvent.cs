namespace RhythmBase.Global.Events;

/// <summary>
/// Represents a base interface for all events.
/// </summary>
public interface IEvent
{
}
/// <summary>
/// Represents a marker interface for event types in an event-driven architecture.
/// </summary>
public interface IEvent<TType, TTick> : IEvent
    where TType : struct, Enum
    where TTick : struct, ITickTime<TTick>
{
    /// <summary>
    /// Gets the type of the event.
    /// </summary>
    TType Type { get; }

    /// <summary>
    /// Gets the beat of the event.
    /// </summary>
    TTick TickTime { get; }
}
