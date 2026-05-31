using RhythmBase.Adofai.Components;
using RhythmBase.Adofai.Converters;
using System.Text.Json;

namespace RhythmBase.Adofai.Events;

/// <summary>
/// Represents the base class for all ADOFAI events.
/// </summary>
public abstract class BaseEvent : IBaseEvent
{
	/// <inheritdoc/>
	public abstract EventType Type { get; }
    /// <summary>
    /// Gets the beat at which the event occurs.
    /// </summary>
    public TickTime TickTime { get; }

    internal Dictionary<string, JsonElement> _extraData = [];
	/// <summary>
	/// Returns a string representation of the event type.
	/// </summary>
	/// <returns>A string that represents the event type.</returns>
	public override string ToString() => Type.ToString();
	/// <summary>
	/// Gets or sets the <see cref="JsonElement"/> associated with the specified key.
	/// </summary>
	/// <remarks>When setting a value, if the specified key already exists, the value is replaced.  If the key
	/// does not exist, a new key-value pair is added.</remarks>
	/// <param name="key">The key of the element to get or set. The key is case-sensitive.</param>
	/// <returns></returns>
	public JsonElement this[string key]
	{
		get => _extraData.ContainsKey(key) ? _extraData[key] : new JsonElement();
		set => _extraData[key] = value;
	}
}
