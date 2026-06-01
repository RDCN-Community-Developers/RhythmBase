using RhythmBase.BeatBlock.Converters;
using RhythmBase.BeatBlock.Events;

namespace RhythmBase.BeatBlock.Components;

public class TagEventCollection : OrderedEventCollection<IBaseEvent, EventType, TickTime>
{
    protected override ReadOnlyEnumCollection<EventType> Types => ClassEnumMap.ToEnums<IBaseEvent>();
	protected override TickTime CreateInstance(float beat) => new TickTime(beat);
	protected override ITickRange<TickTime> CreateRange(float? start, float? end) => new BBRange(start, end);
	protected override ReadOnlyEnumCollection<EventType> TypesOf<TTarget>() => ClassEnumMap.ToEnums(typeof(TTarget));
}