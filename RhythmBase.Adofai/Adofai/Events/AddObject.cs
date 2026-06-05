using RhythmBase.Global.Components.Vector;
namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents an event to add an object in the game.  
/// </summary>  
[JsonObjectSerializable]
public class AddObject : BaseTileEvent, IBeginningEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.AddObject;
	/// <summary>  
	/// Gets or sets the type of the object to be added.  
	/// </summary>  
	public ObjectType ObjectType { get; set; } = ObjectType.Floor;
	/// <summary>  
	/// Gets or sets the type of the planet's color.  
	/// </summary>  
	public PlanetColorType PlanetColorType { get; set; } = PlanetColorType.DefaultRed;
	/// <summary>  
	/// Gets or sets the color of the planet.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(PlanetColorType)} is RhythmBase.Adofai.{nameof(Adofai.PlanetColorType)}.{nameof(PlanetColorType.Custom)}")]
	public Color PlanetColor { get; set; } = Color.Red;
	/// <summary>  
	/// Gets or sets the tail color of the planet.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(PlanetColorType)} is RhythmBase.Adofai.{nameof(Adofai.PlanetColorType)}.{nameof(PlanetColorType.Custom)}")]
	public Color PlanetTailColor { get; set; } = Color.Red;
	/// <summary>  
	/// Gets or sets the type of the track.  
	/// </summary>  
	public TrackType TrackType { get; set; } = TrackType.Normal;
	/// <summary>  
	/// Gets or sets the angle of the track.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(TrackType)} is not RhythmBase.Adofai.{nameof(Adofai.TrackType)}.{nameof(TrackType.Midspin)}")]
	public float TrackAngle { get; set; }
	/// <summary>  
	/// Gets or sets the type of the track's color.  
	/// </summary>  
	public TrackColorType TrackColorType { get; set; }
	/// <summary>  
	/// Gets or sets the primary color of the track.  
	/// </summary>  
	public Color TrackColor { get; set; } = Color.FromRgba("debb7b");
	/// <summary>  
	/// Gets or sets the secondary color of the track.  
	/// </summary>  
	public Color SecondaryTrackColor { get; set; } = Color.White;
	/// <summary>  
	/// Gets or sets the duration of the track color animation.  
	/// </summary>  
	public float TrackColorAnimDuration { get; set; } = 2f;
	/// <summary>  
	/// Gets or sets the opacity of the track.  
	/// </summary>  
	public float TrackOpacity { get; set; } = 100f;
	/// <summary>  
	/// Gets or sets the style of the track.  
	/// </summary>  
	public TrackStyle TrackStyle { get; set; } = TrackStyle.Standard;
	/// <summary>  
	/// Gets or sets the icon of the track.  
	/// </summary>  
	public TrackIconType TrackIcon { get; set; } = TrackIconType.None;
	/// <summary>  
	/// Gets or sets the angle of the track icon.  
	/// </summary>  
	public float TrackIconAngle { get; set; } = 0;
	/// <summary>
	/// Gets or sets a value indicating whether the track icon is flipped.
	/// </summary>
	public bool TrackIconFlipped { get; set; } = false;
	/// <summary>  
	/// Gets or sets a value indicating whether the track has a red swirl.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(TrackIcon)} is RhythmBase.Adofai.{nameof(TrackIconType)}.{nameof(TrackIconType.Swirl)}")]
	public bool TrackRedSwirl { get; set; } = false;
	/// <summary>  
	/// Gets or sets a value indicating whether the track has a gray set speed icon.  
	/// </summary>  
	[JsonCondition($"""
		$&.{nameof(TrackIcon)}
		is RhythmBase.Adofai.{nameof(TrackIconType)}.{nameof(TrackIconType.Snail)}
		or RhythmBase.Adofai.{nameof(TrackIconType)}.{nameof(TrackIconType.DoubleSnail)}
		or RhythmBase.Adofai.{nameof(TrackIconType)}.{nameof(TrackIconType.Rabbit)}
		or RhythmBase.Adofai.{nameof(TrackIconType)}.{nameof(TrackIconType.DoubleRabbit)}
		""")]
	public bool TrackGraySetSpeedIcon { get; set; } = false;
	/// <summary>  
	/// Gets or sets the BPM value for the track's set speed icon.  
	/// </summary>  
	[JsonCondition($"""
		$&.{nameof(TrackIcon)}
		is RhythmBase.Adofai.{nameof(TrackIconType)}.{nameof(TrackIconType.Snail)}
		or RhythmBase.Adofai.{nameof(TrackIconType)}.{nameof(TrackIconType.DoubleSnail)}
		or RhythmBase.Adofai.{nameof(TrackIconType)}.{nameof(TrackIconType.Rabbit)}
		or RhythmBase.Adofai.{nameof(TrackIconType)}.{nameof(TrackIconType.DoubleRabbit)}
		""")]
	public float TrackSetSpeedIconBpm { get; set; } = 100f;
	/// <summary>  
	/// Gets or sets a value indicating whether the track glow is enabled.  
	/// </summary>  
	public bool TrackGlowEnabled { get; set; } = false;
	/// <summary>  
	/// Gets or sets the glow color of the track.  
	/// </summary>  
	public Color TrackGlowColor { get; set; } = Color.White;
	/// <summary>
	/// Gets or sets a value indicating whether icon outlines should be tracked.
	/// </summary>
	public bool TrackIconOutlines { get; set; } = false;
	/// <summary>  
	/// Gets or sets the position of the object.  
	/// </summary>  
	public PointN Position { get; set; } = new(0, 0);
	/// <summary>  
	/// Gets or sets the relative position of the camera.  
	/// </summary>  
	public ObjectRelativeTo RelativeTo { get; set; } = ObjectRelativeTo.Global;
	/// <summary>  
	/// Gets or sets the pivot offset of the object.  
	/// </summary>  
	public PointN PivotOffset { get; set; } = new(0, 0);
	/// <summary>  
	/// Gets or sets the rotation of the object.  
	/// </summary>  
	public float Rotation { get; set; } = 0f;
	/// <summary>  
	/// Gets or sets a value indicating whether the rotation is locked.  
	/// </summary>  
	public bool LockRotation { get; set; } = false;
	/// <summary>  
	/// Gets or sets the scale of the object.  
	/// </summary>  
	public SizeN Scale { get; set; } = new(100,100);
	/// <summary>  
	/// Gets or sets a value indicating whether the scale is locked.  
	/// </summary>  
	public bool LockScale { get; set; } = false;
	/// <summary>  
	/// Gets or sets the depth of the object.  
	/// </summary>  
	[JsonCondition($"!$&.{nameof(SyncFloorDepth)}")]
	public int Depth { get; set; } = -1;
	/// <summary>
	/// Gets or sets a value indicating whether the floor depth synchronization is enabled.
	/// </summary>
	public bool SyncFloorDepth { get; set; } = false;
	/// <summary>  
	/// Gets or sets the parallax effect of the object.  
	/// </summary>  
	public SizeN Parallax { get; set; } = new(0, 0);
	/// <summary>  
	/// Gets or sets the parallax offset of the object.  
	/// </summary>  
	public SizeN ParallaxOffset { get; set; } = new(0, 0);
	/// <summary>  
	/// Gets or sets the tag associated with the object.  
	/// </summary>  
	public string Tag { get; set; } = string.Empty;
}