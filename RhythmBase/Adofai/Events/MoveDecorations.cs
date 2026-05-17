using RhythmBase.Adofai.Components;
using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components.Vector;

namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents an event to move decorations in the Adofai editor.  
/// </summary>  
[RDJsonObjectSerializable]
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
	[RDJsonCondition($"$&.{nameof(PositionOffset)} is not null")]
	public RDPoint? PositionOffset { get; set; }
	/// <summary>  
	/// Gets or sets a value indicating whether the decoration is visible.  
	/// </summary>
	[RDJsonCondition($"$&.{nameof(Visible)} is not null")]
	public bool? Visible { get; set; }
	/// <summary>  
	/// Gets or sets the reference point for the decoration's position.  
	/// </summary>  
	[RDJsonCondition($"$&.{nameof(RelativeTo)} is not null")]
	public DecorationRelativeTo? RelativeTo { get; set; }
	/// <summary>  
	/// Gets or sets the image used for the decoration.  
	/// </summary>  
	[RDJsonCondition($"$&.{nameof(DecorationImage)} is not null")]
	public FileReference? DecorationImage { get; set; }
	/// <summary>  
	/// Gets or sets the pivot offset for the decoration.  
	/// </summary>  
	[RDJsonCondition($"$&.{nameof(PivotOffset)} is not null")]
	public RDSize? PivotOffset { get; set; }
	/// <summary>  
	/// Gets or sets the rotation offset for the decoration.  
	/// </summary>  
	[RDJsonCondition($"$&.{nameof(RotationOffset)} is not null")]
	public float? RotationOffset { get; set; }
	/// <summary>  
	/// Gets or sets the scale of the decoration.  
	/// </summary>  
	[RDJsonCondition($"$&.{nameof(Scale)} is not null")]
	public RDSize? Scale { get; set; }
	/// <summary>  
	/// Gets or sets the color of the decoration.  
	/// </summary>  
	[RDJsonCondition($"$&.{nameof(Color)} is not null")]
	public RDColor? Color { get; set; }
	/// <summary>  
	/// Gets or sets the opacity of the decoration.  
	/// </summary>
	[RDJsonCondition($"$&.{nameof(Opacity)} is not null")]
	public float? Opacity { get; set; }
	/// <summary>  
	/// Gets or sets the depth of the decoration.  
	/// </summary>  
	[RDJsonCondition($"$&.{nameof(Depth)} is not null")]
	public int? Depth { get; set; }
	/// <summary>  
	/// Gets or sets the parallax value for the decoration.  
	/// </summary>  
	[RDJsonCondition($"$&.{nameof(Parallax)} is not null")]
	public RDPoint? Parallax { get; set; }
	/// <summary>  
	/// Gets or sets the parallax offset for the decoration.  
	/// </summary> 
	[RDJsonCondition($"$&.{nameof(ParallaxOffset)} is not null")]
	public RDPoint? ParallaxOffset { get; set; }
	/// <summary>  
	/// Gets or sets the masking type for the decoration.  
	/// </summary>  
	[RDJsonCondition($"$&.{nameof(MaskingType)} is not null")]
	public MaskingType? MaskingType { get; set; }
	/// <summary>  
	/// Gets or sets a value indicating whether to use masking depth.  
	/// </summary>  
	[RDJsonCondition($"$&.{nameof(UseMaskingDepth)} is not null")]
	public bool? UseMaskingDepth { get; set; }
	/// <summary>  
	/// Gets or sets the front depth for masking.  
	/// </summary>  
	[RDJsonCondition($"$&.{nameof(MaskingFrontDepth)} is not null")]
	public int? MaskingFrontDepth { get; set; }
	/// <summary>  
	/// Gets or sets the back depth for masking.  
	/// </summary>  
	[RDJsonCondition($"$&.{nameof(MaskingBackDepth)} is not null")]
	public int? MaskingBackDepth { get; set; }
	IEnumerable<FileReference> IImageFileEvent.ImageFiles => DecorationImage is not null ? [DecorationImage.Value] : [];
	IEnumerable<FileReference> IFileEvent.Files => DecorationImage is not null ? [DecorationImage.Value] : [];
}
