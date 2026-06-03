using RhythmBase.Global.Components.Easing;

namespace RhythmBase.Rizline.Events;

[JsonObjectSerializable]
public record class CameraPosition : BaseEvent, IKeyPointEvent
{
	public float FloorPosition { get; set; }
	public override EventType Type => EventType.CameraPosition;
	[JsonAlias("easeType")]
	public EaseType Ease { get; set; }
	public float Duration { get; set; }
	public float Value { get; set; }
}