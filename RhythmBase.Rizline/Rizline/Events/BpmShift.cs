using RhythmBase.Global.Components.Easing;

namespace RhythmBase.Rizline.Events;

[JsonObjectSerializable]
public record class BpmShift : BaseEvent, IKeyPointEvent
{
	public float Value { get; set; }
	public override EventType Type => EventType.BpmShift;
	public float FloorPosition { get; set; }
	public EaseType Ease { get; set; }
	public float Duration { get; set; }
}