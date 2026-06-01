using RhythmBase.RhythmDoctor.Components;
using System.Text.Json;
namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event that forwards a row action within the system, providing access to event type, associated tab,
/// and extensible event data.
/// </summary>
[JsonObjectNotSerializable]
public record class ForwardRowEvent : BaseRowAction, IForwardEvent
{
    ///<inheritdoc/>
    public override EventType Type => EventType.ForwardRowEvent;

    ///<inheritdoc/>
    public string ActualType
    {
        get => _extraData.TryGetValue("type", out JsonElement typeElement) && typeElement.ValueKind == JsonValueKind.String ?
                typeElement.GetString() ?? "" : "";
        set => _extraData["type"] = JsonElement.Parse(value);
    }

    ///<inheritdoc/>
    public override Tab Tab => Tab.Rows;

    /// <summary>
    /// Gets a dictionary containing additional data not explicitly modeled by the class.
    /// </summary>
    protected Dictionary<string, JsonElement> ExtraData => _extraData;

    /// <summary>
    /// Initializes a new instance of the <see cref="ForwardRowEvent"/> class.
    /// </summary>
    public ForwardRowEvent()
    {
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ForwardRowEvent"/> class with the specified data.
    /// </summary>
    /// <param name="data">The data for the event.</param>
    public ForwardRowEvent(JsonDocument data)
    {
        foreach (var kvp in data.RootElement.EnumerateObject())
            _extraData[kvp.Name] = kvp.Value;
        base.TickTime =
            _extraData.TryGetValue("bar", out JsonElement barElement) && barElement.ValueKind == JsonValueKind.Number ?
            _extraData.TryGetValue("beat", out JsonElement beatElement) && beatElement.ValueKind == JsonValueKind.Number ?
            new(barElement.GetInt32(), beatElement.GetSingle()) : new(barElement.GetInt32(), 1) : default;
        Tag = _extraData.TryGetValue("tag", out JsonElement tagElement) && tagElement.ValueKind == JsonValueKind.String ?
            tagElement.GetString() ?? "" : "";
        Active = (!_extraData.TryGetValue("active", out JsonElement activeElement) || activeElement.ValueKind != JsonValueKind.True) && activeElement.ValueKind != JsonValueKind.False || activeElement.GetBoolean();
        RunTag = (_extraData.TryGetValue("runTag", out JsonElement runTagElement) && runTagElement.ValueKind == JsonValueKind.True || runTagElement.ValueKind == JsonValueKind.False) && runTagElement.GetBoolean();
        Condition = _extraData.TryGetValue("condition", out JsonElement conditionElement) && conditionElement.ValueKind == JsonValueKind.String ?
            Condition.Deserialize(conditionElement.GetString() ?? "") : new();
        _row = _extraData.TryGetValue("row", out JsonElement rowElement) && rowElement.ValueKind == JsonValueKind.Number ?
            rowElement.GetInt32() : -1;
        _extraData.Remove("bar");
        _extraData.Remove("beat");
        _extraData.Remove("tag");
        _extraData.Remove("active");
        _extraData.Remove("runTag");
        _extraData.Remove("condition");
        _extraData.Remove("row");
    }
    ///<inheritdoc/>
    public override string ToString() => $"{TickTime} *{ActualType}";
}
