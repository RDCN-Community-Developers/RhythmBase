namespace RhythmBase.Global.Components;


/// <summary>
/// Represents a beat that can be compared and equated with other beats of the same type.
/// </summary>
/// <typeparam name="TSelf">The type that implements this interface.</typeparam>
public interface ITickTime<TSelf> : IComparable<TSelf>, IEquatable<TSelf> where TSelf : ITickTime<TSelf>
{
	/// <summary>
	/// Gets the time span of the beat.
	/// </summary>
	public TimeSpan TimeSpan { get; }
	/// <summary>
	/// Gets the beat-only value.
	/// </summary>
	public float Tick { get; }
#if NET8_0_OR_GREATER
	/// <summary>
	/// Compares two values to compute which is greater.
	/// </summary>
	/// <param name="x">The value to compare with y.</param>
	/// <param name="y">The value to compare with x.</param>
	/// <returns>x if it is greater than y; otherwise, y.</returns>
	public static abstract TSelf Min(TSelf x, TSelf y);
	/// <summary>
	/// Compares two values to compute which is lesser.
	/// </summary>
	/// <param name="x">The value to compare with y.</param>
	/// <param name="y">The value to compare with x.</param>
	/// <returns>x if it is lesser than y; otherwise, y.</returns>
	public static abstract TSelf Max(TSelf x, TSelf y);
#endif
}
