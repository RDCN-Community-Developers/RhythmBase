using RhythmBase.Adofai.Components;
using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components.Vector;
namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents an event to move a track in the Adofai editor.  
/// </summary>  
[JsonObjectSerializable]
public class MoveTrack : BaseTaggedTileEvent, IEaseEvent, IBeginningEvent
{
	/// <inheritdoc/>  
	public override EventType Type => EventType.MoveTrack;
	/// <summary>  
	/// Gets or sets the starting tile for the track movement.  
	/// </summary>  
	public TileReference StartTile { get; set; }
	/// <summary>  
	/// Gets or sets the reference to the ending tile for the track movement.  
	/// </summary>  
	public TileReference EndTile { get; set; }
	/// <summary>  
	/// Gets or sets the gap length between tiles.  
	/// </summary>  
	public int GapLength { get; set; }
	/// <summary>  
	/// Gets or sets the duration of the track movement.  
	/// </summary>  
	public float Duration { get; set; }
	/// <summary>  
	/// Gets or sets the position offset for the track movement.  
	/// </summary> 
	[JsonCondition($"$&.{nameof(PositionOffset)} is not null")]
	public Point? PositionOffset { get; set; }
	/// <summary>
	/// Gets or sets the rotation offset value.
	/// </summary>
	[JsonCondition($"$&.{nameof(RotationOffset)} is not null")]
	public float? RotationOffset { get; set; }
	/// <summary>
	/// Gets or sets the scale factor for resizing operations.
	/// </summary>
	[JsonCondition($"$&.{nameof(Scale)} is not null")]
	public Size? Scale { get; set; }
	/// <summary>
	/// Gets or sets the opacity level of the element.
	/// </summary>
	[JsonCondition($"$&.{nameof(Opacity)} is not null")]
	public float? Opacity { get; set; }
	/// <summary>  
	/// Gets or sets the easing type for the track movement.  
	/// </summary>  
	public EaseType Ease { get; set; }
	/// <summary>  
	/// Gets or sets a value indicating whether the movement should only affect the maximum visual effects.  
	/// </summary>  
	public bool MaxVfxOnly { get; set; }
}
