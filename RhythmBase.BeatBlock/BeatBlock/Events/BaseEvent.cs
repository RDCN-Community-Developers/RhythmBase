using RhythmBase.BeatBlock.Components;
using System.Text.Json;

namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Represents the base class for all BeatBlock events.
/// </summary>
[JsonObjectSerializable]
public abstract record class BaseEvent : IBaseEvent
{
    /// <summary>
    /// Gets the type of the event.
    /// </summary>
    public abstract EventType Type { get; }
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
    /// Gets the beat of the event.
    /// </summary>
    public TickTime TickTime { get; }
    /// <summary>
    /// Gets or sets additional data associated with the specified property name.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>The <see cref="JsonElement"/> value associated with the property.</returns>
    public JsonElement this[string propertyName]
    {
        get => _extraData.TryGetValue(propertyName, out JsonElement value) ? value : default;
        set
        {
            if (value.ValueKind == JsonValueKind.Undefined)
                _extraData.Remove(propertyName);
            else
                _extraData[propertyName] = value;
        }
    }
    internal Dictionary<string, JsonElement> _extraData = [];
}
