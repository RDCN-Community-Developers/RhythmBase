using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components.Vector;
using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents a move event in the rhythm base system.
/// </summary>
[JsonObjectSerializable]
public record class Move : BaseDecorationAction, IEaseEvent
{
	///<inheritdoc/>
	public override EventType Type => EventType.Move;
	///<inheritdoc/>
	public override Tab Tab => Tab.Decorations;

	/// <summary>
	/// Gets or sets the position of the move event.
	/// </summary>
	/// <remarks>
	/// Percentage of the screen width and height. (0,0) is the bottom-left corner, (100,100) is the top-right corner.
	/// Leave it null to keep the original position.
	/// </remarks>
	[Tween]
	public PointE? Position { get; set; }
	/// <summary>
	/// Gets or sets the scale of the move event.
	/// </summary>
	/// <remarks>
	/// Percentage of the original size. (100,100) is the original size.
	/// Leave it null to keep the original size.
	/// </remarks>
	[Tween]
	public SizeE? Scale { get; set; }
	/// <summary>
	/// Gets or sets the angle of the move event.
	/// </summary>
	/// <remarks>
	/// Degree. (0) is the original angle.
	/// Leave it null to keep the original angle.
	/// </remarks>
	[Tween]
	public Expression? Angle { get; set; }
	/// <summary>
	/// Gets or sets the pivot point of the move event.
	/// <remark>
	/// Percentage of the original size. (0,0) is the bottom-left corner, (100,100) is the top-right corner.
	/// Leave it null to keep the original pivot point.
	/// </remark>
	/// </summary>
	[Tween]
	public Point? Pivot { get; set; }
	///<inheritdoc/>
	public float Duration { get; set; }
	///<inheritdoc/>
	public EaseType Ease { get; set; } = EaseType.Linear;
	///<inheritdoc/>
	public override string ToString() => base.ToString();
}
