using RhythmBase.RhythmDoctor.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace RhythmBase.Global.Components;

/// <summary>
/// Represents a range of beats.
/// </summary>
/// <typeparam name="TBeat">The type of beat. Must be a struct and implement <see cref="IBeat{TBeat}"/>.</typeparam>
public interface IBeatRange<TBeat>
    where TBeat : struct, IBeat<TBeat>
{
    /// <summary>
    /// Start beat.
    /// </summary>
    public TBeat? Start { get; }
    /// <summary>
    /// End beat.
    /// </summary>
    public TBeat? End { get; }
    /// <summary>
    /// Determines whether the specified beat is within the range.
    /// </summary>
    /// <param name="b">The beat to check.</param>
    /// <returns>True if the beat is within the range; otherwise, false.</returns>
    public bool Contains(TBeat b);
    /// <summary>
    /// Gets the start and end beats of the range using the specified level.
    /// </summary>
    /// <param name="level">The level to use for resolving the beats.</param>
    /// <returns>A tuple containing the start and end beats.</returns>
    (TBeat Start, TBeat End) GetStartAndEnd(RDLevel level);
    /// <summary>
    /// Returns a new range that represents the intersection of this range and another range.
    /// </summary>
    /// <param name="other">The other range to intersect with.</param>
    /// <returns>A new range representing the intersection.</returns>
    IBeatRange<TBeat> Intersect(IBeatRange<TBeat> other);
    /// <summary>
    /// Returns a new range that represents the union of this range and another range.
    /// </summary>
    /// <param name="other">The other range to union with.</param>
    /// <returns>A new range representing the union.</returns>
    IBeatRange<TBeat> Union(IBeatRange<TBeat> other);
#if NET8_0_OR_GREATER
    /// <summary>
    /// Gets an infinite beat range.
    /// </summary>
    public static IBeatRange<TBeat> Infinity { get; }
    /// <summary>
    /// Gets an empty beat range.
    /// </summary>
    public static IBeatRange<TBeat> Empty { get; }
#endif
}