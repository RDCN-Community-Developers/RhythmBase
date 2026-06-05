using RhythmBase.BeatBlock.Converters;
using RhythmBase.BeatBlock.Events;

namespace RhythmBase.BeatBlock.Components;

public class TagEventCollection : OrderedEventCollection<IBaseEvent, EventType, TickTime>
{
    protected override ReadOnlyEnumCollection<EventType> Types => ClassEnumMap.ToEnums<IBaseEvent>();
	protected override TickTime CreateInstance(float beat) => new(beat);
	protected override ITickRange<TickTime> CreateRange(float? start, float? end) => new Range(start is null ? null : new TickTime(start.Value), end is null ? null : new TickTime(end.Value));
	protected override ReadOnlyEnumCollection<EventType> TypesOf<TTarget>() => ClassEnumMap.ToEnums(typeof(TTarget));
}