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
		foreach (var prop in data.RootElement.EnumerateObject())
		{
			if (prop.NameEquals("type"u8)) ActualType = prop.Value.GetString() ?? "";
			else if (prop.NameEquals("order"u8)) Order = prop.Value.GetInt32();
			else if (prop.NameEquals("angle"u8)) Angle = prop.Value.GetSingle();
			else if (prop.NameEquals("time"u8)) _beat = new(prop.Value.GetSingle());
			else if (prop.NameEquals("variant"u8)) Variant = prop.Value.GetString() ?? "";
			else _extraData[prop.Name] = prop.Value;
		}
	}
	/// <inheritdoc/>
	public override string ToString() => $"{TickTime} *{ActualType}";
}
