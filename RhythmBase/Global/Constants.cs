namespace RhythmBase.Global;
/// <summary>
/// Provides a collection of constants used throughout the application for versioning and compatibility checks.
/// </summary>
/// <remarks>These constants define the minimum and default supported versions for A Dance of Fire and Ice
/// (Adofai) and Rhythm Doctor, ensuring that the application can validate and manage compatibility with these formats
/// effectively.</remarks>
public static partial class Constants
{
	/// <summary>
	/// Represents the tolerance level used for floating-point comparisons.
	/// </summary>
	public const float Tolerance = 0.00001f;
	extension(float a)
	{
		/// <summary>
		/// Determines whether the absolute value of the current value is less than the defined tolerance.
		/// </summary>
		/// <remarks>Use this method to check if a value is effectively considered zero within the specified
		/// tolerance, which is useful for floating-point comparisons where exact equality is unreliable.</remarks>
		/// <returns>true if the absolute value of the current value is less than the tolerance; otherwise, false.</returns>
		public bool IsToleranceZero => Math.Abs(a) < Tolerance;
		/// <summary>
		/// Determines whether the absolute difference between the current value and the specified value is less than the
		/// defined tolerance.
		/// </summary>
		/// <remarks>This method is useful for comparing floating-point values where precision errors may occur. The
		/// comparison uses a predefined tolerance to account for minor differences due to floating-point
		/// arithmetic.</remarks>
		/// <param name="b">The value to compare with the current value for equality within the specified tolerance.</param>
		/// <returns>true if the absolute difference between the current value and b is less than the tolerance; otherwise, false.</returns>
		public bool ToleranceSequals(float b) => Math.Abs(a - b) < Tolerance;
	}
}
