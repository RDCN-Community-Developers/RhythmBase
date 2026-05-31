using System.Text.Json;

namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents a custom event in the ADOFAI game.  
/// </summary>  
[JsonObjectSerializationFallback]
public class ForwardEvent : BaseEvent, IBaseEvent, IForwardEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.ForwardEvent;
	/// <summary>  
	/// Gets the actual type of the custom event from the event data.  
	/// </summary>  
	public string ActualType
	{
		get => _extraData.TryGetValue("eventType", out JsonElement typeElement) ? typeElement.ToString() ?? "UnknownType" : "UnknownType";
		set => _extraData["eventType"] = JsonDocument.Parse($"\"{value}\"").RootElement;
	}
	/// <summary>
	/// Initializes a new instance of the <see cref="ForwardEvent"/> class.
	/// </summary>
	public ForwardEvent() { }
	/// <summary>
	/// Initializes a new instance of the <see cref="ForwardEvent"/> class from a <see cref="JsonDocument"/>.
	/// </summary>
	/// <param name="doc">The <see cref="JsonDocument"/> containing event data.</param>
	public ForwardEvent(JsonDocument doc)
	{

	}
	/// <summary>  
	/// Gets or sets the data associated with the custom event.  
	/// </summary>  
	public JsonElement Data { get; set; } = new();
	/// <summary>  
	/// Returns a string representation of the custom event type.  
	/// </summary>  
	/// <returns>A string that represents the custom event type.</returns>  
	public override string ToString() => ActualType;
}
