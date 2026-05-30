using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components.Vector;
using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>  
/// Represents an event to set the foreground in a room.  
/// </summary>  
[JsonObjectSerializable]
public record class SetForeground : BaseEvent, IEaseEvent, IRoomEvent, IColorEvent, IImageFileEvent
{
	/// <summary>  
	/// Gets or sets the rooms associated with the event.  
	/// </summary>  
	public Room Rooms { get; set; } = new Room([0]);
	/// <summary>  
	/// Gets or sets the content mode for the event.  
	/// </summary>  
	public ContentMode ContentMode { get; set; } = ContentMode.ScaleToFill;
	/// <summary>  
	/// Gets or sets the tiling type for the event.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(ContentMode)} == RhythmBase.RhythmDoctor.{nameof(ContentMode)}.{nameof(ContentMode.Tiled)}")]
	public TilingType TilingType { get; set; } = TilingType.Scroll;
	/// <summary>  
	/// Gets or sets the color for the foreground.  
	/// </summary>  
	[Tween]
	public PaletteColorWithAlpha Color { get; set; } = Global.Components.Color.White;
	/// <summary>  
	/// Gets or sets the list of images for the foreground.  
	/// </summary>  
	[JsonAlias("image")]
	public List<FileReference> Images { get; set; } = [];
	/// <summary>  
	/// Gets or sets the frames per second for the foreground animation.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(ContentMode)} == RhythmBase.RhythmDoctor.{nameof(ContentMode)}.{nameof(ContentMode.Tiled)}")]
	public float Fps { get; set; } = 30f;
	/// <summary>
	/// Gets or sets the horizontal scroll value.
	/// </summary>
	[Tween]
	[JsonCondition($"""
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
	/// Gets or sets the speed of the foreground scrolling when the content mode is set to tiled.
	/// </summary>
	[Tween]
	[JsonCondition($"""
		$&.{nameof(ContentMode)} == RhythmBase.RhythmDoctor.{nameof(ContentMode)}.{nameof(ContentMode.Tiled)}
		""")]
	public Point Speed { get; set; } = new(0, 0);
	/// <summary>  
	/// Gets or sets the duration of the event.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(ContentMode)} == RhythmBase.RhythmDoctor.{nameof(ContentMode)}.{nameof(ContentMode.Tiled)}")]
	public float Duration { get; set; } = 0;
	/// <summary>  
	/// Gets or sets the interval between frames.  
	/// </summary>  
	[JsonCondition($"""
		$&.{nameof(ContentMode)} == RhythmBase.RhythmDoctor.{nameof(ContentMode)}.{nameof(ContentMode.Tiled)} &&
		$&.{nameof(TilingType)} == RhythmBase.RhythmDoctor.{nameof(TilingType)}.{nameof(SetForeground.TilingType.Pulse)}
		""")]
	public float Interval
	{
		get => field;
		set => field = value > 0.01f ? value : 0.01f;
	}
	/// <summary>  
	/// Gets or sets the easing type for the event.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(ContentMode)} == RhythmBase.RhythmDoctor.{nameof(ContentMode)}.{nameof(ContentMode.Tiled)}")]
	public EaseType Ease { get; set; } = EaseType.Linear;
	/// <summary>  
	/// Gets the type of the event.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(ContentMode)} == RhythmBase.RhythmDoctor.{nameof(ContentMode)}.{nameof(ContentMode.Tiled)}")]
	public override EventType Type => EventType.SetForeground;

	/// <summary>  
	/// Gets the tab associated with the event.  
	/// </summary>  
	public override Tab Tab => Tab.Actions;
	IEnumerable<FileReference> IImageFileEvent.ImageFiles => [.. Images];
	IEnumerable<FileReference> IFileEvent.Files => [.. Images];

	/// <summary>  
	/// Returns a string that represents the current object.  
	/// </summary>  
	/// <returns>A string that represents the current object.</returns>  
	public override string ToString() => base.ToString() + $" {Color},{string.Join(',', Images.Select(i => i.ToString()))}";
}
