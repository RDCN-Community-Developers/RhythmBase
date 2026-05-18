using RhythmBase.RhythmDoctor.Components;
using System.Text.Json;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents the base interface for an event in the Rhythm Doctor system.
/// </summary>
public interface IBaseEvent : IEvent<EventType, RDBeat>
{
    /// <summary>
    /// Gets or sets a value indicating whether the event is active.
    /// </summary>
    bool Active { get; set; }
    /// <summary>
    /// Gets or sets the beat associated with the event.
    /// </summary>
    new RDBeat Beat { get; set; }
    /// <summary>
    /// Gets or sets the condition associated with the event.
    /// </summary>
    Condition Condition { get; set; }
    /// <summary>
    /// Gets the tab associated with the event.
    /// </summary>
    Tab Tab { get; }
    /// <summary>
    /// Gets or sets the tag associated with the event.
    /// </summary>
    string Tag { get; set; }
    /// <summary>
    /// Gets or sets the run tag associated with the event.
    /// </summary>
    bool RunTag { get; set; }
    /// <summary>
    /// Gets or sets the Y coordinate of the event.
    /// </summary>
    int Y { get; set; }
    /// <summary>
    /// Gets or sets the <see cref="JsonElement"/> associated with the specified property name.
    /// </summary>
    /// <param name="propertyName">The name of the property whose value is to be retrieved or set. The property name is case-sensitive.</param>
    /// <returns></returns>
    JsonElement this[string propertyName] { get; set; }
}