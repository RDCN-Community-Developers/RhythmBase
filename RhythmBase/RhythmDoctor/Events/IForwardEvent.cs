using System.Text.Json;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents a forward event that can store arbitrary key-value pairs and an actual type identifier.
/// </summary>
public interface IForwardEvent : IBaseEvent, Global.Events.IForwardEvent
{
	/// <summary>
	/// Gets or sets the actual type of the forward event.
	/// </summary>
	string ActualType { get; set; }

	/// <summary>
	/// Gets or sets the value associated with the specified key in the event's data.
	/// </summary>
	/// <param name="key">The key of the value to get or set.</param>
	/// <returns>The <see cref="JsonElement"/> value associated with the specified key.</returns>
	new JsonElement this[string key] { get; set; }

	/// <summary>
	/// Returns a string that represents the current forward event.
	/// </summary>
	/// <returns>A string representation of the forward event.</returns>
	string ToString();
}