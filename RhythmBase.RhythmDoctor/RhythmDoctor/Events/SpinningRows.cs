using RhythmBase.Global.Components.Easing;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event that applies a spinning rows animation with configurable easing and duration.
/// </summary>
[JsonObjectSerializable]
public record class SpinningRows : BaseRowAction, IEaseEvent
{
	/// <summary>
	/// The text to display during the spinning animation (if applicable).
	/// </summary>
	public string Text { get; set; } = "";

	/// <summary>
	/// Specifies the type of spinning action to perform.
	/// </summary>
	public SpiningAction Action { get; set; }

	/// <summary>
	/// Target row index to connect to when <see cref="Action"/> is <see cref="SpiningAction.Connect"/>.
	/// </summary>
	[JsonCondition($"$&.{nameof(Action)} is RhythmBase.RhythmDoctor.{nameof(SpiningAction)}.{nameof(SpiningAction.Connect)}")]
	public int ToRow { get; set; }

	/// <summary>
	/// Rotation angle in degrees used by rotation-related actions.
	/// </summary>
	/// <remarks>
	/// Degree. (0) is the original angle.
	/// </remarks>
	[JsonCondition($"""
			$&.{nameof(Action)}
			is RhythmBase.RhythmDoctor.{nameof(SpiningAction)}.{nameof(SpiningAction.Rotate)}
			or RhythmBase.RhythmDoctor.{nameof(SpiningAction)}.{nameof(SpiningAction.ConstantRotation)}
			or RhythmBase.RhythmDoctor.{nameof(SpiningAction)}.{nameof(SpiningAction.Split)}
			or RhythmBase.RhythmDoctor.{nameof(SpiningAction)}.{nameof(SpiningAction.Merge)}
			""")]
	public float Angle { get; set; }

	/// <summary>
	/// Amplitude for wavy rotation effects.
	/// </summary>
	[JsonCondition($"""
			$&.{nameof(Amplitude)} is not null &&
			$&.{nameof(Action)}
			is RhythmBase.RhythmDoctor.{nameof(SpiningAction)}.{nameof(SpiningAction.WavyRotation)}
			""")]
	public float? Amplitude { get; set; }

	/// <summary>
	/// Frequency for wavy rotation effects.
	/// </summary>
	[JsonCondition($"""
			$&.{nameof(Amplitude)} is not null &&
			$&.{nameof(Action)}
			is RhythmBase.RhythmDoctor.{nameof(SpiningAction)}.{nameof(SpiningAction.WavyRotation)}
			""")]
	public float? Frequency { get; set; }

	/// <summary>
	/// When true, additional visual effects are applied during merge operations.
	/// </summary>
	[JsonCondition($"$&.{nameof(Action)} is RhythmBase.RhythmDoctor.{nameof(SpiningAction)}.{nameof(SpiningAction.Merge)}")]
	public bool DoEffects { get; set; }

	///<inheritdoc/>
	[JsonCondition($"""
			$&.{nameof(Action)}
			is RhythmBase.RhythmDoctor.{nameof(SpiningAction)}.{nameof(SpiningAction.Rotate)}
			or RhythmBase.RhythmDoctor.{nameof(SpiningAction)}.{nameof(SpiningAction.ConstantRotation)}
			or RhythmBase.RhythmDoctor.{nameof(SpiningAction)}.{nameof(SpiningAction.Split)}
			or RhythmBase.RhythmDoctor.{nameof(SpiningAction)}.{nameof(SpiningAction.Merge)}
			or RhythmBase.RhythmDoctor.{nameof(SpiningAction)}.{nameof(SpiningAction.WavyRotation)}
			""")]
	public EaseType Ease { get; set; }

	///<inheritdoc/>
	[JsonCondition($"""
			$&.{nameof(Action)}
			is RhythmBase.RhythmDoctor.{nameof(SpiningAction)}.{nameof(SpiningAction.Rotate)}
			or RhythmBase.RhythmDoctor.{nameof(SpiningAction)}.{nameof(SpiningAction.ConstantRotation)}
			or RhythmBase.RhythmDoctor.{nameof(SpiningAction)}.{nameof(SpiningAction.Split)}
			or RhythmBase.RhythmDoctor.{nameof(SpiningAction)}.{nameof(SpiningAction.Merge)}
			or RhythmBase.RhythmDoctor.{nameof(SpiningAction)}.{nameof(SpiningAction.WavyRotation)}
			""")]
	public float Duration { get; set; }
	///<inheritdoc/>
	public override EventType Type => EventType.SpinningRows;

	///<inheritdoc/>
	public override Tab Tab => Tab.Actions;
}