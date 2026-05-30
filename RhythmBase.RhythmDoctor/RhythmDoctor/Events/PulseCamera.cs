using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents a camera pulse event in a room.
/// </summary>
[JsonObjectSerializable]
public record class PulseCamera : BaseEvent, IRoomEvent
{
	///<inheritdoc/>
	public Room Rooms { get; set; } = new Room([0]);
	/// <summary>
	/// Gets or sets the strength of the pulse.
	/// </summary>
	/// <remarks>
	/// Can only be 1, 2 or 3.
	/// </remarks>
	public int Strength { get; set; } = 1;
	/// <summary>
	/// Gets or sets the count of pulses.
	/// </summary>
	public int Count { get; set; } = 1;
	/// <summary>
	/// Gets or sets the frequency of the pulses.
	/// </summary>
	public float Frequency { get; set; } = 1;
	///<inheritdoc/>
	public override EventType Type => EventType.PulseCamera;
	///<inheritdoc/>
	public override Tab Tab => Tab.Actions;
}
