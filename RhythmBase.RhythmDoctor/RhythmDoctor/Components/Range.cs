namespace RhythmBase.RhythmDoctor.Components;


/// <summary>
/// Beat range.
/// </summary>
public struct Range : ITickRange<TickTime>
{
    /// <summary>
    /// Start beat.
    /// </summary>
    public TickTime? Start { get; }
    /// <summary>
    /// End beat.
    /// </summary>
    public TickTime? End { get; }
    /// <summary>
    /// Beat interval.
    /// </summary>
    public readonly float BeatInterval
    {
        get
        {
            float BeatInterval = Start is TickTime startNotNull && End is TickTime endNotNull
                ? endNotNull.Tick - startNotNull.Tick
                : float.PositiveInfinity;
            return BeatInterval;
        }
    }
    /// <summary>
    /// Time interval.
    /// </summary>
    /// <returns></returns>
    public readonly TimeSpan TimeInterval
    {
        get
        {
            bool flag = Start != null && End != null;
            TimeSpan TimeInterval;
            if (flag)
            {
                if (Start!.Value.Tick == End!.Value.Tick)
                {
                    TimeInterval = TimeSpan.Zero;
                }
                else
                {
                    TimeInterval = End.Value.TimeSpan - Start.Value.TimeSpan;
                }
            }
            else
            {
                TimeInterval = TimeSpan.MaxValue;
            }
            return TimeInterval;
        }
    }
    /// <param name="start">Start beat.</param>
    /// <param name="end">End beat.</param>
    public Range(TickTime? start, TickTime? end)
    {
        this = default;
        if (start != null && end != null && !((TickTime)start).FromSameLevelOrNull((TickTime)end))
        {
            throw new RhythmBaseException("RDIndexes must come from the same RDLevel.");
        }
        if (start != null && end != null && start > end)
        {
            Start = end;
            End = start;
        }
        else
        {
            Start = start;
            End = end;
        }
    }
    /// <summary>
    /// Determines whether the specified beat is within the range.
    /// </summary>
    /// <param name="b">The beat to check.</param>
    /// <returns>True if the beat is within the range; otherwise, false.</returns>
    public readonly bool Contains(TickTime b) => (Start == null || Start <= b) && (End == null || b < End);
    readonly ITickRange<TickTime> ITickRange<TickTime>.Intersect(ITickRange<TickTime> other) => Intersect((Range)other);
    readonly ITickRange<TickTime> ITickRange<TickTime>.Union(ITickRange<TickTime> other) => Union((Range)other);
    /// <summary>
    /// Computes the intersection of the current range with another specified range.
    /// </summary>
    /// <remarks>The intersection of two ranges is the range that contains all elements common to both ranges.  If
    /// either range is unbounded (i.e., has a null start or end), the resulting range will reflect  the bounds of the
    /// other range where applicable. If the resulting range is invalid  (i.e., the start is greater than the end), an
    /// empty range is returned.</remarks>
    /// <param name="other">The range to intersect with the current range.</param>
    /// <returns>A new <see cref="Range"/> representing the intersection of the two ranges.  If the ranges do not overlap,
    /// returns an empty range.</returns>
    public readonly Range Intersect(Range other)
    {
        TickTime? newStart;
        if (Start == null || (other.Start != null && other.Start > Start))
            newStart = other.Start;
        else
            newStart = Start;
        TickTime? newEnd;
        if (End == null || (other.End != null && other.End < End))
            newEnd = other.End;
        else
            newEnd = End;
        return newStart != null && newEnd != null && newStart > newEnd ? Empty : new Range(newStart, newEnd);
    }
    /// <summary>
    /// Creates a new <see cref="Range"/> that represents the union of the current range and the specified range.
    /// </summary>
    /// <remarks>The union operation considers null values for the start or end of a range as unbounded.  If both
    /// ranges have null start or end values, the resulting range will also have null for those bounds.</remarks>
    /// <param name="other">The <see cref="Range"/> to combine with the current range.</param>
    /// <returns>A new <see cref="Range"/> that spans from the earliest start point to the latest end point of the two ranges. If
    /// either range has a null start or end, the resulting range will use the non-null value, if available.</returns>
    public readonly Range Union(Range other)
    {
        TickTime? newStart;
        if (Start == null || (other.Start != null && other.Start > Start))
            newStart = Start;
        else
            newStart = other.Start;
        TickTime? newEnd;
        if (End == null || (other.End != null && other.End < End))
            newEnd = End;
        else
            newEnd = other.End;
        return new Range(newStart, newEnd);
    }
    /// <summary>
    /// Gets the start and end beats for the current range, using the specified level's default values if not explicitly
    /// set.
    /// </summary>
    /// <param name="level">The level from which to retrieve the default start beat and length if the current range does not specify them.</param>
    /// <returns>A tuple containing the start and end beats. The start beat is set to the current range's start value if defined;
    /// otherwise, it defaults to the level's default beat. The end beat is set to the current range's end value if
    /// defined; otherwise, it defaults to the level's length.</returns>
    public readonly (TickTime Start, TickTime End) GetStartAndEnd(TickTime endTime)
        => (Start is TickTime startNotNull ? startNotNull : new(1), End is TickTime endNotNull ? endNotNull : endTime);
    /// <summary>
    /// Gets a range that represents an infinite range with no upper or lower bounds.
    /// </summary>
    /// <remarks>This property can be used to represent a range that is unbounded in both directions.</remarks>
    public static Range Infinity => new(null, null);
    /// <summary>
    /// Gets an empty range with no defined start or end values.
    /// </summary>
    public static Range Empty => new(new(), new());
#if NET8_0_OR_GREATER
    /// <summary>
    /// Creates a new <see cref="Range"/> from the specified start and end beats.
    /// </summary>
    /// <param name="start">The start beat.</param>
    /// <param name="end">The end beat.</param>
    /// <returns>A new <see cref="Range"/> instance.</returns>
    public static ITickRange<TickTime> CreateRange(TickTime? start, TickTime? end)
    {
        return new Range(start, end);
    }
#endif
}
