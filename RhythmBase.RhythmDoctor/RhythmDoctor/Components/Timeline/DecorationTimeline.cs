using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components.Vector;
namespace RhythmBase.RhythmDoctor.Components.TimeLine;

/// <summary>
/// Represents the timeline of a decoration, including its properties and animation curves over time.
/// </summary>
public record class DecorationTimeline
{
#pragma warning disable CS8618
	/// <summary>
	/// Gets or sets the decoration associated with this timeline.
	/// </summary>
	public Decoration Decoration { get; internal set; }

	/// <summary>
	/// Gets or sets the position curve of the decoration over time.
	/// </summary>
	public VectorTweenCurve<PointN> Position { get; internal set; }

	/// <summary>
	/// Gets or sets the 
	/// curve of the decoration over time.
	/// </summary>
	public VectorTweenCurve<PointN> Pivot { get; internal set; }

	/// <summary>
	/// Gets or sets the angle curve of the decoration over time.
	/// </summary>
	public TweenCurve<float> Angle { get; internal set; }

	/// <summary>
	/// Gets or sets the size curve of the decoration over time.
	/// </summary>
	public VectorTweenCurve<SizeN> Size { get; internal set; }

	/// <summary>
	/// Gets or sets the visibility curve of the decoration over time.
	/// </summary>
	public Curve<bool> Visible { get; internal set; }

	/// <summary>
	/// Gets or sets the depth curve of the decoration over time.
	/// </summary>
	public Curve<int> Depth { get; internal set; }

	/// <summary>
	/// Gets or sets the room curve of the decoration over time.
	/// </summary>
	public Curve<RoomIndex> Room { get; internal set; }

	/// <summary>
	/// Gets or sets the blend type curve of the decoration over time.
	/// </summary>
	public Curve<BlendType> Blend { get; internal set; }

	/// <summary>
	/// Gets or sets the animation curve of the decoration over time.
	/// </summary>
	public Curve<string> Animation { get; internal set; }

	/// <summary>
	/// Gets or sets the border type curve of the decoration over time.
	/// </summary>
	public Curve<Border> Border { get; internal set; }

	/// <summary>
	/// Gets or sets the border color curve of the decoration over time.
	/// </summary>
	public TweenCurve<Color> BorderColor { get; internal set; }

	/// <summary>
	/// Gets or sets the tint color curve of the decoration over time.
	/// </summary>
	public TweenCurve<Color> TintColor { get; internal set; }

	/// <summary>
	/// Gets or sets the opacity curve of the decoration over time.
	/// </summary>
	public TweenCurve<int> Opacity { get; internal set; }

	/// <summary>
	/// Gets or sets the tile position curve of the decoration over time.
	/// </summary>
	public VectorTweenCurve<PointN> TilePosition { get; internal set; }

	/// <summary>
	/// Gets or sets the tile tiling curve of the decoration over time.
	/// </summary>
	public VectorTweenCurve<PointN> TileTiling { get; internal set; }

	/// <summary>
	/// Gets or sets the tile speed curve of the decoration over time.
	/// </summary>
	public VectorTweenCurve<PointN> TileSpeed { get; internal set; }
}
