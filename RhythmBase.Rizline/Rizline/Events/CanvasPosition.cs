using RhythmBase.Global.Components.Easing;

namespace RhythmBase.Rizline.Events;

[JsonObjectSerializable]
public record class CanvasPosition : BaseEvent, IKeyPointEvent
{
	public float FloorPosition { get; set; }
	public override EventType Type => EventType.CanvasPosition;
	[JsonAlias("easeType")]
	public EaseType Ease { get; set; }
	public float Duration { get; set; }
	public float Value { get; set; }
}
