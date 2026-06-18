using RhythmBase.Rizline.Components;

namespace RhythmBase.Rizline.Events;

/// <summary>
/// Common interface for all Rizline events with a type discriminator and tick-based timing.
/// </summary>
public interface IBaseEvent : IEvent<EventType, TickTime>
{
	/// <summary>
	/// The event type discriminator.
	/// </summary>
	public EventType Type { get; }
}
