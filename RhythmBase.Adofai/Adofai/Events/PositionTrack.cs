using RhythmBase.Adofai.Components;
using RhythmBase.Global.Components.Vector;
namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents an event that adjusts the position, rotation, scale, and opacity of a track in the level.  
/// </summary>  
[JsonObjectSerializable]
public class PositionTrack : BaseTileEvent
{
	/// <inheritdoc/>  
	public override EventType Type => EventType.PositionTrack;
	/// <summary>  
	/// Gets or sets the position offset of the track.  
	/// </summary> 
	[JsonCondition($"$&.{nameof(PositionOffset)} is not null")]
	public Point? PositionOffset { get; set; }
	/// <summary>  
	/// Gets or sets the reference tile relative to which the position is calculated.  
	/// </summary>  
	public TileReference RelativeTo { get; set; }
	/// <summary>  
	/// Gets or sets the rotation of the track in degrees.  
	/// </summary>
	[JsonCondition($"$&.{nameof(Rotation)} is not null")]
	public float? Rotation { get; set; }
	/// <summary>  
	/// Gets or sets the scale of the track.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(Scale)} is not null")]
	public float? Scale { get; set; }
	/// <summary>  
	/// Gets or sets the opacity of the track.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(Opacity)} is not null")]
	public float? Opacity { get; set; }
	/// <summary>  
	/// Gets or sets a value indicating whether the event applies only to the current tile.  
	/// </summary>  
	public bool JustThisTile { get; set; } = false;
	/// <summary>  
	/// Gets or sets a value indicating whether the event is editor-only.  
	/// </summary>  
	public bool EditorOnly { get; set; } = false;
	/// <summary>  
	/// Gets or sets a value indicating whether the track should stick to floors.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(StickToFloors)} is not null")]
	public bool? StickToFloors { get; set; }
}
