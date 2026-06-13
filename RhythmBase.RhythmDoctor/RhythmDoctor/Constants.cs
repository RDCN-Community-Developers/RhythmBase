using System.Collections.ObjectModel;

namespace RhythmBase.RhythmDoctor;

/// <summary>
/// Provides constant values in the Rhythm Doctor game.
/// </summary>
public static class Constants
{
	public const int DefaultVersion = 67;
	/// <summary>
	/// Bitmask flags that describe the capabilities and application targets of a VFX preset.
	/// </summary>
	/// <remarks>
	/// The values are intended to be combined using bitwise operators to express multiple attributes
	/// (for example: <c>MultiRooms | EnableIntensity</c>). The underlying storage type is <see cref="short"/>.
	/// Use bitwise checks (for example, <c>(attributes &amp; VfxAttribute.EnableIntensity) != 0</c>) to test for features.
	/// </remarks>
	public enum VfxAttribute : short
	{
		/// <summary>
		/// The preset is disabled or not available.
		/// </summary>
		Disabled = 1,
		/// <summary>
		/// The preset applies only to the top room.
		/// </summary>
		TopOnly = 0b100_0,
		/// <summary>
		/// The preset applies to a single room (non-top).
		/// </summary>
		SingleRoom = 0b001_0,
		/// <summary>
		/// The preset applies either to a single room or to the top room.
		/// </summary>
		SingleRoomOrTop = 0b101_0,
		/// <summary>
		/// The preset applies to multiple rooms.
		/// </summary>
		MultiRooms = 0b010_0,
		/// <summary>
		/// The preset applies to multiple rooms and can also affect the top room.
		/// </summary>
		MultiRoomsWithTop = 0b110_0,
		/// <summary>
		/// The preset supports an intensity parameter.
		/// </summary>
		EnableIntensity = 0b001_000_0,
		/// <summary>
		/// The preset supports X/Y parameters.
		/// </summary>
		EnableXY = 0b010_000_0,
		/// <summary>
		/// The preset supports absolute positioning or absolute mode.
		/// </summary>
		Absolute = 0b100_000_0,
		/// <summary>
		/// The preset supports absolute intensity in addition to other attributes.
		/// </summary>
		EnableAbsoluteIntensity = 0b101_000_0,
		/// <summary>
		/// The preset supports absolute X/Y coordinates.
		/// </summary>
		EnableAbsoluteXY = 0b110_000_0,
		/// <summary>
		/// The preset supports easing (smooth interpolation).
		/// </summary>
		EnableEase = 0b1_000_000_0,
		/// <summary>
		/// The preset supports a threshold parameter.
		/// </summary>
		EnableThreshold = 0b10_000_000_0,
		/// <summary>
		/// The preset supports color adjustments.
		/// </summary>
		EnableColor = 0b100_000_000_0,
		/// <summary>
		/// The preset supports a speed parameter.
		/// </summary>
		EnableSpeed = 0b1000_000_000_0,
	}
	/// <summary>
	/// The default beats per minute for a new level.
	/// </summary>
	public const float DefaultBpm = 100f;
	/// <summary>
	/// The default number of crotchets per bar for a new level.
	/// </summary>
	public const int DefaultCpb = 8;
	/// <summary>
	/// Read-only mapping that associates each <see cref="VfxPreset"/> with the corresponding <see cref="VfxAttribute"/> flags.
	/// </summary>
	/// <remarks>
	/// Use this dictionary to determine what features a given preset supports (for example, whether it affects multiple rooms,
	/// supports intensity, color, easing, etc.). The values are intended to be tested with bitwise operations.
	/// </remarks>
	public static ReadOnlyDictionary<VfxPreset, VfxAttribute> VfxAttributes => _vfxAttributes;
	/// <summary>
	/// Represents the total number of rooms available.
	/// </summary>

	public const int RoomCount = 4;

