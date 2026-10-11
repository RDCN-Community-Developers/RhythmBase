namespace RhythmBase.Adofai.Components;

/// <summary>
/// Represents a pair of X and Y value ranges.
/// </summary>
/// <param name="x">The X value range.</param>
/// <param name="y">The Y value range.</param>
public struct ValueRangePair(ValueRange x, ValueRange y)
{
	/// <summary>
	/// Gets or sets the X value range.
	/// </summary>
	public ValueRange X { get; set; } = x;
	/// <summary>
	/// Gets or sets the Y value range.
	/// </summary>
	public ValueRange Y { get; set; } = y;
}