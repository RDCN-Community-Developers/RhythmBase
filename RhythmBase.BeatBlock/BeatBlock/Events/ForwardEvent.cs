using System.Text.Json;

namespace RhythmBase.BeatBlock.Events;

[JsonObjectSerializationFallback]
public record class ForwardEvent : BaseEvent, IForwardEvent
{
    public override EventType Type => EventType.ForwardEvent;
    public string ActualType
    {
        get => _extraData.TryGetValue("type", out JsonElement typeElement) && typeElement.ValueKind == JsonValueKind.String ?
                typeElement.GetString() ?? "" : "";
        set => _extraData["type"] = JsonElement.Parse($"\"{value}\"");
    }
    protected Dictionary<string, JsonElement> ExtraData => _extraData;
    public ForwardEvent() { }
    public ForwardEvent(JsonDocument data)
    {
        this.Order = _extraData.TryGetValue("order", out JsonElement orderElement) && orderElement.ValueKind == JsonValueKind.Number ? orderElement.GetInt32() : 0;
        this.Angle = _extraData.TryGetValue("angle", out JsonElement angleElement) && angleElement.ValueKind == JsonValueKind.Number ? angleElement.GetSingle() : 0;
        //this.Beat = _extraData.TryGetValue("beat", out JsonElement beatElement) && beatElement.ValueKind == JsonValueKind.Number ? beatElement.GetSingle() : 0;
        this.Time = _extraData.TryGetValue("time", out JsonElement timeElement) && timeElement.ValueKind == JsonValueKind.Number ? timeElement.GetSingle() : 0;
        this.Variant = _extraData.TryGetValue("variant", out JsonElement variantElement) && variantElement.ValueKind == JsonValueKind.String ? variantElement.GetString() ?? "" : "";
        _extraData["type"] = data.RootElement.GetProperty("type");
        _extraData.Remove("order");
        _extraData.Remove("angle");
        _extraData.Remove("time");
        _extraData.Remove("variant");
    }
}
