using RhythmBase.Rizline.Components;

namespace RhythmBase.Rizline.Events;

[JsonObjectHasSerializer(typeof(Converters.MemberConverter<>))]
public abstract record class BaseEvent : IBaseEvent
{
    public abstract EventType Type { get; }
    public TickTime TickTime { get; set; }
}
