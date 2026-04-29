using RhythmBase.BeatBlock;
using RhythmBase.BeatBlock.Components;
using RhythmBase.BeatBlock.Events;
using RhythmBase.Global.Components.Easing;
using System.Drawing;
namespace RhythmBase.BeatBlock.Events;

public class CharacterMap
{
    public static readonly CharacterMap Regular = new CharacterMap("regular");
    public static readonly CharacterMap Full = new CharacterMap("full");
    public string Map { get; }
    public CharacterMap(string map)
    {
        Map = map;
    }
}
