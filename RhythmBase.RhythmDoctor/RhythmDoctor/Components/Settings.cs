using RhythmBase.Global.Components;
using RhythmBase.Global.Components.RichText;
using System.Text.Json;
namespace RhythmBase.RhythmDoctor.Components;

/// <summary>  
/// Level settings.  
/// </summary>  
public class Settings
{
	private int[] rankMaxMistakes = new int[4];
	private string[] rankDescription = ["", "", "", "", "", ""];
	/// <summary>  
	/// The version number of the level.  
	/// The minimum level version number supported by this library is 55.  
	/// </summary>  
	public int Version { get; set; } = DefaultVersionRhythmDoctor;
	/// <summary>  
	/// Song artist.  
	/// </summary>  
	public string Artist { get; set; } = "";
	/// <summary>  
	/// Song name.  
	/// </summary>  
	public RichLine<RichStringStyle> Song { get; set; } = RichLine<RichStringStyle>.Empty;
	/// <summary>  
	/// Special artist type.  
	/// </summary>  
	public SpecialArtistTypes SpecialArtistType { get; set; } = SpecialArtistTypes.None;
	/// <summary>  
	/// File path for proof of artist's permission.  
	/// </summary>  
	public FileReference ArtistPermission { get; set; } = FileReference.Empty;
	/// <summary>  
	/// Artist links.  
	/// </summary>  
	public string ArtistLinks { get; set; } = "";
	/// <summary>  
	/// Level author.  
	/// </summary>  
	public RichLine<RichStringStyle> Author { get; set; } = RichLine<RichStringStyle>.Empty;
	/// <summary>  
	/// Level difficulty.  
	/// </summary>  
	public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Easy;
	/// <summary>  
	/// Show seizure warning.  
	/// </summary>  
	public bool SeizureWarning { get; set; } = false;
	/// <summary>  
	/// Preview image file path.  
	/// </summary>  
	public FileReference PreviewImage { get; set; } = FileReference.Empty;
	/// <summary>  
	/// Syringe packaging image file path.  
	/// </summary>  
	public FileReference SyringeIcon { get; set; } = FileReference.Empty;
	/// <summary>  
	/// The file path of the music used for previewing.  
	/// </summary>  
	public FileReference PreviewSong { get; set; } = FileReference.Empty;
	/// <summary>  
	/// Start time of preview music.  
	/// </summary>  
	public TimeSpan PreviewSongStartTime { get; set; } = TimeSpan.Zero;
	/// <summary>  
	/// Duration of preview music.  
	/// </summary>  
	public TimeSpan PreviewSongDuration { get; set; } = TimeSpan.Zero;
	/// <summary>  
	/// Hue offset or grayscale of the level name on the syringe.  
	/// </summary>  
	public float SongNameHueOrGrayscale { get; set; } = 0f;
	/// <summary>  
	/// Whether grayscale is enabled.  
	/// </summary>  
	public bool SongLabelGrayscale { get; set; } = false;
	/// <summary>  
	/// Level description.  
	/// </summary>  
	public RichLine<RichStringStyle> Description { get; set; } = RichLine<RichStringStyle>.Empty;
	/// <summary>  
	/// Level tags.  
	/// </summary>  
	public RichLine<RichStringStyle> Tags { get; set; } = RichLine<RichStringStyle>.Empty;
	/// <summary>  
	/// Separate two-player level file paths.  
	/// It is uncertain if this attribute is still being used.  
	/// </summary>  
	public string Separate2PLevelFilename { get; set; } = "";
	/// <summary>  
	/// Level play mode.  
	/// </summary>  
	public LevelPlayedMode CanBePlayedOn { get; set; } = LevelPlayedMode.OnePlayerOnly;
	/// <summary>  
	/// Behavior of the first beat of the level.  
	/// </summary>  
	public FirstBeatBehaviors FirstBeatBehavior { get; set; } = FirstBeatBehaviors.RunNormally;
	/// <summary>  
	/// Appearance of the level in multiplayer mode.  
	/// </summary>  
	public MultiplayerAppearances MultiplayerAppearance { get; set; } = MultiplayerAppearances.HorizontalStrips;
	/// <summary>  
	/// A percentage value indicating the total volume of the level.  
	/// </summary>  
	public float LevelVolume { get; set; } = 1f;
	/// <summary>  
	/// Maximum number of mistakes per rank.  
	/// </summary>  
	public int[] RankMaxMistakes
	{
		get => rankMaxMistakes;
		set => rankMaxMistakes = value.Length == 4 ? value : throw new RhythmBaseException();
	}
	/// <summary>  
	/// Description of each rank.  
	/// </summary>  
	public string[] RankDescription
	{
		get => rankDescription;
		set => rankDescription = value.Length == 6 ? value : throw new RhythmBaseException();
	}
	/// <summary>  
	/// Mods enabled for the level.  
	/// </summary>  
	public List<string> Mods { get; set; } = [];
	/// <summary>
	/// Gets or sets the JSON element associated with the specified property name.
	/// </summary>
	/// <remarks>If the value being set is undefined, the property will be removed from the collection. Otherwise,
	/// the property will be updated with the new value.</remarks>
	/// <param name="propertyName">The name of the property to get or set. This must be a valid string identifier for the JSON element.</param>
	/// <returns>The JSON element associated with the specified property name. Returns default if the property does not exist.</returns>
	public JsonElement this[string propertyName]
	{
		get => _extraData.TryGetValue(propertyName, out JsonElement value) ? value : default;
		set
		{
			if (value.ValueKind == JsonValueKind.Undefined)
				_extraData.Remove(propertyName);
			else
				_extraData[propertyName] = value;
		}
	}
	internal Dictionary<string, JsonElement> _extraData = [];
	internal IEnumerable<FileReference> GetAllFileReferences()
	{
		yield return ArtistPermission;
		yield return PreviewImage;
		yield return SyringeIcon;
		yield return PreviewSong;
	}
}
/// <summary>  
/// Difficulty level of the level.  
/// </summary>  
[JsonEnumSerializable]
public enum DifficultyLevel
{
	/// <summary>  
	/// Easy difficulty.  
	/// </summary>  
	Easy,
	/// <summary>  
	/// Medium difficulty.  
	/// </summary>  
	Medium,
	/// <summary>  
	/// Tough difficulty.  
	/// </summary>  
	Tough,
	/// <summary>  
	/// Very tough difficulty.  
	/// </summary>  
	VeryTough
}
/// <summary>  
/// Play mode of the level.  
/// </summary>  
[JsonEnumSerializable]
public enum LevelPlayedMode
{
	/// <summary>  
	/// Can be played by one player only.  
	/// </summary>  
	OnePlayerOnly,
	/// <summary>  
	/// Can be played by two players only.  
	/// </summary>  
	TwoPlayerOnly,
	/// <summary>  
	/// Can be played in both one-player and two-player modes.  
	/// </summary>  
	BothModes
}
/// <summary>  
/// Behavior of the first beat of the level.  
/// </summary>  
[JsonEnumSerializable]
public enum FirstBeatBehaviors
{
	/// <summary>  
	/// Run normally.  
	/// </summary>  
	RunNormally,
	/// <summary>  
	/// Run events on prebar.  
	/// </summary>  
	RunEventsOnPrebar
}
/// <summary>  
/// Appearance of the level in multiplayer mode.  
/// </summary>  
[JsonEnumSerializable]
public enum MultiplayerAppearances
{
	/// <summary>  
	/// Horizontal strips appearance.  
	/// </summary>  
	HorizontalStrips,
	/// <summary>  
	/// No special appearance.  
	/// </summary>  
	Nothing
}
