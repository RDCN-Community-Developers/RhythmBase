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
	public float Speed { get; set; }
	///<inheritdoc/>
	public float Duration { get; set; }
	///<inheritdoc/>
	public override EventType Type => EventType.SetSpeed;
	///<inheritdoc/>
	public override Tab Tab => Tab.Actions;
	///<inheritdoc/>
	public override string ToString() => base.ToString() + $" Speed:{Speed}";
}
