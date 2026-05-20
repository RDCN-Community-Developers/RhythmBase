using RhythmBase.RhythmDoctor.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace RhythmBase.BeatBlock.Components;

/// <summary>
/// Represents a beat in the BeatBlock level format.
/// </summary>
public struct BBBeat : IBeat<BBBeat>
{
    /// <summary>
    /// Gets the time span of the beat.
    /// </summary>
    public TimeSpan TimeSpan { get; }
    /// <summary>
    /// Gets the beat value.
    /// </summary>
    public float BeatOnly { get; }
    /// <summary>
    /// Compares this instance to another <see cref="BBBeat"/>.
    /// </summary>
    /// <param name="other">The other <see cref="BBBeat"/> to compare to.</param>
    /// <returns>A value indicating the relative order of the instances.</returns>
    public int CompareTo(BBBeat other)
    {
        return BeatOnly.CompareTo(other.BeatOnly);
    }
    /// <summary>
    /// Determines whether this instance equals another <see cref="BBBeat"/>.
    /// </summary>
    /// <param name="other">The other <see cref="BBBeat"/> to compare to.</param>
    /// <returns><see langword="true"/> if the instances are equal; otherwise, <see langword="false"/>.</returns>
    public bool Equals(BBBeat other)
    {
        return BeatOnly.Equals(other.BeatOnly);
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="BBBeat"/> struct.
    /// </summary>
    /// <param name="beatOnly">The beat value.</param>
    public BBBeat(float beatOnly)
    {
        TimeSpan = TimeSpan.Zero;
        BeatOnly = beatOnly;
    }
}
public struct BBRange : IBeatRange<BBBeat>
{
    public BBBeat? Start { get; }
    public BBBeat? End { get; }

    public BBRange(BBBeat? start, BBBeat? end)
    {
        Start = start;
        End = end;
    }
    public BBRange(float? start, float? end)
    {
        Start = start.HasValue ? new BBBeat(start.Value) : null;
        End = end.HasValue ? new BBBeat(end.Value) : null;
    }

    public bool Contains(BBBeat b)
    {
        return Start?.CompareTo(b) <= 0 && End?.CompareTo(b) >= 0;
    }

    public (BBBeat Start, BBBeat End) GetStartAndEnd(RDLevel level)
    {
        throw new NotImplementedException();
    }

    public IBeatRange<BBBeat> Intersect(IBeatRange<BBBeat> other)
    {
        throw new NotImplementedException();
    }

    public IBeatRange<BBBeat> Union(IBeatRange<BBBeat> other)
    {
        throw new NotImplementedException();
    }
}