namespace RhythmBase.Adofai.Components;

/// <summary>
/// Initializes a new instance of the <see cref="ValueRangePair"/> struct with the specified X and Y value ranges.
/// </summary>
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