namespace RhythmBase.Rizline.Events;

/// <summary>
/// Represents a judge ring color transition event on a guide line.
/// </summary>
[JsonObjectSerializable]
public record class JudgeRingColor : BaseEvent, IColorDurationEvent
{
	/// <summary>
	/// The event type, always <see cref="EventType.JudgeRingColor"/>.
	/// </summary>
	public override EventType Type => EventType.JudgeRingColor;
	/// <summary>
	/// The starting color of the judge ring transition.
	/// </summary>
	public Color StartColor { get; set; }
	/// <summary>
	/// The ending color of the judge ring transition.
	/// </summary>
	public Color EndColor { get; set; }
}
