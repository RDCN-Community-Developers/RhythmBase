using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components.Vector;
using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event to set the background color.
/// </summary>
[JsonObjectSerializable]
public record class SetBackgroundColor : BaseEvent, IEaseEvent, IRoomEvent, IColorEvent, IImageFileEvent
{
	/// <summary>
	/// Gets or sets the rooms associated with the event.
	/// </summary>
	public Room Rooms { get; set; } = new Room([0]);
	/// <summary>
	/// Gets or sets the easing type for the event.
	/// </summary>
	[JsonCondition($"""
		$&.{nameof(BackgroundType)} == RhythmBase.RhythmDoctor.{nameof(BackgroundType)}.{nameof(BackgroundType.Image)} &&
		$&.{nameof(ContentMode)} == RhythmBase.RhythmDoctor.{nameof(ContentMode)}.{nameof(ContentMode.Tiled)}
		""")]
	public EaseType Ease { get; set; } = EaseType.Linear;
	/// <summary>
	/// Gets or sets the content mode for the event.
	/// </summary>
	[JsonCondition($"$&.{nameof(BackgroundType)} == RhythmBase.RhythmDoctor.{nameof(BackgroundType)}.{nameof(BackgroundType.Image)}")]
	public ContentMode ContentMode { get; set; } = ContentMode.ScaleToFill;
	/// <summary>
	/// Gets or sets the filter mode for the event.
	/// </summary>
	public Filter Filter { get; set; } = Filter.NearestNeighbor;
	/// <summary>
	/// Gets or sets the color for the background.
	/// </summary>
	[Tween]
	public PaletteColorWithAlpha Color { get; set; } = Global.Components.Color.White;
	/// <summary>
	/// Gets or sets the interval beat for the event when the background type is set to image, content mode is set to tiled, and tiling type is set to pulse.
	/// </summary>
	/// <remarks>
	/// Recommended to be a value greater than or equals to 0.01 to avoid potential issues with extremely small values.
	/// </remarks>
	[JsonCondition($"""
		$&.{nameof(BackgroundType)} == RhythmBase.RhythmDoctor.{nameof(BackgroundType)}.{nameof(BackgroundType.Image)} &&
		$&.{nameof(ContentMode)} == RhythmBase.RhythmDoctor.{nameof(ContentMode)}.{nameof(ContentMode.Tiled)} &&
		$&.{nameof(TilingType)} == RhythmBase.RhythmDoctor.{nameof(TilingType)}.{nameof(TilingType.Pulse)}
		""")]
	public float Interval
	{
		get => field > 0.01f ? field : 0.01f;
		set => field = value > 0.01f ? value : 0.01f;
	}
	/// <summary>
	/// Gets or sets the background type for the event.
	/// </summary>
	public BackgroundType BackgroundType { get; set; } = BackgroundType.Color;
	/// <summary>
	/// Gets or sets the duration of the event.
	/// </summary>
	[JsonCondition($"""
		$&.{nameof(BackgroundType)} == RhythmBase.RhythmDoctor.{nameof(BackgroundType)}.{nameof(BackgroundType.Image)} &&
		$&.{nameof(ContentMode)} == RhythmBase.RhythmDoctor.{nameof(ContentMode)}.{nameof(ContentMode.Tiled)}
		""")]
	public float Duration { get; set; } = 0f;
	/// <summary>
	/// Gets or sets the frames per second for the event.
	/// </summary>
	[JsonCondition($"""
		$&.{nameof(BackgroundType)} == RhythmBase.RhythmDoctor.{nameof(BackgroundType)}.{nameof(BackgroundType.Image)} &&
		$&.{nameof(Images)}.Count > 1
		""")]
	public float Fps { get; set; } = 30f;
	/// <summary>
	/// Gets or sets the list of images for the background.
	/// </summary>
	[JsonCondition($"$&.{nameof(BackgroundType)} == RhythmBase.RhythmDoctor.{nameof(BackgroundType)}.{nameof(BackgroundType.Image)}")]
	[JsonAlias("image")]
	public List<FileReference> Images { get; set; } = [];
	/// <summary>
	/// Gets or sets the horizontal scroll value.
	/// </summary>
	[Tween]
	[JsonCondition($"""
		$&.{nameof(BackgroundType)} == RhythmBase.RhythmDoctor.{nameof(BackgroundType)}.{nameof(BackgroundType.Image)} &&
		$&.{nameof(ContentMode)} == RhythmBase.RhythmDoctor.{nameof(ContentMode)}.{nameof(ContentMode.Tiled)} &&
		!($&.{nameof(ScrollX)} is null || $&.{nameof(ScrollY)} is null)
		""")]
	public float? ScrollX
	{
		get => Speed.X;
		set
		{
			Point speed = Speed;
			speed.X = value;
			Speed = speed;
		}
	}
	/// <summary>
	/// Gets or sets the vertical scroll value.
	/// </summary>
	[Tween]
	[JsonCondition($"""
		$&.{nameof(BackgroundType)} == RhythmBase.RhythmDoctor.{nameof(BackgroundType)}.{nameof(BackgroundType.Image)} &&
		$&.{nameof(ContentMode)} == RhythmBase.RhythmDoctor.{nameof(ContentMode)}.{nameof(ContentMode.Tiled)} &&
		!($&.{nameof(ScrollX)} is null || $&.{nameof(ScrollY)} is null)
		""")]
	public float? ScrollY
	{
		get => Speed.Y;
		set
		{
			Point speed = Speed;
			speed.Y = value;
			Speed = speed;
		}
	}
	/// <summary>
	/// Gets or sets the speed of the background scrolling when the content mode is set to tiled.
	/// </summary>
	[JsonCondition($"""
		$&.{nameof(BackgroundType)} == RhythmBase.RhythmDoctor.{nameof(RhythmBase. RhythmDoctor.BackgroundType)}.{nameof(BackgroundType.Image)} &&
		$&.{nameof(ContentMode)} == RhythmBase.RhythmDoctor.{nameof(RhythmBase.RhythmDoctor.ContentMode)}.{nameof(ContentMode.Tiled)} &&
		($&.{nameof(ScrollX)} is null || $&.{nameof(ScrollY)} is null)
		""")]
	public Point Speed { get; set; } = new(0, 0);
	/// <summary>
	/// Gets or sets the tiling type for the background.
	/// </summary>
	[JsonCondition($"""
		$&.{nameof(BackgroundType)} == RhythmBase.RhythmDoctor.{nameof(RhythmBase.RhythmDoctor.BackgroundType)}.{nameof(BackgroundType.Image)} &&
		$&.{nameof(ContentMode)} == RhythmBase.RhythmDoctor.{nameof(RhythmBase.RhythmDoctor.ContentMode)}.{nameof(ContentMode.Tiled)}
		""")]
	public TilingType TilingType { get; set; } = TilingType.Scroll;
	/// <summary>
	/// Gets the event type.
	/// </summary>
	public override EventType Type => EventType.SetBackgroundColor;
	/// <summary>
	/// Gets the tab associated with the event.
	/// </summary>
	public override Tab Tab => Tab.Actions;
	IEnumerable<FileReference> IImageFileEvent.ImageFiles => [.. Images];
	IEnumerable<FileReference> IFileEvent.Files => [.. Images];

	/// <summary>
	/// Returns a string that represents the current object.
	/// </summary>
	public override string ToString() => BackgroundType == BackgroundType.Color
	? base.ToString() + $" {Color}"
	: base.ToString() + $" {string.Join(',', Images)}";
}