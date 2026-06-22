using System.Collections;

namespace RhythmBase.BeatBlock.Components;

/// <summary>
/// Represents an index into a color palette.
/// </summary>
public record struct ColorIndex : IEquatable<ColorIndex>
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
	internal byte Value { get; set; }
	/// <inheritdoc/>
	public readonly override string ToString() => Value.ToString();
	/// <inheritdoc/>
	public readonly bool Equals(ColorIndex other) => Value == other.Value;
	/// <inheritdoc/>
	public readonly override int GetHashCode() => Value;
}
/// <summary>
/// Represents an index into a color palette, with a default value that can be used to indicate the absence of a color index.
/// </summary>
public record struct ColorIndexOrDefault
{
	private const byte defaultValue = byte.MaxValue;
	/// <summary>
	/// Gets the default <see cref="ColorIndexOrDefault"/> instance, which has a value of <see cref="defaultValue"/> (255). This can be used to represent the absence of a color index.
	/// </summary>
	public static ColorIndexOrDefault Default => new() { Value = defaultValue };
	/// <summary>
	/// Initializes a new instance of the <see cref="ColorIndexOrDefault"/> struct.
	/// </summary>
	/// <param name="value">The index value. Must be less than 8.</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is greater than or equal to 8.</exception>
	public ColorIndexOrDefault(byte value)
	{
		if (value >= 8)
			throw new ArgumentOutOfRangeException(nameof(value));
		Value = value;
	}
	/// <summary>
	/// Implicitly converts a <see cref="byte"/> to a <see cref="ColorIndex"/>.
	/// </summary>
	/// <param name="value">The byte value.</param>
	/// <returns>A <see cref="ColorIndexOrDefault"/> with the specified value.</returns>
	public static implicit operator ColorIndexOrDefault(byte value) => new() { Value = value };
	/// <summary>
	/// Implicitly converts a <see cref="ColorIndex"/> to a <see cref="ColorIndexOrDefault"/>.
	/// </summary>
	/// <param name="index">The color index.</param>
	/// <returns>A <see cref="ColorIndexOrDefault"/> with the specified value.</returns>
	public static implicit operator ColorIndexOrDefault(ColorIndex index) => new() { Value = index.Value };
	/// <summary>
	/// Implicitly converts a <see cref="ColorIndexOrDefault"/> to a <see cref="byte"/>.
	/// </summary>
	/// <param name="index">The color index.</param>
	/// <returns>The byte value of the color index.</returns>
	public static implicit operator byte(ColorIndexOrDefault index) => index.Value;
	internal byte Value { get; set; }
	/// <inheritdoc/>
	public readonly override string ToString() => Value.ToString();
	/// <inheritdoc/>
	public readonly bool Equals(ColorIndexOrDefault other) => Value == other.Value;
	/// <inheritdoc/>
	public readonly override int GetHashCode() => Value;
}
