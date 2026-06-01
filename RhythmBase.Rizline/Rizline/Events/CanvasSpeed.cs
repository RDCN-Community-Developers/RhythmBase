using RhythmBase.Global.Components.Easing;
using RhythmBase.Rizline.Events;

namespace RhythmBase.Rizline.Rizline.Events;

[JsonObjectSerializable]
public record class CanvasSpeed : BaseEvent, IKeyPointEvent
{
	public float FloorPosition { get; set; }
	public override EventType Type => EventType.CanvasSpeed;
	[JsonAlias("easeType")]
	public EaseType Ease { get; set; }
	public float Duration { get; set; }
	public float Value { get; set; }
}
