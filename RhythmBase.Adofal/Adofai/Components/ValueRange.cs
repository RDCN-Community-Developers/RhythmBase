namespace RhythmBase.Adofai.Components;

/// <summary>
/// Represents a range of values with a minimum and maximum.
/// </summary>
public struct ValueRange
{
	/// <summary>
	/// Gets or sets the minimum value of the range.
	/// </summary>
	public float Min { get; set; }

	/// <summary>
	/// Gets or sets the maximum value of the range.
	/// </summary>
	public float Max { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="ValueRange"/> struct with the specified minimum and maximum values.
	/// </summary>
	/// <param name="min">The minimum value of the range.</param>
	/// <param name="max">The maximum value of the range.</param>
	public ValueRange(float min, float max)
	{
		this.Min = min;
		this.Max = max;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ValueRange"/> struct with a single value for both minimum and maximum.
	/// </summary>
	/// <param name="value">The value to set for both minimum and maximum.</param>
	public ValueRange(float value)
	{
		this.Min = value;
		this.Max = value;
	}
}
