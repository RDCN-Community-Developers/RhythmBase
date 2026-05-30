using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components.Vector;
using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event to move a row with various properties such as position, scale, angle, and pivot.
/// </summary>
[JsonObjectSerializable]
public record class MoveRow : BaseRowAction, IEaseEvent
{
	/// <summary>
	/// Gets or sets a value indicating whether a custom position is used.
	/// </summary>
	[JsonAlias("customPosition")]
	[JsonCondition($"$&.{nameof(Target)} is RhythmBase.RhythmDoctor.{nameof(MoveRowTarget)}.{nameof(MoveRowTarget.WholeRow)}")]
	public bool EnableCustomPosition { get; set; } = true;
	/// <summary>
	/// Gets or sets the target of the move row event.
	/// </summary>
	public MoveRowTarget Target { get; set; } = MoveRowTarget.WholeRow;
	/// <summary>
	/// Gets or sets the row position.
	/// </summary>
	/// <remarks>
	/// Percentage of the screen width and height. (0,0) is the bottom-left corner, (100,100) is the top-right corner.
	/// Leave it null to keep the original position.
	/// </remarks>
	[Tween]
	[JsonAlias("rowPosition")]
	[JsonCondition($"$&.{nameof(EnableCustomPosition)} is true")]
	public PointE? Position { get; set; }
	/// <summary>
	/// Gets or sets the scale.
	/// </summary>
	/// <remarks>
	/// Percentage of the original size. (100,100) is the original size.
	/// Leave it null to keep the original size.
	/// </remarks>
	[Tween]
	public SizeE? Scale { get; set; }
	/// <summary>
	/// Gets or sets the angle.
	/// </summary>
	/// <remarks>
	/// Degree. (0) is the original angle.
	/// Leave it null to keep the original angle.
	/// </remarks>
	[Tween]
	public Expression? Angle { get; set; }
	/// <summary>
	/// Gets or sets the pivot.
	/// </summary>
	/// <remarks>
	/// Percentage of the original size. (0,0) is the bottom-left corner, (100,100) is the top-right corner.
	/// Leave it null to keep the original pivot.
	/// </remarks>
	[Tween]
	[JsonCondition($"$&.{nameof(Target)} is RhythmBase.RhythmDoctor.{nameof(MoveRowTarget)}.{nameof(MoveRowTarget.WholeRow)}")]
	public float? Pivot { get; set; }
	///<inheritdoc/>
	[JsonCondition($"""
		$&.{nameof(EnableCustomPosition)}
		""")]
	public float Duration { get; set; } = 1;
	/// <summary>
	/// Gets or sets the acceleration duration for the move row event.
	/// This value defines how long the acceleration phase lasts at the start of the movement.
	/// </summary>
	[JsonCondition($"""
		$&.{nameof(AccelerationDuration)} is not null &&
		($&.{nameof(EnableCustomPosition)} || $&.{nameof(Target)} is RhythmBase.RhythmDoctor.{nameof(MoveRowTarget)}.{nameof(MoveRowTarget.WholeRow)})
		""")]
	public float? AccelerationDuration { get; set; }
	/// <summary>
	/// Gets or sets the deceleration duration for the move row event.
	/// This value defines how long the deceleration phase lasts at the end of the movement.
	/// </summary>
	[JsonCondition($"""
		$&.{nameof(AccelerationDuration)} is not null &&
		($&.{nameof(EnableCustomPosition)} || $&.{nameof(Target)} is RhythmBase.RhythmDoctor.{nameof(MoveRowTarget)}.{nameof(MoveRowTarget.WholeRow)})
		""")]
	public float? DecelerationDuration { get; set; }
	///<inheritdoc/>
	[JsonCondition($"""
		$&.{nameof(EnableCustomPosition)}
		""")]
	public EaseType Ease { get; set; } = EaseType.Linear;
	///<inheritdoc/>
	public override EventType Type => EventType.MoveRow;
	///<inheritdoc/>
	public override Tab Tab => Tab.Actions;
}