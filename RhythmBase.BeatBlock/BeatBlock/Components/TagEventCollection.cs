using RhythmBase.BeatBlock.Events;

namespace RhythmBase.BeatBlock.Components;

public class TagEventCollection : OrderedEventCollection<IBaseEvent, EventType, TickTime>
{
    internal override ReadOnlyEnumCollection<EventType> Types => Utils.EventTypeUtils.ToEnums<IBaseEvent>();
    internal override TickTime CreateInstance(float beat) => new TickTime(beat);
    internal override ITickRange<TickTime> CreateRange(float? start, float? end) => new BBRange(start, end);
    internal override ReadOnlyEnumCollection<EventType> TypesOf<TTarget>() => Utils.EventTypeUtils.ToEnums(typeof(TTarget));
}