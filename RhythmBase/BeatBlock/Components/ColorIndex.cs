namespace RhythmBase.BeatBlock.Components;

public struct ColorIndex
{
    public ColorIndex(byte value)
    {
        if (value >= 8)
            throw new ArgumentOutOfRangeException("value");
        Value = value;
    }
    public static implicit operator ColorIndex(byte value) => new ColorIndex { Value = value };
    public static implicit operator byte(ColorIndex index) => index.Value;
    public byte Value { get; set; }
}
