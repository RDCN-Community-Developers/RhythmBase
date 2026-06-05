namespace RhythmBase.Global.Components;

/// <summary>
/// Represents a range of beats.
/// </summary>
/// <typeparam name="TTickTime">The type of beat. Must be a struct and implement <see cref="ITickTime{TBeat}"/>.</typeparam>
public interface ITickRange<TTickTime>
    where TTickTime : struct, ITickTime<TTickTime>
{
    /// <summary>
    /// Start beat.
    /// </summary>
    public TTickTime? Start { get; }
    /// <summary>
    /// End beat.
    /// </summary>
    public TTickTime? End { get; }
    /// <summary>
    /// Determines whether the specified beat is within the range.
    /// </summary>
    /// <param name="b">The beat to check.</param>
    /// <returns>True if the beat is within the range; otherwise, false.</returns>
    public bool Contains(TTickTime b);
    /// <summary>
    /// Gets the start and end beats of the range, using the specified default end time if the range has no end.
    /// </summary>
    /// <param name="endTime">The default end time to use if the range has no explicit end.</param>
    /// <returns>A tuple containing the start and end beats.</returns>
    (TTickTime Start, TTickTime End) GetStartAndEnd(TTickTime endTime);
    /// <summary>
    /// Returns a new range that represents the intersection of this range and another range.
    /// </summary>
    /// <param name="other">The other range to intersect with.</param>
    /// <returns>A new range representing the intersection.</returns>
    ITickRange<TTickTime> Intersect(ITickRange<TTickTime> other);
    /// <summary>
    /// Returns a new range that represents the union of this range and another range.
    /// </summary>
    /// <param name="other">The other range to union with.</param>
    /// <returns>A new range representing the union.</returns>
    ITickRange<TTickTime> Union(ITickRange<TTickTime> other);
#if NET8_0_OR_GREATER
    /// <summary>
    /// Gets an infinite beat range.
    /// </summary>
    public static abstract ITickRange<TTickTime> Infinity { get; }
    /// <summary>
    /// Gets an empty beat range.
    /// </summary>
    public static abstract ITickRange<TTickTime> Empty { get; }
#endif
}