namespace RhythmBase.Rizline.Events;

[JsonObjectSerializable]
public record class LineColor : BaseEvent, IColorDurationEvent
{
	public override EventType Type => EventType.LineColor;
	public Color StartColor { get; set; }
	public Color EndColor { get; set; }
}
