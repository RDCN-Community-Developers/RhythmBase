using RhythmBase.BeatBlock.Converters;
using RhythmBase.BeatBlock.Events;

namespace RhythmBase.BeatBlock.Components;

/// <summary>
/// Represents a collection of events associated with a tag in a BeatBlock level.
/// </summary>
public class TagEventCollection : OrderedEventCollection<IBaseEvent, EventType, TickTime>
{
    /// <inheritdoc/>
    protected override ReadOnlyEnumCollection<EventType> Types => EventTypeRegistry.ToEnums<IBaseEvent>();
    /// <inheritdoc/>
	protected override TickTime CreateInstance(float beat) => new(beat);
    /// <inheritdoc/>
	protected override ITickRange<TickTime> CreateRange(float? start, float? end) => new Range(start is null ? null : new TickTime(start.Value), end is null ? null : new TickTime(end.Value));
    /// <inheritdoc/>
	protected override ReadOnlyEnumCollection<EventType> TypesOf<TTarget>() => EventTypeRegistry.ToEnums(typeof(TTarget));
}