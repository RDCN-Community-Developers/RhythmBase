namespace RhythmBase.RhythmDoctor.Components;


/// <summary>
/// Beat range.
/// </summary>
public struct RDRange : IBeatRange<RDBeat>
{
    /// <summary>
    /// Start beat.
    /// </summary>
    public RDBeat? Start { get; }
    /// <summary>
    /// End beat.
    /// </summary>
    public RDBeat? End { get; }
    /// <summary>
    /// Beat interval.
    /// </summary>
    public readonly float BeatInterval
    {
        get
        {
            float BeatInterval = Start is RDBeat startNotNull && End is RDBeat endNotNull
                ? endNotNull.BeatOnly - startNotNull.BeatOnly
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
                if (Start!.Value.BeatOnly == End!.Value.BeatOnly)
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
    public RDRange(RDBeat? start, RDBeat? end)
    {
        this = default;
        if (start != null && end != null && !((RDBeat)start).FromSameLevelOrNull((RDBeat)end))
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
    public readonly bool Contains(RDBeat b) => (Start == null || Start <= b) && (End == null || b < End);
    readonly IBeatRange<RDBeat> IBeatRange<RDBeat>.Intersect(IBeatRange<RDBeat> other) => Intersect((RDRange)other);
    readonly IBeatRange<RDBeat> IBeatRange<RDBeat>.Union(IBeatRange<RDBeat> other) => Union((RDRange)other);
    /// <summary>
    /// Computes the intersection of the current range with another specified range.
    /// </summary>
    /// <remarks>The intersection of two ranges is the range that contains all elements common to both ranges.  If
    /// either range is unbounded (i.e., has a null start or end), the resulting range will reflect  the bounds of the
    /// other range where applicable. If the resulting range is invalid  (i.e., the start is greater than the end), an
    /// empty range is returned.</remarks>
    /// <param name="other">The range to intersect with the current range.</param>
    /// <returns>A new <see cref="RDRange"/> representing the intersection of the two ranges.  If the ranges do not overlap,
    /// returns an empty range.</returns>
    public readonly RDRange Intersect(RDRange other)
    {
        RDBeat? newStart;
        if (Start == null || (other.Start != null && other.Start > Start))
            newStart = other.Start;
        else
            newStart = Start;
        RDBeat? newEnd;
        if (End == null || (other.End != null && other.End < End))
            newEnd = other.End;
        else
            newEnd = End;
        return newStart != null && newEnd != null && newStart > newEnd ? Empty : new RDRange(newStart, newEnd);
    }
    /// <summary>
    /// Creates a new <see cref="RDRange"/> that represents the union of the current range and the specified range.
    /// </summary>
    /// <remarks>The union operation considers null values for the start or end of a range as unbounded.  If both
    /// ranges have null start or end values, the resulting range will also have null for those bounds.</remarks>
    /// <param name="other">The <see cref="RDRange"/> to combine with the current range.</param>
    /// <returns>A new <see cref="RDRange"/> that spans from the earliest start point to the latest end point of the two ranges. If
    /// either range has a null start or end, the resulting range will use the non-null value, if available.</returns>
    public readonly RDRange Union(RDRange other)
    {
        RDBeat? newStart;
        if (Start == null || (other.Start != null && other.Start > Start))
            newStart = Start;
        else
            newStart = other.Start;
        RDBeat? newEnd;
        if (End == null || (other.End != null && other.End < End))
            newEnd = End;
        else
            newEnd = other.End;
        return new RDRange(newStart, newEnd);
    }
    /// <summary>
    /// Gets the start and end beats for the current range, using the specified level's default values if not explicitly
    /// set.
    /// </summary>
    /// <param name="level">The level from which to retrieve the default start beat and length if the current range does not specify them.</param>
    /// <returns>A tuple containing the start and end beats. The start beat is set to the current range's start value if defined;
    /// otherwise, it defaults to the level's default beat. The end beat is set to the current range's end value if
    /// defined; otherwise, it defaults to the level's length.</returns>
    public readonly (RDBeat Start, RDBeat End) GetStartAndEnd(RDLevel level) => (Start?.WithLinkIfNull(level) ?? level.DefaultBeat, End?.WithLinkIfNull(level) ?? level.Duration);
    /// <summary>
    /// Gets a range that represents an infinite range with no upper or lower bounds.
    /// </summary>
    /// <remarks>This property can be used to represent a range that is unbounded in both directions.</remarks>
    public static RDRange Infinity => new(null, null);
    /// <summary>
    /// Gets an empty range with no defined start or end values.
    /// </summary>
    public static RDRange Empty => new(new(), new());
#if NET8_0_OR_GREATER
    public static IBeatRange<RDBeat> CreateRange(RDBeat? start, RDBeat? end)
    {
        return new RDRange(start, end);
    }
#endif
}
