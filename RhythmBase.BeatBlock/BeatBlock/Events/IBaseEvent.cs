using RhythmBase.BeatBlock.Components;
using System.Text.Json;

namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Represents an event in a BeatBlock level.
/// </summary>
public interface IBaseEvent : IEvent<EventType, TickTime>
{
    /// <summary>
    /// Gets or sets the time of the event.
    /// </summary>
    public float Time { get; set; }
    /// <summary>
    /// Gets or sets the angle of the event.
    /// </summary>
    public float Angle { get; set; }
    /// <summary>
    /// Gets or sets the variant of the event.
    /// </summary>
    public string? Variant { get; set; }
    /// <summary>
    /// Gets or sets the order of the event.
    /// </summary>
    public int? Order { get; set; }
    /// <summary>
    /// Gets or sets additional data associated with the specified property name.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>The <see cref="JsonElement"/> value associated with the property.</returns>
    JsonElement this[string propertyName] { get; set; }
}
/// <summary>
/// Represents a chart event in a BeatBlock level.
/// </summary>
public interface IChartEvent : IBaseEvent
{
}
/// <summary>
/// Represents a pure event in a BeatBlock level.
/// </summary>
public interface IPureEvent : IBaseEvent
{
}