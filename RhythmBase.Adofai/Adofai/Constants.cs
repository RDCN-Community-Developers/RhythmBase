namespace RhythmBase.Adofai;

/// <summary>
/// Provides constants and utility methods for A Dance of Fire and Ice level files.
/// </summary>
public static partial class Constants
{
	public static partial float DefaultBpm => 100f;
	/// <summary>
	/// The minimum Adofai format version RhythmBase accepts after upgrading. Aligned with the game's
	/// current format (<c>ADOFAI.EditorConstants.adofaiFileVersion</c>).
	/// </summary>
	public const int MinimumSupportedVersion = 19;

	/// <summary>
	/// The default Adofai format version assumed when creating or exporting content.
	/// Aligned with the game's <c>ADOFAI.EditorConstants.adofaiFileVersion</c>.
	/// </summary>
	public const int DefaultVersion = 19;
}
