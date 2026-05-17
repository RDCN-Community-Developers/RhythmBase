using RhythmBase.Adofai.Components.Filters;
using RhythmBase.BeatBlock.Events;
using RhythmBase.RhythmDoctor.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace RhythmBase.BeatBlock.Components;

public partial class BBLevel : OrderedEventCollection<IBaseEvent, EventType, BBBeat>, IJsonLevel<BBLevel, IBaseEvent, EventType, BBBeat>, IDisposable
{
    public static BBLevel Default => new BBLevel();
    public Properties Properties { get; set; }
    public int Count { get; }
    public string ResolvedPath { get; }
    public string? Filepath { get; }
    public string? ResolvedDirectory { get; }
    IBeatCalculator<BBBeat> ILevel<BBLevel, IBaseEvent, EventType, BBBeat>.Calculator => Calculator;
    public BeatCalculator Calculator { get; }
    internal override BBBeat CreateInstance(float beat) => new BBBeat(beat);
    internal override IBeatRange<BBBeat> CreateRange(float? start, float? end)
    {
        throw new NotImplementedException();
    }
    internal override ReadOnlyEnumCollection<EventType> Types => throw new NotImplementedException();
    internal override ReadOnlyEnumCollection<EventType> TypesOf<TTarget>()
    {
        throw new NotImplementedException();
    }
    public bool Add(IBaseEvent e)
    {
        throw new NotImplementedException();
    }
    public bool Remove(IBaseEvent e)
    {
        throw new NotImplementedException();
    }
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
public class BeatCalculator : IBeatCalculator<BBBeat>
{
    public BBBeat BeatOf(float beatOnly)
    {
        throw new NotImplementedException();
    }

    public BBBeat BeatOf(TimeSpan timeSpan)
    {
        throw new NotImplementedException();
    }

    public TimeSpan BeatOnlyToTimeSpan(float beat)
    {
        throw new NotImplementedException();
    }

    public float BeatsPerMinuteOf(BBBeat beat)
    {
        throw new NotImplementedException();
    }

    public void CalculateBeats(BBLevel level)
    {
        // Implement beat calculation logic here
    }

    public IBeatRange<BBBeat> IntervalOf(BBBeat beat1, BBBeat beat2)
    {
        throw new NotImplementedException();
    }

    public IBeatRange<BBBeat> IntervalOf(TimeSpan timeSpan1, TimeSpan timeSpan2)
    {
        throw new NotImplementedException();
    }

    public void Refresh()
    {
        throw new NotImplementedException();
    }

    public float TimeSpanToBeatOnly(TimeSpan timeSpan)
    {
        throw new NotImplementedException();
    }
}
