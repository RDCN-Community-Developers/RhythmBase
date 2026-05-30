using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components.Vector;
namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents a tile event in the rhythm base system.
/// </summary>
[JsonObjectSerializable]
public record class Tile : BaseDecorationAction, IEaseEvent
{
	///<inheritdoc/>
	public override EventType Type => EventType.Tile;
	///<inheritdoc/>
	public override Tab Tab => Tab.Decorations;

	/// <summary>
	/// Gets or sets the position of the tile.
	/// </summary>
	/// <remarks>
	/// Percentage of the screen width and height. (0,0) is the bottom-left corner, (100,100) is the top-right corner.
	/// Leave it null to keep the original position.
	/// </remarks>
	[Tween]
	public Point? Position { get; set; }
	/// <summary>
	/// Gets or sets the tiling of the tile.
	/// </summary>
	/// <remarks>
	/// The number of times the tile is repeated in the horizontal and vertical directions. (1,1) is the original tiling.
	/// Leave it null to keep the original tiling.
	/// </remarks>
	[Tween]
	public Point? Tiling { get; set; }
	/// <summary>
	/// Gets or sets the speed of the tile.
	/// </summary>
	/// <remarks>
	/// Tile size per second. (0,0) means the tile is fixed. Positive values mean the tile moves in the original direction, while negative values mean the tile moves in the opposite direction.
	/// Leave it null to keep the original speed.
	/// </remarks>
	[Tween]
	public Point? Speed { get; set; }
	/// <summary>
	/// Gets or sets the type of tiling.
	/// </summary>
	[JsonCondition($"$&.{nameof(Speed)} is not null")]
	public TilingType TilingType { get; set; }
	/// <summary>
	/// Gets or sets the interval for the tiling.
	/// </summary>
	/// <remarks>
	/// The beat interval for the tiling. It is only used when <see cref="TilingType"/> is <see cref="TilingType.Pulse"/>. The default value is 1.
	/// </remarks>
	[JsonCondition($"$&.{nameof(TilingType)} is RhythmBase.RhythmDoctor.{nameof(TilingType)}.{nameof(Tile.TilingType.Pulse)}")]
	public float Interval { get; set; }
	///<inheritdoc/>
	public EaseType Ease { get; set; }
	///<inheritdoc/>
	public float Duration { get; set; }
}