	private static readonly ReadOnlyDictionary<VfxPreset, VfxAttribute> _vfxAttributes = new(new Dictionary<VfxPreset, VfxAttribute>
	{
		[VfxPreset.Vignette] = VfxAttribute.MultiRooms,
		[VfxPreset.VignetteFlicker] = VfxAttribute.MultiRooms,
		[VfxPreset.CutsceneMode] = VfxAttribute.MultiRooms,
		[VfxPreset.WavyRows] = VfxAttribute.MultiRooms | VfxAttribute.EnableIntensity | VfxAttribute.EnableEase | VfxAttribute.EnableSpeed,
		[VfxPreset.LightStripVert] = VfxAttribute.MultiRooms,
		[VfxPreset.SilhouettesOnHBeat] = VfxAttribute.MultiRooms,
		[VfxPreset.ColourfulShockwaves] = VfxAttribute.MultiRooms,
		[VfxPreset.BassDropOnHit] = VfxAttribute.MultiRooms,
		[VfxPreset.ShakeOnHeartBeat] = VfxAttribute.MultiRooms,
		[VfxPreset.ShakeOnHit] = VfxAttribute.MultiRooms,
		[VfxPreset.NumbersAbovePulses] = VfxAttribute.MultiRooms,
		[VfxPreset.FallingPetals] = VfxAttribute.MultiRooms,
		[VfxPreset.FallingPetalsInstant] = VfxAttribute.MultiRooms,
		[VfxPreset.FallingPetalsSnow] = VfxAttribute.MultiRooms,
		[VfxPreset.FallingLeaves] = VfxAttribute.MultiRooms,
		[VfxPreset.Rain] = VfxAttribute.MultiRooms | VfxAttribute.EnableIntensity | VfxAttribute.EnableEase,
		[VfxPreset.Snow] = VfxAttribute.MultiRooms,
		[VfxPreset.Blizzard] = VfxAttribute.MultiRooms | VfxAttribute.EnableIntensity | VfxAttribute.EnableEase,
		[VfxPreset.Embers] = VfxAttribute.MultiRooms | VfxAttribute.EnableIntensity | VfxAttribute.EnableEase | VfxAttribute.EnableColor,
		[VfxPreset.Matrix] = VfxAttribute.MultiRooms,
		[VfxPreset.Diamonds] = VfxAttribute.MultiRooms | VfxAttribute.EnableIntensity | VfxAttribute.EnableEase | VfxAttribute.EnableColor,
		[VfxPreset.Confetti] = VfxAttribute.MultiRooms,
		[VfxPreset.ConfettiBurst] = VfxAttribute.MultiRooms,
		[VfxPreset.Balloons] = VfxAttribute.MultiRooms,
		[VfxPreset.VHS] = VfxAttribute.MultiRooms,
		[VfxPreset.VHSRewind] = VfxAttribute.MultiRooms | VfxAttribute.EnableIntensity | VfxAttribute.EnableEase,
		[VfxPreset.Scanlines] = VfxAttribute.MultiRooms,
		[VfxPreset.Aberration] = VfxAttribute.MultiRooms | VfxAttribute.EnableIntensity | VfxAttribute.EnableEase,
		[VfxPreset.JPEG] = VfxAttribute.MultiRooms | VfxAttribute.EnableIntensity | VfxAttribute.EnableEase,
		[VfxPreset.Grain] = VfxAttribute.MultiRooms | VfxAttribute.EnableIntensity | VfxAttribute.EnableEase,
		[VfxPreset.Blur] = VfxAttribute.MultiRooms | VfxAttribute.EnableIntensity | VfxAttribute.EnableEase,
		[VfxPreset.RadialBlur] = VfxAttribute.MultiRooms | VfxAttribute.EnableIntensity | VfxAttribute.EnableEase,
		[VfxPreset.Fisheye] = VfxAttribute.MultiRoomsWithTop | VfxAttribute.EnableIntensity | VfxAttribute.EnableEase,
		[VfxPreset.HallOfMirrors] = VfxAttribute.MultiRoomsWithTop,
		[VfxPreset.TileN] = VfxAttribute.MultiRoomsWithTop | VfxAttribute.EnableEase | VfxAttribute.EnableAbsoluteXY,
		[VfxPreset.CustomScreenScroll] = VfxAttribute.MultiRoomsWithTop | VfxAttribute.EnableEase | VfxAttribute.EnableAbsoluteXY,
		[VfxPreset.ScreenWaves] = VfxAttribute.MultiRooms | VfxAttribute.EnableIntensity | VfxAttribute.EnableEase,
		[VfxPreset.HeatDistortion] = VfxAttribute.MultiRooms | VfxAttribute.EnableIntensity | VfxAttribute.EnableEase,
		[VfxPreset.Pixelate] = VfxAttribute.MultiRooms | VfxAttribute.EnableXY | VfxAttribute.EnableEase,
		[VfxPreset.Mosaic] = VfxAttribute.MultiRooms | VfxAttribute.EnableIntensity | VfxAttribute.EnableEase,
		[VfxPreset.GlassShatter] = VfxAttribute.MultiRooms,
		[VfxPreset.GlitchObstruction] = VfxAttribute.MultiRoomsWithTop,
		[VfxPreset.Noise] = VfxAttribute.MultiRoomsWithTop,
		[VfxPreset.HueShift] = VfxAttribute.MultiRoomsWithTop | VfxAttribute.EnableIntensity | VfxAttribute.EnableEase,
		[VfxPreset.Brightness] = VfxAttribute.MultiRoomsWithTop | VfxAttribute.EnableIntensity | VfxAttribute.EnableEase,
		[VfxPreset.Contrast] = VfxAttribute.MultiRoomsWithTop | VfxAttribute.EnableIntensity | VfxAttribute.EnableEase,
		[VfxPreset.Saturation] = VfxAttribute.MultiRoomsWithTop | VfxAttribute.EnableIntensity | VfxAttribute.EnableEase,
		[VfxPreset.BlackAndWhite] = VfxAttribute.MultiRoomsWithTop,
		[VfxPreset.Sepia] = VfxAttribute.MultiRoomsWithTop,
		[VfxPreset.Bloom] = VfxAttribute.MultiRooms | VfxAttribute.EnableAbsoluteIntensity | VfxAttribute.EnableEase | VfxAttribute.EnableColor | VfxAttribute.EnableThreshold,
		[VfxPreset.OrangeBloom] = VfxAttribute.MultiRooms,
		[VfxPreset.BlueBloom] = VfxAttribute.MultiRooms,
		[VfxPreset.Funk] = VfxAttribute.MultiRooms,
		[VfxPreset.Drawing] = VfxAttribute.MultiRooms | VfxAttribute.EnableIntensity | VfxAttribute.EnableEase,
		[VfxPreset.Dots] = VfxAttribute.MultiRooms | VfxAttribute.EnableIntensity | VfxAttribute.EnableEase,
		[VfxPreset.EyesBig] = VfxAttribute.MultiRooms | VfxAttribute.EnableIntensity | VfxAttribute.EnableXY | VfxAttribute.EnableSpeed | VfxAttribute.EnableEase | VfxAttribute.EnableColor,
		[VfxPreset.EyesSmall] = VfxAttribute.MultiRooms | VfxAttribute.EnableIntensity | VfxAttribute.EnableXY | VfxAttribute.EnableSpeed | VfxAttribute.EnableEase | VfxAttribute.EnableColor,
		[VfxPreset.Tutorial] = VfxAttribute.MultiRooms | VfxAttribute.EnableIntensity | VfxAttribute.EnableEase,
		[VfxPreset.Tile2] = VfxAttribute.MultiRoomsWithTop | VfxAttribute.Disabled,
		[VfxPreset.Tile3] = VfxAttribute.MultiRoomsWithTop | VfxAttribute.Disabled,
		[VfxPreset.Tile4] = VfxAttribute.MultiRoomsWithTop | VfxAttribute.Disabled,
		[VfxPreset.ScreenScrollX] = VfxAttribute.MultiRooms | VfxAttribute.Disabled,
		[VfxPreset.ScreenScroll] = VfxAttribute.MultiRooms | VfxAttribute.Disabled,
		[VfxPreset.ScreenScrollSansVHS] = VfxAttribute.MultiRooms | VfxAttribute.Disabled,
		[VfxPreset.ScreenScrollXSansVHS] = VfxAttribute.MultiRooms | VfxAttribute.Disabled,
		[VfxPreset.RowGlowWhite] = VfxAttribute.MultiRooms | VfxAttribute.Disabled,
		[VfxPreset.RowOutline] = VfxAttribute.MultiRooms | VfxAttribute.Disabled,
		[VfxPreset.RowShadow] = VfxAttribute.MultiRooms | VfxAttribute.Disabled,
		[VfxPreset.RowAllWhite] = VfxAttribute.MultiRooms | VfxAttribute.Disabled,
		[VfxPreset.RowSilhouetteGlow] = VfxAttribute.MultiRooms | VfxAttribute.Disabled,
		[VfxPreset.RowPlain] = VfxAttribute.MultiRooms | VfxAttribute.Disabled,
		[VfxPreset.Blackout] = VfxAttribute.MultiRooms | VfxAttribute.Disabled,
		[VfxPreset.MiawMiaw] = VfxAttribute.MultiRooms | VfxAttribute.Disabled,
		[VfxPreset.DisableAll] = VfxAttribute.MultiRooms | VfxAttribute.Disabled,
	});

	public static readonly string[] LevelFileExtensions = new[] { ".json", ".rdlevel" };
	public static readonly string[] LevelZipExtensions = new[] { ".zip", ".rdzip" };
	public static readonly string[] ImageFileExtensions = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif" };
	public static readonly string[] WaveFileExtensions = new[] { ".wav", ".mp3", ".ogg", ".aac" };
	public static bool IsLevelFile(string filename)
	{
		string extension = Path.GetExtension(filename).ToLowerInvariant();
		return LevelFileExtensions.Contains(extension);
	}
	public static bool IsLevelZip(string filename)
	{
		string extension = Path.GetExtension(filename).ToLowerInvariant();
		return LevelZipExtensions.Contains(extension);
	}
	public static bool IsImageFile(string filename)
	{
		string extension = Path.GetExtension(filename).ToLowerInvariant();
		return ImageFileExtensions.Contains(extension);
	}
	public static bool IsWaveFile(string filename)
	{
		string extension = Path.GetExtension(filename).ToLowerInvariant();
		return WaveFileExtensions.Contains(extension);
	}
}
