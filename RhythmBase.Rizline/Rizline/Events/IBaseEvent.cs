using RhythmBase.Rizline.Components;

namespace RhythmBase.Rizline.Events;

public interface IBaseEvent : IEvent<EventType, TickTime>
{
	public EventType Type { get; }
}
