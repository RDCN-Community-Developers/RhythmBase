using RhythmBase.Global.Components.Easing;
namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event that sets the speed in the rhythm base.
/// </summary>
[JsonObjectSerializable]
public record class SetSpeed : BaseEvent, IEaseEvent
{
	///<inheritdoc/>
	public EaseType Ease { get; set; } = EaseType.Linear;
	/// <summary>
	/// Gets or sets the speed for the event.
	/// </summary>
	/// <remarks>
	/// Must be a non-negative value.
	/// 1 is the original speed, less than 1 is slower, and greater than 1 is faster.
	/// </remarks>
	[Tween]
	[ValueRange<float>(0f, float.PositiveInfinity)]
	public float Speed { get; set; }
	///<inheritdoc/>
	[ValueRange<float>(0f, float.PositiveInfinity)]
	public float Duration { get; set; }
	///<inheritdoc/>
	public override EventType Type => EventType.SetSpeed;
	///<inheritdoc/>
	public override Tab Tab => Tab.Actions;
}
