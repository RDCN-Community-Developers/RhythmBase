using RhythmBase.Adofai.Serialization;

namespace RhythmBase.Adofai.Events;

/// <summary>
/// Represents a base class for angle-offset events that also carry an event tag.
/// In this model it belongs to the time-authoritative branch (<see cref="BaseEvent"/>) because
/// the presence of <see cref="BaseEvent.AngleOffset"/> is what makes an event freely placeable,
/// not tile-bound.
/// </summary>
[JsonObjectHasSerializer(typeof(MemberConverterBaseTaggedTileEvent<>))]
public abstract class BaseTaggedTileEvent : BaseEvent
{
	/// <summary>
	/// Gets or sets the event tag associated with the event.
	/// </summary>
	public string EventTag { get; set; } = string.Empty;
}
