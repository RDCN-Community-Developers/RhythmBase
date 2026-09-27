using System.Collections.ObjectModel;
using static RhythmBase.RhythmDoctor.Constants.VfxAttribute;

namespace RhythmBase.RhythmDoctor;

/// <summary>
/// Provides constant values in the game.
/// </summary>
public static partial class Constants
{
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
		/// <summary>
		/// The preset supports a position parameter.
		/// </summary>
		EnablePosition = 0b10000_000_000_0,
	}
	/// <summary>
	/// The default version number used when creating a new level.
	/// </summary>
	public const int DefaultVersion = 68;
	public static partial float DefaultBpm => 100f;
	/// <summary>
	/// The default number of crotchets per bar for a new level.
	/// </summary>
	public const int DefaultCpb = 8;
	/// <summary>
	/// Represents the total number of rooms available.
	/// </summary>
	public const int RoomCapacity = 4;
	/// <summary>
	/// Represents the total number of rows available in the game.
	/// </summary>
	public const int RowCapacity = 16;
	/// <summary>
	/// Represents the total number of beat in an <see cref="RhythmBase.RhythmDoctor.Events.AddClassicBeat"/> event.
	/// </summary>
	public const int ClassicBeatCapacity = 7;
	/// <summary>
	/// Represents the total number of palette colors available.
	/// </summary>
	public const int PaletteColorCount = 21;
	/// <summary>
	/// Read-only mapping that associates each <see cref="VfxPreset"/> with the corresponding <see cref="VfxAttribute"/> flags.
	/// </summary>
	/// <remarks>
	/// Use this dictionary to determine what features a given preset supports (for example, whether it affects multiple rooms,
	/// supports intensity, color, easing, etc.). The values are intended to be tested with bitwise operations.
	/// </remarks>
	public static IReadOnlyDictionary<VfxPreset, VfxAttribute> VfxAttributes => _vfxAttributes;
	private static readonly ReadOnlyDictionary<VfxPreset, VfxAttribute> _vfxAttributes = new(new Dictionary<VfxPreset, VfxAttribute>
	{
		[VfxPreset.Vignette] = MultiRooms,
		[VfxPreset.VignetteFlicker] = MultiRooms,
		[VfxPreset.CutsceneMode] = MultiRooms,
		[VfxPreset.WavyRows] = MultiRooms | EnableIntensity | EnableEase | EnableSpeed,
		[VfxPreset.LightStripVert] = MultiRooms,
		[VfxPreset.SilhouettesOnHBeat] = MultiRooms,
		[VfxPreset.ColourfulShockwaves] = MultiRooms,
		[VfxPreset.BassDropOnHit] = MultiRooms,
		[VfxPreset.ShakeOnHeartBeat] = MultiRooms,
		[VfxPreset.ShakeOnHit] = MultiRooms,
		[VfxPreset.NumbersAbovePulses] = MultiRooms,
		[VfxPreset.FallingPetals] = MultiRooms,
		[VfxPreset.FallingPetalsInstant] = MultiRooms,
		[VfxPreset.FallingPetalsSnow] = MultiRooms,
		[VfxPreset.FallingLeaves] = MultiRooms,
		[VfxPreset.Rain] = MultiRooms | EnableIntensity | EnableEase,
		[VfxPreset.Snow] = MultiRooms,
		[VfxPreset.Blizzard] = MultiRooms | EnableIntensity | EnableEase,
		[VfxPreset.Embers] = MultiRooms | EnableIntensity | EnableEase | EnableColor,
		[VfxPreset.Matrix] = MultiRooms,
		[VfxPreset.Diamonds] = MultiRooms | EnableIntensity | EnableEase | EnableColor,
		[VfxPreset.Confetti] = MultiRooms,
		[VfxPreset.ConfettiBurst] = MultiRooms,
		[VfxPreset.Balloons] = MultiRooms,
		[VfxPreset.VHS] = MultiRooms,
		[VfxPreset.VHSRewind] = MultiRooms | EnableIntensity | EnableEase,
		[VfxPreset.Scanlines] = MultiRooms,
		[VfxPreset.Aberration] = MultiRooms | EnableIntensity | EnableEase,
		[VfxPreset.JPEG] = MultiRooms | EnableIntensity | EnableEase,
		[VfxPreset.Grain] = MultiRooms | EnableIntensity | EnableEase,
		[VfxPreset.Blur] = MultiRooms | EnableIntensity | EnableEase,
		[VfxPreset.RadialBlur] = MultiRooms | EnableIntensity | EnablePosition | EnableEase,
		[VfxPreset.Fisheye] = MultiRoomsWithTop | EnableIntensity | EnableEase,
		[VfxPreset.HallOfMirrors] = MultiRoomsWithTop,
		[VfxPreset.TileN] = MultiRoomsWithTop | EnableEase | EnableXY | EnablePosition,
		[VfxPreset.CustomScreenScroll] = MultiRoomsWithTop | EnableEase | EnableAbsoluteXY,
		[VfxPreset.ScreenWaves] = MultiRooms | EnableIntensity | EnablePosition | EnableEase,
		[VfxPreset.HeatDistortion] = MultiRooms | EnableIntensity | EnablePosition | EnableEase,
		[VfxPreset.Pixelate] = MultiRooms | EnableXY | EnableEase,
		[VfxPreset.Mosaic] = MultiRooms | EnableIntensity | EnableEase,
		[VfxPreset.GlassShatter] = MultiRooms,
		[VfxPreset.GlitchObstruction] = MultiRoomsWithTop,
		[VfxPreset.Noise] = MultiRoomsWithTop,
		[VfxPreset.HueShift] = MultiRoomsWithTop | EnableIntensity | EnableEase,
		[VfxPreset.Brightness] = MultiRoomsWithTop | EnableIntensity | EnableEase,
		[VfxPreset.Contrast] = MultiRoomsWithTop | EnableIntensity | EnableEase,
		[VfxPreset.Saturation] = MultiRoomsWithTop | EnableIntensity | EnableEase,
		[VfxPreset.BlackAndWhite] = MultiRoomsWithTop,
		[VfxPreset.Sepia] = MultiRoomsWithTop,
		[VfxPreset.Bloom] = MultiRooms | EnableAbsoluteIntensity | EnableEase | EnableColor | EnableThreshold,
		[VfxPreset.OrangeBloom] = MultiRooms,
		[VfxPreset.BlueBloom] = MultiRooms,
		[VfxPreset.Funk] = MultiRooms,
		[VfxPreset.Drawing] = MultiRooms | EnableIntensity | EnableEase | EnableSpeed | EnableColor,
		[VfxPreset.Dots] = MultiRooms | EnableIntensity | EnableEase,
		[VfxPreset.EyesBig] = MultiRooms | EnableIntensity | EnableXY | EnableSpeed | EnableEase | EnableColor | EnablePosition,
		[VfxPreset.EyesSmall] = MultiRooms | EnableIntensity | EnableXY | EnableSpeed | EnableEase | EnableColor | EnablePosition,
		[VfxPreset.Tutorial] = MultiRooms | EnableIntensity | EnableEase | EnableColor,
		[VfxPreset.Tile2] = MultiRoomsWithTop | Disabled,
		[VfxPreset.Tile3] = MultiRoomsWithTop | Disabled,
		[VfxPreset.Tile4] = MultiRoomsWithTop | Disabled,
		[VfxPreset.ScreenScrollX] = MultiRooms | Disabled,
		[VfxPreset.ScreenScroll] = MultiRooms | Disabled,
		[VfxPreset.ScreenScrollSansVHS] = MultiRooms | Disabled,
		[VfxPreset.ScreenScrollXSansVHS] = MultiRooms | Disabled,
		[VfxPreset.RowGlowWhite] = MultiRooms | Disabled,
		[VfxPreset.RowOutline] = MultiRooms | Disabled,
		[VfxPreset.RowShadow] = MultiRooms | Disabled,
		[VfxPreset.RowAllWhite] = MultiRooms | Disabled,
		[VfxPreset.RowSilhouetteGlow] = MultiRooms | Disabled,
		[VfxPreset.RowPlain] = MultiRooms | Disabled,
		[VfxPreset.Blackout] = MultiRooms | Disabled,
		[VfxPreset.MiawMiaw] = MultiRooms | Disabled,
		[VfxPreset.DisableAll] = MultiRoomsWithTop,
	});
	/// <summary>
	/// Read-only mapping that associates each <see cref="SoundType"/> with a collection of related <see cref="SoundType"/> values.
	/// </summary>
	public static IReadOnlyDictionary<SoundType, SoundType[]> SoundGroupTypeMap => _soundGroupTypeMap;
	private static readonly ReadOnlyDictionary<SoundType, SoundType[]> _soundGroupTypeMap = new(
		new Dictionary<SoundType, SoundType[]>
		{
			[SoundType.ClapSoundHold] = [SoundType.ClapSoundHoldLongEnd, SoundType.ClapSoundHoldLongStart, SoundType.ClapSoundHoldShortEnd, SoundType.ClapSoundHoldShortStart,],
			[SoundType.PulseSoundHold] = [SoundType.PulseSoundHoldStart, SoundType.PulseSoundHoldShortEnd, SoundType.PulseSoundHoldEnd, SoundType.PulseSoundHoldStartAlt, SoundType.PulseSoundHoldShortEndAlt, SoundType.PulseSoundHoldEndAlt,],
			[SoundType.ClapSoundHoldP2] = [SoundType.ClapSoundHoldLongEndP2, SoundType.ClapSoundHoldLongStartP2, SoundType.ClapSoundHoldShortEndP2, SoundType.ClapSoundHoldShortStartP2,],
			[SoundType.PulseSoundHoldP2] = [SoundType.PulseSoundHoldStartP2, SoundType.PulseSoundHoldShortEndP2, SoundType.PulseSoundHoldEndP2, SoundType.PulseSoundHoldStartAltP2, SoundType.PulseSoundHoldShortEndAltP2, SoundType.PulseSoundHoldEndAltP2,],
			[SoundType.FreezeshotSound] = [SoundType.FreezeshotSoundCueLow, SoundType.FreezeshotSoundCueHigh, SoundType.FreezeshotSoundRiser, SoundType.FreezeshotSoundCymbal,],
			[SoundType.BurnshotSound] = [SoundType.BurnshotSoundCueLow, SoundType.BurnshotSoundCueHigh, SoundType.BurnshotSoundRiser, SoundType.BurnshotSoundCymbal,],
			[SoundType.HoldshotSound] = [SoundType.HoldshotSoundCue, SoundType.HoldshotSoundClapStart, SoundType.HoldshotSoundClapShortEnd, SoundType.HoldshotSoundClapLongEnd,],
		});
	/// <summary>
	/// File extensions recognized as Rhythm Doctor level files.
	/// </summary>
	public static readonly string[] LevelFileExtensions = [".json", ".rdlevel"];
	/// <summary>
	/// File extensions recognized as Rhythm Doctor level archives.
	/// </summary>
	public static readonly string[] LevelZipExtensions = [".zip", ".rdzip"];
	/// <summary>
	/// File extensions recognized as image files.
	/// </summary>
	public static readonly string[] ImageFileExtensions = [".png", ".jpg", ".jpeg", ".bmp", ".gif"];
	/// <summary>
	/// File extensions recognized as audio files.
	/// </summary>
	public static readonly string[] WaveFileExtensions = [".wav", ".mp3", ".ogg", ".aac"];
	/// <summary>
	/// Determines whether the specified file path has a Rhythm Doctor level file extension.
	/// </summary>
	/// <param name="filename">The file path to check.</param>
	/// <returns><see langword="true"/> if the file has a recognized level file extension; otherwise, <see langword="false"/>.</returns>
	public static bool IsLevelFile(string filename)
	{
		string extension = Path.GetExtension(filename).ToLowerInvariant();
		return LevelFileExtensions.Contains(extension);
	}
	/// <summary>
	/// Determines whether the specified file path has a Rhythm Doctor level archive extension.
	/// </summary>
	/// <param name="filename">The file path to check.</param>
	/// <returns><see langword="true"/> if the file has a recognized level archive extension; otherwise, <see langword="false"/>.</returns>
	public static bool IsLevelZip(string filename)
	{
		string extension = Path.GetExtension(filename).ToLowerInvariant();
		return LevelZipExtensions.Contains(extension);
	}
	/// <summary>
	/// Determines whether the specified file path has an image file extension.
	/// </summary>
	/// <param name="filename">The file path to check.</param>
	/// <returns><see langword="true"/> if the file has a recognized image file extension; otherwise, <see langword="false"/>.</returns>
	public static bool IsImageFile(string filename)
	{
		string extension = Path.GetExtension(filename).ToLowerInvariant();
		return ImageFileExtensions.Contains(extension);
	}
	/// <summary>
	/// Determines whether the specified file path has an audio file extension.
	/// </summary>
	/// <param name="filename">The file path to check.</param>
	/// <returns><see langword="true"/> if the file has a recognized audio file extension; otherwise, <see langword="false"/>.</returns>
	public static bool IsWaveFile(string filename)
	{
		string extension = Path.GetExtension(filename).ToLowerInvariant();
		return WaveFileExtensions.Contains(extension);
	}
}