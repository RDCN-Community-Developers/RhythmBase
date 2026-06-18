namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Represents a forward-compatible event interface for unrecognized event types.
/// </summary>
public interface IForwardEvent : IBaseEvent, Global.Events.IForwardEvent
{
}