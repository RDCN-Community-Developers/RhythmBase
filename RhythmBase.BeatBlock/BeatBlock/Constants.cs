using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RhythmBase.BeatBlock;

/// <summary>
/// Contains constant values used by the BeatBlock level format.
/// </summary>
public static partial class Constants
{ 
	public static partial float DefaultBpm => 100f;
	/// <summary>
	/// Represents the minimum supported version of the Rhythm Doctor application required for compatibility.
	/// </summary>
	public const int MinimumSupportedVersion = 14;

	/// <summary>
	/// The default Rhythm Doctor version used when no explicit target version is specified.
	/// </summary>
	public const int DefaultVersion = 18;

	/// <summary>
	/// Read-only mapping of the game's built-in sound effect names to their relative asset paths.
	/// A <c>playSound</c> whose <c>sound</c> is not one of these names is treated as an external file path
	/// during format upgrades. <c>map</c> is a nested group and has no single file path.
	/// </summary>
	public static IReadOnlyDictionary<string, string> BuiltInSounds => _builtInSounds;
	private static readonly ReadOnlyDictionary<string, string> _builtInSounds = new(
		new Dictionary<string, string>(StringComparer.Ordinal)
		{
			["click"] = "assets/sfx/click.ogg",
			["hold"] = "assets/sfx/hold.ogg",
			["barely"] = "assets/sfx/barely.ogg",
			["mine"] = "assets/sfx/mine.ogg",
			["tap"] = "assets/sfx/tap.ogg",
			["side"] = "assets/sfx/side.ogg",
			["pause"] = "assets/sfx/pause.ogg",
			["map"] = "",
			["ttsminebeep"] = "assets/sfx/ttsminebeep.ogg",
			["ttsexplosion"] = "assets/sfx/ttsexplosion.ogg",
		});

	/// <summary>
	/// Determines whether the specified name is a built-in sound effect.
	/// </summary>
	/// <param name="name">The sound name to check.</param>
	/// <returns><see langword="true"/> if the name is built in; otherwise, <see langword="false"/>.</returns>
	public static bool IsBuiltInSound(string name) => _builtInSounds.ContainsKey(name);
}
