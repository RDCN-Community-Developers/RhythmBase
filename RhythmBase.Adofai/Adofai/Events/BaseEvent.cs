using RhythmBase.Adofai.Components;
using System.Text.Json;

namespace RhythmBase.Adofai.Events;

/// <summary>
/// Base class for ADOFAI events that carry an <see cref="AngleOffset"/>.
/// These events are addressed by time first: <see cref="TickTime"/> is authoritative and writable,
/// and the owning tile (floor) is derived from it — or, conversely, the event can be placed with a
/// tile plus <see cref="AngleOffset"/> and the tick is calculated back.
/// </summary>
/// <remarks>
/// This type and <see cref="BaseTileEvent"/> are siblings: neither inherits from the other.
/// </remarks>
[JsonObjectHasSerializer(typeof(Serialization.MemberConverter<>))]
public abstract class BaseEvent : IBaseEvent
{
	/// <inheritdoc/>
	public abstract EventType Type { get; }
	/// <summary>
	/// Gets or sets the absolute time of the event. This is the authoritative position.
	/// </summary>
	public TickTime TickTime { get; set; }
	/// <summary>
	/// Sets the authoritative tick and clears the derived floor so it is recomputed from the new tick.
	/// </summary>
	/// <param name="tick">The new absolute time.</param>
	public void SetTick(TickTime tick)
	{
		TickTime = tick;
		_floor = -1;
	}
	/// <summary>
	/// Gets or sets the angle offset (in degrees) applied to the owning tile.
	/// </summary>
	public float AngleOffset { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether the event is active.
	/// </summary>
	public bool Active { get; set; } = true;
	/// <summary>
	/// The floor assigned on export. Only meaningful while serializing; it is recalculated from
	/// <see cref="TickTime"/> rather than being the authoritative position.
	/// </summary>
	internal int _floor = -1;

	internal Dictionary<string, JsonElement> _extraData = [];
	/// <summary>
	/// Returns a string representation of the event type.
	/// </summary>
	/// <returns>A string that represents the event type.</returns>
	public override string ToString() => Type.ToString();
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
