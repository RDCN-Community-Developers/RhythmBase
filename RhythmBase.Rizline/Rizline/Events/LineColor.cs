using RhythmBase.Rizline.Events;

namespace RhythmBase.Rizline.Rizline.Events;

[JsonObjectSerializable]
public record class LineColor : BaseEvent, IColorDurationEvent
{
	public override EventType Type => EventType.LineColor;
	public Color StartColor { get; set; }
	public Color EndColor { get; set; }
}
