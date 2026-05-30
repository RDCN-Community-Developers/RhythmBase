using RhythmBase.Global.Components.Vector;

namespace RhythmBase.Adofai.Events;
/// <summary>  
/// Represents an event to add a decoration to a tile in the game.  
/// </summary>  
[JsonObjectSerializable]
public class AddDecoration : BaseTileEvent, IBeginningEvent, IImageFileEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.AddDecoration;
	/// <summary>  
	/// Gets or sets the image of the decoration.  
	/// </summary>  
	public FileReference DecorationImage { get; set; } = "";
	/// <summary>  
	/// Gets or sets the position of the decoration.  
	/// </summary>  
	public PointN Position { get; set; } = new(0, 0);
	/// <summary>  
	/// Gets or sets the reference point for the decoration's position.  
	/// </summary>  
	public DecorationRelativeTo RelativeTo { get; set; } = DecorationRelativeTo.Global;
	/// <summary>
	/// Gets or sets a value indicating whether the object should adhere to the floor surface.
	/// </summary>
	public bool StickToFloor { get; set; } = false;
	/// <summary>  
	/// Gets or sets the pivot offset of the decoration.  
	/// </summary>  
	public SizeN PivotOffset { get; set; } = new(0, 0);
	/// <summary>  
	/// Gets or sets the rotation of the decoration in degrees.  
	/// </summary>  
	public float Rotation { get; set; }
	/// <summary>  
	/// Gets or sets a value indicating whether the rotation is locked.  
	/// </summary>  
	[JsonCondition($"!$&.{nameof(StickToFloor)}")]
	public bool LockRotation { get; set; } = false;
	/// <summary>  
	/// Gets or sets the scale of the decoration.  
	/// </summary>  
	public SizeN Scale { get; set; } = new(100, 100);
	/// <summary>  
	/// Gets or sets a value indicating whether the scale is locked.  
	/// </summary>  
	[JsonCondition($"!$&.{nameof(StickToFloor)}")]
	public bool LockScale { get; set; }
	/// <summary>  
	/// Gets or sets the tile size of the decoration.  
	/// </summary>  
	[JsonCondition($"""
		$&.{nameof(BlendMode)} is RhythmBase.Adofai.{nameof(Adofai.BlendMode)}.{nameof(BlendMode.None)} &&
		$&.{nameof(MaskingType)} is not RhythmBase.Adofai.{nameof(Adofai.MaskingType)}.{nameof(MaskingType.Mask)}
		""")]
	public SizeN Tile { get; set; } = new(1, 1);
	/// <summary>  
	/// Gets or sets the color of the decoration.  
	/// </summary>  
	public Color Color { get; set; } = Color.White;
	/// <summary>  
	/// Gets or sets the opacity of the decoration.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(MaskingType)} is not RhythmBase.Adofai.{nameof(Adofai.MaskingType)}.{nameof(MaskingType.Mask)}")]
	public float Opacity { get; set; } = 100f;
	/// <summary>  
	/// Gets or sets the depth of the decoration.  
	/// </summary>  
	[JsonCondition($"""
		$&.{nameof(MaskingType)} is not RhythmBase.Adofai.{nameof(Adofai.MaskingType)}.{nameof(MaskingType.Mask)} &&
		!$&.{nameof(SyncFloorDepth)}
		""")]
	public int Depth { get; set; } = -1;
	/// <summary>
	/// Gets or sets a value indicating whether the floor depth synchronization is enabled.
	/// </summary>
	public bool SyncFloorDepth { get; set; }
	/// <summary>  
	/// Gets or sets the parallax effect of the decoration.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(RelativeTo)} is not RhythmBase.Adofai.{nameof(DecorationRelativeTo)}.{nameof(DecorationRelativeTo.Camera)}")]
	public SizeN Parallax { get; set; } = new(0, 0);
	/// <summary>  
	/// Gets or sets the parallax offset of the decoration.  
	/// </summary>  
	public SizeN ParallaxOffset { get; set; } = new(0, 0);
	/// <summary>  
	/// Gets or sets the tag associated with the decoration.  
	/// </summary>  
	public string Tag { get; set; } = string.Empty;
	/// <summary>  
	/// Gets or sets a value indicating whether image smoothing is applied.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(MaskingType)} is not RhythmBase.Adofai.{nameof(Adofai.MaskingType)}.{nameof(MaskingType.Mask)}")]
	public bool ImageSmoothing { get; set; } = true;
	/// <summary>  
	/// Gets or sets the blend mode of the decoration.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(MaskingType)} is not RhythmBase.Adofai.{nameof(Adofai.MaskingType)}.{nameof(MaskingType.Mask)}")]
	public BlendMode BlendMode { get; set; } = BlendMode.None;
	/// <summary>  
	/// Gets or sets the masking type of the decoration.  
	/// </summary>  
	public MaskingType MaskingType { get; set; } = MaskingType.None;
	/// <summary>
	/// Gets or sets the target string to be masked.
	/// </summary>
	public FileReference MaskingTarget { get; set; } = string.Empty;
	/// <summary>  
	/// Gets or sets a value indicating whether masking depth is used.  
	/// </summary>  
	public bool UseMaskingDepth { get; set; } = false;
	/// <summary>  
	/// Gets or sets the front depth for masking.  
	/// </summary>  
	public int MaskingFrontDepth { get; set; } = -1;
	/// <summary>  
	/// Gets or sets the back depth for masking.  
	/// </summary>  
	public int MaskingBackDepth { get; set; } = -1;
	/// <summary>  
	/// Gets or sets the type of hitbox for the decoration.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(MaskingType)} is not RhythmBase.Adofai.{nameof(Adofai.MaskingType)}.{nameof(MaskingType.Mask)}")]
	public HitboxTypes Hitbox { get; set; } = HitboxTypes.None;
	/// <summary>
	/// Gets or sets the type of trigger for the hitbox.
	/// </summary>
	public HitboxTriggerType HitboxTriggerType { get; set; } = HitboxTriggerType.Once;
	/// <summary>
	/// Gets or sets the interval, in milliseconds, at which the hitbox is repeatedly activated.
	/// </summary>
	public float HitboxRepeatInterval { get; set; } = 1000f;
	/// <summary>  
	/// Gets or sets the event tag associated with the hitbox.  
	/// </summary>  
	public string HitboxEventTag { get; set; } = string.Empty;
	/// <summary>
	/// Gets or sets the target for hitbox detection.
	/// </summary>
	[JsonCondition($"$&.{nameof(Hitbox)} is not RhythmBase.Adofai.{nameof(HitboxTypes)}.{nameof(HitboxTypes.None)}")]
	public HitboxDetectTarget HitboxDetectTarget { get; set; } = HitboxDetectTarget.Planet;
	/// <summary>
	/// Gets or sets the target planet for hitbox detection.
	/// </summary>
	public HitboxTargetPlanet HitboxTargetPlanet { get; set; } = HitboxTargetPlanet.Any;
	/// <summary>
	/// Gets or sets the decorative tag associated with the hitbox.
	/// </summary>
	public string HitboxDecoTag { get; set; } = string.Empty;
	/// <summary>  
	/// Gets or sets the type of fail hitbox for the decoration.  
	/// </summary>  
	public FailHitboxTypes FailHitboxType { get; set; }
	/// <summary>  
	/// Gets or sets the scale of the fail hitbox.  
	/// </summary>  
	public SizeN FailHitboxScale { get; set; } = new(100, 100);
	/// <summary>  
	/// Gets or sets the offset of the fail hitbox.  
	/// </summary>  
	public SizeN FailHitboxOffset { get; set; } = new(0, 0);
	/// <summary>  
	/// Gets or sets the rotation of the fail hitbox in degrees.  
	/// </summary>  
	public float FailHitboxRotation { get; set; } = 0f;
	/// <summary>  
	/// Gets or sets the components associated with the decoration.  
	/// </summary>  
	public string Components { get; set; } = string.Empty;
	IEnumerable<FileReference> IImageFileEvent.ImageFiles => [DecorationImage, MaskingTarget];
	IEnumerable<FileReference> IFileEvent.Files => [DecorationImage, MaskingTarget];
}