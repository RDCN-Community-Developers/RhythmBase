using RhythmBase.BeatBlock;
using RhythmBase.BeatBlock.Components;
using RhythmBase.BeatBlock.Events;
using RhythmBase.Global.Components.Easing;
using System.Drawing;
namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Represents a character map for BeatBlock levels.
/// </summary>
public class CharacterMap
{
    /// <summary>
    /// Gets the regular character map.
    /// </summary>
    public static readonly CharacterMap Regular = new CharacterMap("regular");
    /// <summary>
    /// Gets the full character map.
    /// </summary>
    public static readonly CharacterMap Full = new CharacterMap("full");
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
}
