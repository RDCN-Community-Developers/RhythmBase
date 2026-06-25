using RhythmBase.BeatBlock.Components;
using System.Text.Json;

namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Represents a forward-compatible event for unrecognized event types.
/// </summary>
[JsonObjectSerializationFallback]
public record class ForwardEvent : BaseEvent, IForwardEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.ForwardEvent;
    /// <summary>
    /// Gets or sets the actual event type string.
    /// </summary>
    public string ActualType
    {
        get => _extraData.TryGetValue("type", out JsonElement typeElement) && typeElement.ValueKind == JsonValueKind.String ?
                typeElement.GetString() ?? "" : "";
        set => _extraData["type"] = JsonElement.Parse($"\"{value}\"");
    }
    /// <summary>
    /// Gets the extra data dictionary.
    /// </summary>
    protected Dictionary<string, JsonElement> ExtraData => _extraData;
    /// <summary>
    /// Initializes a new instance of the <see cref="ForwardEvent"/> class.
    /// </summary>
    public ForwardEvent() { }
    /// <summary>
    /// Initializes a new instance of the <see cref="ForwardEvent"/> class from the specified JSON document.
    /// </summary>
    /// <param name="data">The JSON document containing the event data.</param>
    public ForwardEvent(JsonDocument data)
    {
        this.Order = _extraData.TryGetValue("order", out JsonElement orderElement) && orderElement.ValueKind == JsonValueKind.Number ? orderElement.GetInt32() : 0;
        this.Angle = _extraData.TryGetValue("angle", out JsonElement angleElement) && angleElement.ValueKind == JsonValueKind.Number ? angleElement.GetSingle() : 0;
        //this.Beat = _extraData.TryGetValue("beat", out JsonElement beatElement) && beatElement.ValueKind == JsonValueKind.Number ? beatElement.GetSingle() : 0;
        this.TickTime = _extraData.TryGetValue("time", out JsonElement timeElement) && timeElement.ValueKind == JsonValueKind.Number ? new TickTime(timeElement.GetSingle()) : new TickTime(0);
        this.Variant = _extraData.TryGetValue("variant", out JsonElement variantElement) && variantElement.ValueKind == JsonValueKind.String ? variantElement.GetString() ?? "" : "";
        _extraData["type"] = data.RootElement.GetProperty("type");
        _extraData.Remove("order");
        _extraData.Remove("angle");
        _extraData.Remove("time");
        _extraData.Remove("variant");
    }
}
