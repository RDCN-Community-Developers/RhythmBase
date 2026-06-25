using RhythmBase.BeatBlock.Converters;
using RhythmBase.BeatBlock.Events;

namespace RhythmBase.BeatBlock.Components;

/// <summary>
/// Represents a collection of events associated with a tag in a BeatBlock level.
/// </summary>
public class TagEventCollection : OrderedEventCollection<IBaseEvent> { }