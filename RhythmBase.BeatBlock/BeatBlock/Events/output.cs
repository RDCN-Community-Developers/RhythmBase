namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Represents a character map for BeatBlock levels.
/// </summary>
[JsonObjectSerializable]
public class CharacterMap
{
    /// <summary>
    /// Gets the default character map.
    /// </summary>
    public static CharacterMap Default { get; } = new CharacterMap();
    /// <summary>
    /// Gets the regular character map.
    /// </summary>
    public static readonly CharacterMap Regular = new("regular");
    /// <summary>
    /// Gets the full character map.
    /// </summary>
    public static readonly CharacterMap Full = new("full");
    /// <summary>
    /// Gets the map string.
    /// </summary>
    public string Map { get; }
    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterMap"/> class.
    /// </summary>
    /// <param name="map">The map string.</param>
    public CharacterMap(string map)
    {
        Map = map;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterMap"/> class with an empty map string.
    /// </summary>
    public CharacterMap() : this(string.Empty)
    {
    }
}
