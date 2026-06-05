using RhythmBase.Global.Components.Easing;
namespace RhythmBase.Adofai.Events;

/// <summary>
/// Represents an event to set object properties in the Adofai editor.
/// </summary>
[JsonObjectSerializable]
public class SetObject : BaseTaggedTileEvent, IEaseEvent, IBeginningEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.SetObject;
	/// <summary>
	/// Gets or sets the duration of the event.
	/// </summary>
	public float Duration { get; set; } = 1f;
	/// <summary>
	/// Gets or sets the tag associated with the object.
	/// </summary>
	public string Tag { get; set; } = string.Empty;
	/// <summary>
	/// Gets or sets the easing type for the event.
	/// </summary>
	public EaseType Ease { get; set; }
	/// <summary>
	/// Gets or sets the color of the planet.
	/// </summary>
	[JsonCondition($"$&.{nameof(PlanetColor)} is not null")]
	public Color? PlanetColor { get; set; }
	/// <summary>
	/// Gets or sets the color of the planet's tail.
	/// </summary>
	[JsonCondition($"$&.{nameof(PlanetTailColor)} is not null")]
	public Color? PlanetTailColor { get; set; }
	/// <summary>
	/// Gets or sets the angle of the track.
	/// </summary>
	[JsonCondition($"$&.{nameof(TrackAngle)} is not null")]
	public float? TrackAngle { get; set; }
	/// <summary>
	/// Gets or sets the type of the track color.
	/// </summary>
	[JsonCondition($"$&.{nameof(TrackColorType)} is not null")]
	public TrackColorType? TrackColorType { get; set; }
	/// <summary>
	/// Gets or sets the primary color of the track.
	/// </summary>
	[JsonCondition($"$&.{nameof(TrackColor)} is not null")]
	public Color? TrackColor { get; set; } 
	/// <summary>
	/// Gets or sets the secondary color of the track.
	/// </summary>
	[JsonCondition($"$&.{nameof(SecondaryTrackColor)} is not null")]
	public Color? SecondaryTrackColor { get; set; }
	/// <summary>
	/// Gets or sets the duration of the track color animation.
	/// </summary>
	[JsonCondition($"$&.{nameof(TrackColorAnimDuration)} is not null")]
	public float? TrackColorAnimDuration { get; set; }
	/// <summary>
	/// Gets or sets the opacity of the track.
	/// </summary>
	[JsonCondition($"$&.{nameof(TrackOpacity)} is not null")]
	public float? TrackOpacity { get; set; } 
	/// <summary>
	/// Gets or sets the style of the track.
	/// </summary>
	[JsonCondition($"$&.{nameof(TrackStyle)} is not null")]
	public TrackStyle? TrackStyle { get; set; }
	/// <summary>
	/// Gets or sets the icon of the track.
	/// </summary>
	[JsonCondition($"$&.{nameof(TrackIcon)} is not null")]
	public TrackIcon? TrackIcon { get; set; }
	/// <summary>
	/// Gets or sets the angle of the track icon.
	/// </summary>
	[JsonCondition($"$&.{nameof(TrackIconAngle)} is not null")]
	public float? TrackIconAngle { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether the track has a red swirl.
	/// </summary>
	[JsonCondition($"$&.{nameof(TrackIconFlipped)} is not null")]
	public bool? TrackIconFlipped { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether the red swirl should be tracked.
	/// </summary>
	[JsonCondition($"$&.{nameof(TrackRedSwirl)} is not null")]
	public bool? TrackRedSwirl { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether the track has a gray set speed icon.
	/// </summary>
	[JsonCondition($"$&.{nameof(TrackGraySetSpeedIcon)} is not null")]
	public bool? TrackGraySetSpeedIcon { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether the track glow is enabled.
	/// </summary>
	[JsonCondition($"$&.{nameof(TrackGlowEnabled)} is not null")]
	public bool? TrackGlowEnabled { get; set; }
	/// <summary>
	/// Gets or sets the color of the track glow.
	/// </summary>
	[JsonCondition($"$&.{nameof(TrackGlowColor)} is not null")]
	public Color? TrackGlowColor { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether icon outlines should be tracked.
	/// </summary>
	[JsonCondition($"$&.{nameof(TrackIconOutlines)} is not null")]
	public bool? TrackIconOutlines { get; set; }
}