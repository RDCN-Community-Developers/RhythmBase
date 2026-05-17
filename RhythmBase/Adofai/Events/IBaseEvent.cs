using RhythmBase.Adofai.Components;
using System.Text.Json;

namespace RhythmBase.Adofai.Events;

/// <summary>
/// Represents the base interface for all Adofai event types.
/// Provides access to the event type and dynamic event data via indexer.
/// </summary>
public interface IBaseEvent : IEvent<EventType, ADBeat>
{
	/// <summary>
	/// Gets or sets the value associated with the specified key in the event data.
	/// </summary>
	/// <param name="key">The key of the event data to access.</param>
	/// <returns>The <see cref="JsonElement"/> value associated with the specified key.</returns>
	JsonElement this[string key] { get; set; }
}