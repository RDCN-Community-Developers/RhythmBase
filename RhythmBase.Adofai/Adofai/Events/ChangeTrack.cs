namespace RhythmBase.Adofai.Events;

/// <summary>
/// Represents an event that changes the visual and animation properties of the track.
/// </summary>
[JsonObjectSerializable]
public class ChangeTrack : BaseTileEvent
{
	/// <summary>
	/// Gets the concrete event type identifier for serialization and dispatch.
	/// </summary>
	public override EventType Type => EventType.ChangeTrack;

	/// <summary>
	/// Gets or sets the color application mode for the track.
	/// </summary>
	public TrackColorType TrackColorType { get; set; } = TrackColorType.Single;

	/// <summary>
	/// Gets or sets the primary color applied to the track.
	/// </summary>
	public Color TrackColor { get; set; } = new(0xffdebb7b);

	/// <summary>
	/// Gets or sets the secondary color used when color mode requires two colors.
	/// </summary>
	public Color SecondaryTrackColor { get; set; } = Color.White;

	/// <summary>
	/// Gets or sets the duration of the color animation in beats.
	/// </summary>
	public float TrackColorAnimDuration { get; set; } = 2;

	/// <summary>
	/// Gets or sets the pulse behavior applied to the track colors.
	/// </summary>
	public TrackColorPulse TrackColorPulse { get; set; } = TrackColorPulse.None;

	/// <summary>
	/// Gets or sets the number of beats the color pulse lasts.
	/// </summary>
	public int TrackPulseLength { get; set; } = 10;

	/// <summary>
	/// Gets or sets the visual style of the track surface.
	/// </summary>
	public TrackStyle TrackStyle { get; set; } = TrackStyle.Standard;

	/// <summary>
	/// Gets or sets the animation that plays as the track appears.
	/// </summary>
	public TrackAnimationType TrackAnimation { get; set; } = TrackAnimationType.None;

	/// <summary>
	/// Gets or sets how many beats ahead of the player the animation starts.
	/// </summary>
	public float BeatsAhead { get; set; } = 3;

	/// <summary>
	/// Gets or sets the animation used when the track disappears.
	/// </summary>
	public TrackDisappearAnimationType TrackDisappearAnimation { get; set; } = TrackDisappearAnimationType.None;

	/// <summary>
	/// Gets or sets how many beats behind the player the disappear animation lingers.
	/// </summary>
	public float BeatsBehind { get; set; } = 4;
}
