using RhythmBase.BeatBlock.Components;
using System.Text.Json;

namespace RhythmBase.BeatBlock.Events;

public interface IBaseEvent : IEvent<EventType, BBBeat>
{
    public EventType Type { get; }
    public float Time { get; set; }
    public float Angle { get; set; }
    public string? Variant { get; set; }
    public int? Order { get; set; }
    JsonElement this[string propertyName] { get; set; }
}
public interface IChartEvent : IBaseEvent
{
}
public interface IPureEvent : IBaseEvent
{
}