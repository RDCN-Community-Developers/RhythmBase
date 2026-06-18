namespace RhythmBase.Rizline.Events;

/// <summary>
/// Represents an overall line color overlay event on a guide line.
/// </summary>
[JsonObjectSerializable]
public record class LineColor : BaseEvent, IColorDurationEvent
{
	/// <summary>
	/// The event type, always <see cref="EventType.LineColor"/>.
	/// </summary>
	public override EventType Type => EventType.LineColor;
	/// <summary>
	/// The starting color of the line color overlay.
	/// </summary>
	public Color StartColor { get; set; }
	/// <summary>
	/// The ending color of the line color overlay.
	/// </summary>
	public Color EndColor { get; set; }
}
