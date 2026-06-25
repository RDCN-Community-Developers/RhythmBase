using System.Text.Json;
using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents a custom event that forwards data and metadata, allowing for dynamic event types and extensible
/// properties.
/// </summary>
[JsonObjectNotSerializable]
[JsonObjectSerializationFallback]
public record class ForwardEvent : BaseEvent, IForwardEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.ForwardEvent;

	/// <inheritdoc/>
	public override Tab Tab => Tab.Unknown;

	///<inheritdoc/>
	public string ActualType { get; private init; } = string.Empty;
	/// <summary>
	/// Gets the collection of additional data associated with the object.
	/// </summary>
	protected Dictionary<string, JsonElement> ExtraData => _extraData;

	/// <summary>
	/// Initializes a new instance of the <see cref="ForwardEvent"/> class.
	/// </summary>
	public ForwardEvent() { }
	/// <summary>
	/// Initializes a new instance of the <see cref="ForwardEvent"/> class using the specified JSON document.
	/// </summary>
	public ForwardEvent(JsonDocument data)
	{
		int _bar = 1;
		float _beat = 0f;
		foreach (var prop in data.RootElement.EnumerateObject())
		{
			if (prop.NameEquals("type"u8)) ActualType = prop.Value.GetString() ?? "";
			else if (prop.NameEquals("beat"u8)) _beat = prop.Value.GetSingle();
			else if (prop.NameEquals("bar"u8)) _bar = prop.Value.GetInt32();
			else if (prop.NameEquals("tag"u8)) Tag = prop.Value.GetString() ?? "";
			else if (prop.NameEquals("active"u8)) Active = prop.Value.GetBoolean();
			else if (prop.NameEquals("runTag"u8)) RunTag = prop.Value.GetBoolean();
			else if (prop.NameEquals("condition"u8)) Condition = Condition.Deserialize(prop.Value.GetString() ?? "");
			else if (prop.NameEquals("y"u8)) Y = prop.Value.GetInt32();
			else _extraData[prop.Name] = prop.Value;
		}
		this._beat = (_bar, _beat);
	}
	/// <inheritdoc/>
	public override string ToString() => $"{TickTime} *{ActualType}";
}
