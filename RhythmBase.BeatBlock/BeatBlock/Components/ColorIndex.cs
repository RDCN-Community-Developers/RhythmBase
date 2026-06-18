namespace RhythmBase.BeatBlock.Components;

/// <summary>
/// Represents an index into a color palette.
/// </summary>
public struct ColorIndex
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColorIndex"/> struct.
    /// </summary>
    /// <param name="value">The index value. Must be less than 8.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is greater than or equal to 8.</exception>
    public ColorIndex(byte value)
    {
        if (value >= 8)
            throw new ArgumentOutOfRangeException(nameof(value));
        Value = value;
    }
    /// <summary>
    /// Implicitly converts a <see cref="byte"/> to a <see cref="ColorIndex"/>.
    /// </summary>
    /// <param name="value">The byte value.</param>
    /// <returns>A <see cref="ColorIndex"/> with the specified value.</returns>
    public static implicit operator ColorIndex(byte value) => new() { Value = value };
    /// <summary>
    /// Implicitly converts a <see cref="ColorIndex"/> to a <see cref="byte"/>.
    /// </summary>
    /// <param name="index">The color index.</param>
    /// <returns>The byte value of the color index.</returns>
    public static implicit operator byte(ColorIndex index) => index.Value;
    /// <summary>
    /// Gets or sets the index value.
    /// </summary>
    public byte Value { get; set; }
    /// <inheritdoc/>
    public override string ToString()
    {
        return Value.ToString();
    }
}
