using RhythmBase.Rizline.Events;

namespace RhythmBase.Rizline.Rizline.Events;

[JsonObjectSerializable]
public record class JudgeRingColor : BaseEvent, IColorDurationEvent
{
	public override EventType Type => EventType.JudgeRingColor;
	public Color StartColor { get; set; }
	public Color EndColor { get; set; }
}
