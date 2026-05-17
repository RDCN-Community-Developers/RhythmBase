namespace RhythmBase.Global.Events;

public interface IEvent
{
}
/// <summary>
/// Represents a marker interface for event types in an event-driven architecture.
/// </summary>
public interface IEvent<TType, TBeat> : IEvent
    where TType : struct, Enum
    where TBeat : struct, IBeat<TBeat>
{
    /// <summary>
    /// Gets the type of the event.
    /// </summary>
    TType Type { get; }

    /// <summary>
    /// Gets the beat of the event.
    /// </summary>
    TBeat Beat { get; }
}
