using RhythmBase.Global.Components.Easing;
namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Ease
/// </summary>
/// <remarks>
/// Eases a variable over time
/// </remarks>
[JsonObjectSerializable]
public record class Ease : BaseEvent, IEaseEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.Ease;
	/// <summary>
	/// Variable to ease (must be a child of cs)
	/// </summary>
	public string Var { get; set; } = string.Empty;
	/// <summary>
	/// Starting value
	/// </summary>
	public float? Start { get; set; }
	/// <summary>
	/// Ending value
	/// </summary>
	public float Value { get; set; }
	/// <summary>
	/// Length of ease
	/// </summary>
	[JsonCondition($"$&.{nameof(Duration)} == 0")]
	public float Duration { get; set; }
	/// <summary>
	/// Ease function to use
	/// </summary>
	[JsonAlias("ease")]
	public EaseType EaseType { get; set; }
	/// <summary>
	/// Times to repeat
	/// </summary>
	public int? Repeats { get; set; }
	/// <summary>
	/// Beats between repeats
	/// </summary>
	public float? RepeatDelay { get; set; }
	[JsonIgnore]
	EaseType IEaseEvent.Ease { get => EaseType; set => EaseType = value; }
}
