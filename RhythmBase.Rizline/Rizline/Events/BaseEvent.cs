using RhythmBase.Rizline.Components;

namespace RhythmBase.Rizline.Events;

public abstract record class BaseEvent : IBaseEvent
{
    public abstract EventType Type { get; }
    public TickTime TickTime { get; }
}
