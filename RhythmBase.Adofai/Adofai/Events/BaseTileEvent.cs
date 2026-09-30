using RhythmBase.Adofai.Components;
using RhythmBase.Adofai.Serialization;
using System.Text.Json;

namespace RhythmBase.Adofai.Events;

/// <summary>
/// Base class for ADOFAI events that are bound to a floor (tile) and have no <c>angleOffset</c>.
/// The owning <see cref="Tile"/> is authoritative: <see cref="TickTime"/> is read-only and is
/// derived from the tile, so inserting or changing preceding tiles refreshes it automatically.
/// </summary>
/// <remarks>
/// This type and <see cref="BaseEvent"/> are siblings: neither inherits from the other.
/// </remarks>
[JsonObjectHasSerializer(typeof(MemberConverterBaseTileEvent<>))]
public abstract class BaseTileEvent : IBaseEvent
{
	/// <inheritdoc/>
	public abstract EventType Type { get; }
	/// <summary>
	/// Gets or sets the parent tile associated with this event.
	/// </summary>
	public Tile? Parent { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether the event is active.
	/// </summary>
	public bool Active { get; set; } = true;
	/// <summary>
	/// Gets the absolute time of the event. When the event is attached to a tile the tile's beat is
	/// authoritative; <paramref name="value"/> is only kept as a fallback before attachment.
	/// </summary>
	public TickTime TickTime
	{
		get => Parent is { Parent: not null } ? Parent.Beat : _tickTime;
		set => _tickTime = value;
	}
	private TickTime _tickTime;
	internal int _floor = -1;
	internal Dictionary<string, JsonElement> _extraData = [];
	/// <summary>
	/// Returns a string representation of the event type.
	/// </summary>
	/// <returns>A string that represents the event type.</returns>
	public override string ToString() => $"{Type}";
	/// <summary>
	/// Gets or sets the <see cref="JsonElement"/> associated with the specified key.
	/// </summary>
	/// <param name="key">The key of the element to get or set. The key is case-sensitive.</param>
	/// <returns>The stored element, or an empty <see cref="JsonElement"/> when missing.</returns>
	public JsonElement this[string key]
	{
		get => _extraData.ContainsKey(key) ? _extraData[key] : new JsonElement();
		set => _extraData[key] = value;
	}
}
