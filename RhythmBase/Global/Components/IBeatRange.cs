using RhythmBase.RhythmDoctor.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace RhythmBase.Global.Components;

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
    (TBeat Start, TBeat End) GetStartAndEnd(RDLevel level);
    IBeatRange<TBeat> Intersect(IBeatRange<TBeat> other);
    IBeatRange<TBeat> Union(IBeatRange<TBeat> other);
#if NET8_0_OR_GREATER
    public static IBeatRange<TBeat> Infinity { get; }
    public static IBeatRange<TBeat> Empty { get; }
#endif
}