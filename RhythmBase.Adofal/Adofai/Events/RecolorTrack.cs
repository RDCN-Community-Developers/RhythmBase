using RhythmBase.Adofai.Components;
using RhythmBase.Global.Components.Easing;
namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents an event to recolor a track in the Adofai level.  
/// </summary>  
[JsonObjectSerializable]
public class RecolorTrack : BaseTaggedTileEvent, IEaseEvent, IBeginningEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.RecolorTrack;
	/// <summary>  
	/// Gets or sets the starting tile reference for the recolor action.  
	/// </summary>  
	public TileReference StartTile { get; set; }
	/// <summary>  
	/// Gets or sets the ending tile reference for the recolor action.  
	/// </summary>  
	public TileReference EndTile { get; set; }
	/// <summary>  
	/// Gets or sets the gap length between tiles for the recolor action.  
	/// </summary>  
	public int GapLength { get; set; } = 0;
	/// <summary>  
	/// Gets or sets the duration of the recolor animation.  
	/// </summary>  
	public float Duration { get; set; } = 0;
	/// <summary>  
	/// Gets or sets the type of track color to apply.  
	/// </summary>  
	public TrackColorType TrackColorType { get; set; } = TrackColorType.Single;
	/// <summary>  
	/// Gets or sets the primary track color.  
	/// </summary>  
	public Color TrackColor { get; set; } = Color.FromRgba("debb7b");
	/// <summary>  
	/// Gets or sets the secondary track color.  
	/// </summary>  
	public Color SecondaryTrackColor { get; set; } = Color.White;
	/// <summary>  
	/// Gets or sets the duration of the track color animation.  
	/// </summary>  
	public float TrackColorAnimDuration { get; set; } = 2f;
	/// <summary>  
	/// Gets or sets the track color pulse type.  
	/// </summary>  
	public TrackColorPulse TrackColorPulse { get; set; } = TrackColorPulse.None;
	/// <summary>  
	/// Gets or sets the length of the track pulse.  
	/// </summary>  
	public int TrackPulseLength { get; set; } = 10;
	/// <summary>  
	/// Gets or sets the style of the track.  
	/// </summary>  
	public TrackStyle TrackStyle { get; set; } = TrackStyle.Standard;
	/// <summary>  
	/// Gets or sets the intensity of the track glow effect.  
	/// </summary>  
	public float TrackGlowIntensity { get; set; } = 100f;
	/// <summary>  
	/// Gets or sets the easing type for the recolor animation.  
	/// </summary>  
	public EaseType Ease { get; set; }
}
