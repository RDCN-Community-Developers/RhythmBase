using RhythmBase.Rizline.Components;

namespace RhythmBase.Rizline.Events;

/// <summary>
/// Base class for all Rizline events, providing a type discriminator and tick-based timing.
/// </summary>
[JsonObjectHasSerializer(typeof(Converters.MemberConverter<>))]
public abstract record class BaseEvent : IBaseEvent
{
    /// <summary>
    /// The event type discriminator.
    /// </summary>
    public abstract EventType Type { get; }
    /// <summary>
    /// The time at which this event occurs, in ticks.
    /// </summary>
    public TickTime TickTime { get; set; }
}
