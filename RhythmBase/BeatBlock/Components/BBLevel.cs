using RhythmBase.BeatBlock.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace RhythmBase.BeatBlock.Components;

public class OrderedEventCollection { 
    public List<Events.BaseEvent> Events { get; set; } = new List<Events.BaseEvent>();
}
public partial class BBLevel : IJsonLevel<BBLevel, BeatCalculator>, IDisposable
{
    public static BBLevel Default => new BBLevel();
    public Properties Properties { get; set; }
    public int Count { get; }
    public string ResolvedPath { get; }
    public string? Filepath { get; }
    public string? ResolvedDirectory { get; }
    public BeatCalculator Calculator { get; }
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
public class BeatCalculator : IBeatCalculator<BeatCalculator, BBLevel>
{
    public void CalculateBeats(BBLevel level)
    {
        // Implement beat calculation logic here
    }
}
