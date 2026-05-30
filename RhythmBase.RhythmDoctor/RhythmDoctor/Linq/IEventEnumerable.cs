using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Events;

namespace RhythmBase.RhythmDoctor.Linq;

/// <summary>
/// Represents a collection of events that can be enumerated.
/// </summary>
/// <typeparam name="TEvent">The type of event.</typeparam>
public interface IEventEnumerable<out TEvent> : Global.Linq.IEventEnumerable<TEvent, EventType, TickTime>
    where TEvent : IBaseEvent
{
}
