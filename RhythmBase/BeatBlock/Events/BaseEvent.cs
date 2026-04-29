using System.Text.Json;

namespace RhythmBase.BeatBlock.Events;

public abstract record class BaseEvent : IBaseEvent
{
    public abstract EventType Type { get; }
    public float Time { get; set; }
    public float Angle { get; set; }
    public string? Variant { get; set; }
    public int? Order { get; set; }
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
