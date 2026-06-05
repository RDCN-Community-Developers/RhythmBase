using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components.Vector;
namespace RhythmBase.Adofai.Components;

	/// <summary>
	/// Represents the settings for an ADOFAI (A Dance of Fire and Ice) level.
	/// </summary>
	public class Settings
	{
		/// <summary>
		/// Gets or sets the version of the level.
		/// </summary>
		public int Version { get; set; } = DefaultVersionAdofai;
		/// <summary>
		/// Gets or sets the name of the artist of the song.
		/// </summary>
		public string Artist { get; set; } = string.Empty;
		/// <summary>
		/// Gets or sets the special type of the artist.
		/// </summary>
		public SpecialArtistTypes SpecialArtistType { get; set; } = SpecialArtistTypes.None;
		/// <summary>
		/// Gets or sets the permission details for the artist's work.
		/// </summary>
		public string ArtistPermission { get; set; } = string.Empty;
		/// <summary>
		/// Gets or sets the name of the song.
		/// </summary>
		public string Song { get; set; } = string.Empty;
		/// <summary>
		/// Gets or sets the author of the level.
		/// </summary>
		public string Author { get; set; } = string.Empty;
		/// <summary>
		/// Gets or sets a value indicating whether the countdown time is separate.
		/// </summary>
		public bool SeparateCountdownTime { get; set; } = true;
		/// <summary>
		/// Gets or sets the preview image for the level.
		/// </summary>
		public string PreviewImage { get; set; } = string.Empty;
		/// <summary>
		/// Gets or sets the preview icon for the level.
		/// </summary>
		public string PreviewIcon { get; set; } = string.Empty;
		/// <summary>
		/// Gets or sets the color of the preview icon.
		/// </summary>
		public Color PreviewIconColor { get; set; } = Color.FromRgba("003f52");
		/// <summary>
		/// Gets or sets the start time of the preview song in seconds.
		/// </summary>
		public float PreviewSongStart { get; set; } = 0;
		/// <summary>
		/// Gets or sets the duration of the preview song in seconds.
		/// </summary>
		public float PreviewSongDuration { get; set; } = 10;
		/// <summary>
		/// Gets or sets a value indicating whether the level has a seizure warning.
		/// </summary>
		public bool SeizureWarning { get; set; } = false;
		/// <summary>
		/// Gets or sets the description of the level.
		/// </summary>
		public string LevelDesc { get; set; } = string.Empty;
		/// <summary>
		/// Gets or sets the tags associated with the level.
		/// </summary>
		public string LevelTags { get; set; } = string.Empty;
		/// <summary>
		/// Gets or sets the links to the artist's work or profile.
		/// </summary>
		public string ArtistLinks { get; set; } = string.Empty;
		/// <summary>
		/// Gets or sets the target speed for the trial.
		/// </summary>
		public float SpeedTrialAim { get; set; } = 0;
		/// <summary>
		/// Gets or sets the difficulty level of the level.
		/// </summary>
		public int Difficulty { get; set; } = 1;
		/// <summary>
		/// Gets or sets the list of required mods for the level.
		/// </summary>
		public List<string> RequiredMods { get; set; } = [];
		/// <summary>
		/// Gets or sets the filename of the song.
		/// </summary>
		public string SongFilename { get; set; } = string.Empty;
		/// <summary>
		/// Gets or sets the beats per minute (BPM) of the song.
		/// </summary>
		public float Bpm { get; set; } = 100;
		/// <summary>
		/// Gets or sets the volume of the song.
		/// </summary>
		public float Volume { get; set; } = 100;
		/// <summary>
		/// Gets or sets the offset of the song in seconds.
		/// </summary>
		public float Offset { get; set; } = 0;
		/// <summary> 
		/// Gets or sets the pitch of the song.
		/// </summary>
		public float Pitch { get; set; } = 100;
		/// <summary>
		/// Gets or sets the hitsound used in the level.
		/// </summary>
		public string Hitsound { get; set; } = "Kick";
		/// <summary>
		/// Gets or sets the volume of the hitsound.
		/// </summary>
		public float HitsoundVolume { get; set; } = 100;
		/// <summary>
		/// Gets or sets the number of countdown ticks before the level starts.
		/// </summary>
		public float CountdownTicks { get; set; } = 4;
		/// <summary>
		/// Gets or sets the shape of the tile.
		/// </summary>
		public TileShape TileShape { get; set; } = TileShape.Long;
		/// <summary>
		/// Gets or sets the type of track color used in the level.
		/// </summary>
		public TrackColorType TrackColorType { get; set; } = TrackColorType.Single;
		/// <summary>
		/// Gets or sets the primary track color.
		/// </summary>
		public Color TrackColor { get; set; } = Color.FromRgba("debb7b");
		/// <summary>
		/// Gets or sets the secondary track color.
		/// </summary>
		public Color SecondaryTrackColor { get; set; } = Color.FromRgba("ffffff");
		/// <summary>
		/// Gets or sets the duration of the track color animation in seconds.
		/// </summary>
		public float TrackColorAnimDuration { get; set; } = 2;
		/// <summary>
		/// Gets or sets the type of track color pulse.
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
		public string TrackTexture { get; set; } = string.Empty;
		/// <summary>  
		/// Gets or sets the scale of the track texture.  
		/// </summary>  
		public int TrackTextureScale { get; set; } = 1;
		/// <summary>  
		/// Gets or sets the intensity of the track glow effect.  
		/// </summary>  
		public int TrackGlowIntensity { get; set; } = 100;
		/// <summary>
		/// Gets or sets the type of track animation.
		/// </summary>
		public TrackAnimationType TrackAnimation { get; set; } = TrackAnimationType.None;
		/// <summary>
		/// Gets or sets the number of beats ahead for the track.
		/// </summary>
		public int BeatsAhead { get; set; } = 3;
		/// <summary>
		/// Gets or sets the type of track disappear animation.
		/// </summary>
		public TrackDisappearAnimationType TrackDisappearAnimation { get; set; } = TrackDisappearAnimationType.None;
		/// <summary>
		/// Gets or sets the number of beats behind for the track.
		/// </summary>
		public int BeatsBehind { get; set; } = 4;
		/// <summary>
		/// Gets or sets the background color of the level.
		/// </summary>
		public Color BackgroundColor { get; set; } = Color.FromRgba("000000");
		/// <summary>
		/// Gets or sets a value indicating whether to show the default background if no image is provided.
		/// </summary>
		public bool ShowDefaultBGIfNoImage { get; set; } = true;
		/// <summary>  
		/// Gets or sets a value indicating whether to show the default background tile.  
		/// </summary>  
		public bool ShowDefaultBGTile { get; set; } = true;
		/// <summary>  
		/// Gets or sets the color of the default background tile.  
		/// </summary>  
		public Color DefaultBGTileColor { get; set; } = Color.FromRgba("101121");
		/// <summary>  
		/// Gets or sets the shape type of the default background tile.  
		/// </summary>  
		public DefaultBGTileShapeType DefaultBGTileShapeType { get; set; } = DefaultBGTileShapeType.Default;
		/// <summary>  
		/// Gets or sets the color of the default background shape.  
		/// </summary>  
		public Color DefaultBGShapeColor { get; set; } = Color.FromRgba("ffffff");
		/// <summary>
		/// Gets or sets the background image for the level.
		/// </summary>
		public string BgImage { get; set; } = string.Empty;
		/// <summary>
		/// Gets or sets the color of the background image.
		/// </summary>
		public Color BgImageColor { get; set; } = Color.FromRgba("ffffff");
		/// <summary>
		/// Gets or sets the parallax effect for the background.
		/// </summary>
		public PointNI Parallax { get; set; } = new PointNI(100, 100);
		/// <summary>
		/// Gets or sets the display mode of the background.
		/// </summary>
		public BgDisplayMode BgDisplayMode { get; set; } = BgDisplayMode.FitToScreen;
		/// <summary>
		/// Gets or sets a value indicating whether image smoothing is enabled.
		/// </summary>
		public bool ImageSmoothing { get; set; } = false;
		/// <summary>
		/// Gets or sets a value indicating whether the rotation is locked.
		/// </summary>
		public bool LockRot { get; set; } = false;
		/// <summary>
		/// Gets or sets a value indicating whether the background loops.
		/// </summary>
		public bool LoopBG { get; set; } = false;
		/// <summary>
		/// Gets or sets the scaling ratio for the background.
		/// </summary>
		public float ScalingRatio { get; set; } = 100;
		/// <summary>
		/// Gets or sets the reference point for the camera.
		/// </summary>
		public CameraRelativeTo RelativeTo { get; set; } = CameraRelativeTo.Player;
		/// <summary>
		/// Gets or sets the position of the camera.
		/// </summary>
		public PointNI Position { get; set; } = new PointNI(0, 0);
		/// <summary>
		/// Gets or sets the rotation of the camera.
		/// </summary>
		public float Rotation { get; set; } = 0;
		/// <summary>
		/// Gets or sets the zoom level of the camera.
		/// </summary>
		public float Zoom { get; set; } = 100;
		/// <summary>
		/// Gets or sets a value indicating whether the pulse signal is active on the floor.
		/// </summary>
		public bool PulseOnFloor { get; set; } = false;
		/// <summary>
		/// Gets or sets the background video for the level.
		/// </summary>
		public string BgVideo { get; set; } = string.Empty;
		/// <summary>
		/// Gets or sets a value indicating whether the video loops.
		/// </summary>
		public bool LoopVideo { get; set; } = false;
		/// <summary>
		/// Gets or sets the offset of the video in milliseconds.
		/// </summary>
		public int VidOffset { get; set; } = 0;
		/// <summary>
		/// Gets or sets a value indicating whether floor icons have outlines.
		/// </summary>
		public bool FloorIconOutlines { get; set; } = false;
		/// <summary>
		/// Gets or sets a value indicating whether the camera sticks to floors.
		/// </summary>
		public bool StickToFloors { get; set; } = true;
		/// <summary>
		/// Gets or sets the easing type for the planet animation.
		/// </summary>
		public EaseType PlanetEase { get; set; } = EaseType.Linear;
		/// <summary>
		/// Gets or sets the number of parts for the planet easing.
		/// </summary>
		public int PlanetEaseParts { get; set; } = 1;
		/// <summary>
		/// Gets or sets the behavior of the planet easing parts.
		/// </summary>
		public EasePartBehaviors PlanetEasePartBehavior { get; set; } = EasePartBehaviors.Mirror;
		/// <summary>  
		/// Gets or sets the custom class associated with the level.  
		/// </summary>  
		/// <remarks>  
		/// This property can be used to define a custom class or category for the level,  
		/// allowing for additional customization or grouping.  
		/// </remarks>  
		public string CustomClass { get; set; } = string.Empty;
		/// <summary>
		/// Gets or sets the default text color.
		/// </summary>
		public Color DefaultTextColor { get; set; } = Color.FromRgba("ffffff");
		/// <summary>
		/// Gets or sets the default text shadow color.
		/// </summary>
		public Color DefaultTextShadowColor { get; set; } = Color.FromRgba("000000");
		/// <summary>
		/// Gets or sets the congratulatory text displayed at the end of the level.
		/// </summary>
		public string CongratsText { get; set; } = string.Empty;
		/// <summary>
		/// Gets or sets the text displayed for a perfect score.
		/// </summary>
		public string PerfectText { get; set; } = string.Empty;
		/// <summary>
		/// Gets or sets a value indicating whether legacy flash effects are enabled.
		/// </summary>
		public bool LegacyFlash { get; set; } = false;
		/// <summary>
		/// Gets or sets a value indicating whether the camera uses legacy relative-to settings.
		/// </summary>
		public bool LegacyCamRelativeTo { get; set; } = false;
		/// <summary>
		/// Gets or sets a value indicating whether legacy sprite tiles are used.
		/// </summary>
		public bool LegacySpriteTiles { get; set; } = false;
		/// <summary>  
		/// Gets or sets a value indicating whether legacy tweening is enabled.  
		/// </summary>  
		public bool LegacyTween { get; set; } = false;
		/// <summary>  
		/// Gets or sets a value indicating whether version 15 features are disabled.  
		/// </summary>  
		public bool DisableV15Features { get; set; } = false;
	}
	/// <summary>
	/// Represents the available tile shapes used by the level track.
	/// </summary>
	/// <remarks>
	/// This enumeration is serialized to/from JSON with the <c>RDJsonEnumSerializable</c> attribute.
	/// It is used to determine how floor tiles are rendered and laid out in the level.
	/// </remarks>
	[JsonEnumSerializable]
	public enum TileShape
	{
		/// <summary>
		/// A long tile shape. Typically used for standard long floor segments.
		/// </summary>
		Long,
		/// <summary>
		/// A short tile shape. Typically used for shorter floor segments or tighter turns.
		/// </summary>
		Short,
		/// <summary>
		/// A custom tile shape. Indicates that the tile geometry is user-defined or uses custom dimensions.
		/// </summary>
		Custom,
	}
