using RhythmBase.Rizline.Components;
using RhythmBase.Rizline.Rizline;

namespace RhythmBase.Rizline.Events;

[JsonObjectHasSerializer(typeof(Converters.MemberConverter<>))]
public abstract record class BaseEvent : IBaseEvent
{
    public abstract EventType Type { get; }
    public TickTime TickTime { get; }
}
