using RhythmBase.RhythmDoctor.Components;
using System.Text.Json;
namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents a forward event for a decoration action, providing access to event-specific data and metadata used in
/// decoration processing.
/// </summary>
[JsonObjectNotSerializable]
public record class ForwardDecorationEvent : BaseDecorationAction, IForwardEvent
{
	/// <inheritdoc />
	public override EventType Type => EventType.ForwardDecorationEvent;
	///<inheritdoc/>
	public string ActualType { get; private init; } = string.Empty;
	/// <inheritdoc />
	public override Tab Tab => Tab.Decorations;

	/// <summary>
	/// Gets a dictionary containing additional data not explicitly modeled by the class.
	/// </summary>
	protected Dictionary<string, JsonElement> ExtraData => _extraData;
	/// <summary>
	/// Initializes a new instance of the <see cref="ForwardDecorationEvent"/> class.
	/// </summary>
	public ForwardDecorationEvent() { }
	/// <summary>
	/// Initializes a new instance of the <see cref="ForwardDecorationEvent"/> class with the specified data.
	/// </summary>
	public ForwardDecorationEvent(JsonDocument data)
    {
        foreach (var kvp in data.RootElement.EnumerateObject())
            _extraData[kvp.Name] = kvp.Value;
        TickTime =
			_extraData.TryGetValue("bar", out JsonElement barElement) && barElement.ValueKind == JsonValueKind.Number ?
			_extraData.TryGetValue("beat", out JsonElement beatElement) && beatElement.ValueKind == JsonValueKind.Number ?
			new(barElement.GetInt32(), beatElement.GetSingle()) : new(barElement.GetInt32(), 1) : default;
		Tag = _extraData.TryGetValue("tag", out JsonElement tagElement) && tagElement.ValueKind == JsonValueKind.String ?
			tagElement.GetString() ?? "" : "";
		Active = (!_extraData.TryGetValue("active", out JsonElement activeElement) || activeElement.ValueKind != JsonValueKind.True) && activeElement.ValueKind != JsonValueKind.False || activeElement.GetBoolean();
		RunTag = (_extraData.TryGetValue("runTag", out JsonElement runTagElement) && runTagElement.ValueKind == JsonValueKind.True || runTagElement.ValueKind == JsonValueKind.False) && runTagElement.GetBoolean();
		Condition = _extraData.TryGetValue("condition", out JsonElement conditionElement) && conditionElement.ValueKind == JsonValueKind.String ?
			Condition.Deserialize(conditionElement.GetString() ?? "") : new();
		_decoId = _extraData.TryGetValue("target", out JsonElement targetElement) && targetElement.ValueKind == JsonValueKind.String ?
			targetElement.GetString() ?? "" : "";
		_extraData.Remove("bar");
		_extraData.Remove("beat");
		_extraData.Remove("tag");
		_extraData.Remove("active");
		_extraData.Remove("runTag");
		_extraData.Remove("condition");
		_extraData.Remove("target");
	}
	/// <inheritdoc />
	public override string ToString() => $"{TickTime} *{ActualType}";
}
