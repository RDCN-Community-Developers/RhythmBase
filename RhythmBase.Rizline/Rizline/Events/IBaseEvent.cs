using RhythmBase.Global.Events;
using RhythmBase.Rizline.Components;
using RhythmBase.Rizline.Rizline;

namespace RhythmBase.Rizline.Events;

public interface IBaseEvent : IEvent<EventType, TickTime>
{
    public EventType Type { get; }
}
