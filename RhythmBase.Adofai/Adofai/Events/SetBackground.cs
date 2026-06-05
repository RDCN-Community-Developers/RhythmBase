using RhythmBase.Global.Components.Vector;

namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents an event to set the background properties in the Adofai event system.  
/// </summary>  
[JsonObjectSerializable]
public class SetBackground : BaseTaggedTileEvent, IBeginningEvent, IImageFileEvent
{
	/// <summary>  
	/// Gets the type of the event, which is <see cref="EventType.SetBackground"/>.  
	/// </summary>  
	public override EventType Type => EventType.SetBackground;

	/// <summary>  
	/// Gets or sets the background color.  
	/// </summary>  
	public Color Color { get; set; } = Color.Black;

	/// <summary>  
	/// Gets or sets the background image file path.  
	/// </summary>  
	[JsonAlias("bgImage")]
	public FileReference BackgroundImage { get; set; } = string.Empty;

	/// <summary>  
	/// Gets or sets the color applied to the background image.  
	/// </summary>  
	public Color ImageColor { get; set; } = Color.White;

	/// <summary>  
	/// Gets or sets the parallax effect values for the background.  
	/// </summary>  
	public PointN Parallax { get; set; } = new PointN(100, 100);

	/// <summary>  
	/// Gets or sets the display mode of the background image.  
	/// </summary>  
	public BgDisplayMode BgDisplayMode { get; set; }

	/// <summary>  
	/// Gets or sets a value indicating whether image smoothing is enabled.  
	/// </summary>  
	public bool ImageSmoothing { get; set; }

	/// <summary>  
	/// Gets or sets a value indicating whether the background rotation is locked.  
	/// </summary>  
	public bool LockRot { get; set; }

	/// <summary>  
	/// Gets or sets a value indicating whether the background image should loop.  
	/// </summary>  
	public bool LoopBG { get; set; }

	/// <summary>  
	/// Gets or sets the scaling ratio of the background image.  
	/// </summary>  
	public float ScalingRatio { get; set; } = 100f;
	IEnumerable<FileReference> IImageFileEvent.ImageFiles => [BackgroundImage];
	IEnumerable<FileReference> IFileEvent.Files => [BackgroundImage];
}
