using RhythmBase.Adofai.Components;

namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents the ColorTrack event in ADOFAI.  
/// </summary>  
[RDJsonObjectSerializable]
public class ColorTrack : BaseTileEvent, ISingleEvent, IImageFileEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.ColorTrack;
	/// <summary>  
	/// Gets or sets the type of track color.  
	/// </summary>  
	public TrackColorType TrackColorType { get; set; } = TrackColorType.Single;
	/// <summary>  
	/// Gets or sets the primary track color.  
	/// </summary>  
	public RDColor TrackColor { get; set; } = RDColor.FromRgba("debb7b");
	/// <summary>  
	/// Gets or sets the secondary track color.  
	/// </summary>  
	public RDColor SecondaryTrackColor { get; set; } = RDColor.White;
	/// <summary>  
	/// Gets or sets the duration of the track color animation.  
	/// </summary>  
	[RDJsonAlias("trackColorAnimDuration")]
	public float TrackColorAnimationDuration { get; set; } = 2;
	/// <summary>  
	/// Gets or sets the track color pulse type.  
	/// </summary>  
	public TrackColorPulse TrackColorPulse { get; set; } = TrackColorPulse.None;
	/// <summary>  
	/// Gets or sets the length of the track pulse.  
	/// </summary>  
	public float TrackPulseLength { get; set; } = 10;
	/// <summary>  
	/// Gets or sets the style of the track.  
	/// </summary>  
	public TrackStyle TrackStyle { get; set; } = TrackStyle.Standard;
	/// <summary>  
	/// Gets or sets the texture of the track.  
	/// </summary>  
	[RDJsonCondition($"$&.{nameof(TrackStyle)} is RhythmBase.Adofai.{nameof(Adofai.TrackStyle)}.{nameof(TrackStyle.Standard)}")]
	public FileReference TrackTexture { get; set; } = string.Empty;
	/// <summary>  
	/// Gets or sets the scale of the track texture.  
	/// </summary>  
	public float TrackTextureScale { get; set; } = 1f;
	/// <summary>  
	/// Gets or sets the intensity of the track glow.  
	/// </summary>  
	public float TrackGlowIntensity { get; set; } = 100f;
	/// <summary>
	/// Gets or sets a value indicating whether floor icon outlines are enabled.
	/// </summary>
	[RDJsonCondition($"$&.{nameof(FloorIconOutlines)} is not null")]
	public bool? FloorIconOutlines { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether only the current tile should be processed.
	/// </summary>
	public bool JustThisTile { get; set; } = false;
	IEnumerable<FileReference> IImageFileEvent.ImageFiles => [TrackTexture];
	IEnumerable<FileReference> IFileEvent.Files => [TrackTexture];
}
