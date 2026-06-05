using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components.Vector;

namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents an event to move decorations in the Adofai editor.  
/// </summary>  
[JsonObjectSerializable]
public class MoveDecorations : BaseTaggedTileEvent, IEaseEvent, IBeginningEvent, IImageFileEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.MoveDecorations;
	/// <summary>  
	/// Gets or sets the duration of the event.  
	/// </summary>  
	public float Duration { get; set; } = 1f;
	/// <summary>  
	/// Gets or sets the tag associated with the decoration.  
	/// </summary>  
	public string Tag { get; set; } = string.Empty;
	/// <summary>  
	/// Gets or sets the easing type for the event.  
	/// </summary>  
	public EaseType Ease { get; set; }
	/// <summary>  
	/// Gets or sets the position offset for the decoration.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(PositionOffset)} is not null")]
	public Point? PositionOffset { get; set; }
	/// <summary>  
	/// Gets or sets a value indicating whether the decoration is visible.  
	/// </summary>
	[JsonCondition($"$&.{nameof(Visible)} is not null")]
	public bool? Visible { get; set; }
	/// <summary>  
	/// Gets or sets the reference point for the decoration's position.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(RelativeTo)} is not null")]
	public DecorationRelativeTo? RelativeTo { get; set; }
	/// <summary>  
	/// Gets or sets the image used for the decoration.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(DecorationImage)} is not null")]
	public FileReference? DecorationImage { get; set; }
	/// <summary>  
	/// Gets or sets the pivot offset for the decoration.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(PivotOffset)} is not null")]
	public Size? PivotOffset { get; set; }
	/// <summary>  
	/// Gets or sets the rotation offset for the decoration.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(RotationOffset)} is not null")]
	public float? RotationOffset { get; set; }
	/// <summary>  
	/// Gets or sets the scale of the decoration.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(Scale)} is not null")]
	public Size? Scale { get; set; }
	/// <summary>  
	/// Gets or sets the color of the decoration.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(Color)} is not null")]
	public Color? Color { get; set; }
	/// <summary>  
	/// Gets or sets the opacity of the decoration.  
	/// </summary>
	[JsonCondition($"$&.{nameof(Opacity)} is not null")]
	public float? Opacity { get; set; }
	/// <summary>  
	/// Gets or sets the depth of the decoration.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(Depth)} is not null")]
	public int? Depth { get; set; }
	/// <summary>  
	/// Gets or sets the parallax value for the decoration.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(Parallax)} is not null")]
	public Point? Parallax { get; set; }
	/// <summary>  
	/// Gets or sets the parallax offset for the decoration.  
	/// </summary> 
	[JsonCondition($"$&.{nameof(ParallaxOffset)} is not null")]
	public Point? ParallaxOffset { get; set; }
	/// <summary>  
	/// Gets or sets the masking type for the decoration.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(MaskingType)} is not null")]
	public MaskingType? MaskingType { get; set; }
	/// <summary>  
	/// Gets or sets a value indicating whether to use masking depth.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(UseMaskingDepth)} is not null")]
	public bool? UseMaskingDepth { get; set; }
	/// <summary>  
	/// Gets or sets the front depth for masking.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(MaskingFrontDepth)} is not null")]
	public int? MaskingFrontDepth { get; set; }
	/// <summary>  
	/// Gets or sets the back depth for masking.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(MaskingBackDepth)} is not null")]
	public int? MaskingBackDepth { get; set; }
	IEnumerable<FileReference> IImageFileEvent.ImageFiles => DecorationImage is not null ? [DecorationImage.Value] : [];
	IEnumerable<FileReference> IFileEvent.Files => DecorationImage is not null ? [DecorationImage.Value] : [];
}
