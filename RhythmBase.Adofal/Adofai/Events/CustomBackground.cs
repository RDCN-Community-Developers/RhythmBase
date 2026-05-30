using RhythmBase.Global.Components.Vector;

namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents a custom background event in the Adofai event system.  
/// </summary>  
[JsonObjectSerializable]
public class CustomBackground : BaseTaggedTileEvent, IImageFileEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.CustomBackground;
	/// <summary>  
	/// Gets or sets the color of the background.  
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
	[JsonCondition($"!$&.{nameof(BackgroundImage)}.IsEmpty")]
	public Color ImageColor { get; set; } = Color.White;
	/// <summary>  
	/// Gets or sets the parallax effect values for the background.  
	/// </summary>  
	[JsonCondition($"!string.IsNullOrEmpty($&.{nameof(BackgroundImage)}) || !$&.{nameof(LockRotation)}")]
	public Point Parallax { get; set; }
	/// <summary>  
	/// Gets or sets the display mode of the background image.  
	/// </summary>  
	[JsonAlias("bgDisplayMode")]
	[JsonCondition($"!string.IsNullOrEmpty($&.{nameof(BackgroundImage)})")]
	public BackgroundDisplayMode BackgroundDisplayMode { get; set; } = BackgroundDisplayMode.FitToScreen;
	/// <summary>  
	/// Gets or sets a value indicating whether image smoothing is enabled.  
	/// </summary>  
	public bool ImageSmoothing { get; set; } = true;
	/// <summary>  
	/// Gets or sets a value indicating whether the background rotation is locked.  
	/// </summary>  
	[JsonAlias("lockRot")]
	[JsonCondition($"!string.IsNullOrEmpty($&.{nameof(BackgroundImage)})")]
	public bool LockRotation { get; set; }
	/// <summary>  
	/// Gets or sets a value indicating whether the background image should loop.  
	/// </summary>  
	[JsonAlias("loopBG")]
	[JsonCondition($"!string.IsNullOrEmpty($&.{nameof(BackgroundImage)})")]
	public bool LoopBackground { get; set; }
	/// <summary>  
	/// Gets or sets the scaling ratio of the background image.  
	/// </summary>  
	[JsonCondition($"!string.IsNullOrEmpty($&.{nameof(BackgroundImage)})")]
	public float ScalingRatio { get; set; }
	IEnumerable<FileReference> IImageFileEvent.ImageFiles => [BackgroundImage];
	IEnumerable<FileReference> IFileEvent.Files => [BackgroundImage];
}
