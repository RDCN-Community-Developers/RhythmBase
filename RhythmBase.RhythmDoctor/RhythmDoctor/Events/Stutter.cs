using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents a stutter event in a room.
/// </summary>
[JsonObjectSerializable]
public record class Stutter : BaseEvent, IRoomEvent
{
	///<inheritdoc/>
	public Room Rooms { get; set; } = new Room([0]);
	/// <summary>
	/// Gets or sets the source beat of the stutter event.
	/// </summary>
	/// <remarks>
	/// Must be a value greater than 0 to avoid potential issues with non-positive values.
	/// </remarks>
	[ValueRange<float>(1f, float.PositiveInfinity)]
	public float SourceBeat
	{
		get;
		set
		{
			if (value < 0)
				value = 1;
			field = value;
		}
	} = 1;
	/// <summary>
	/// Gets or sets the length of the stutter event.
	/// </summary>
	/// <remarks>
	/// Must be a value greater than 0 to avoid potential issues with non-positive values.
	/// </remarks>
	[ValueRange<float>(0f, float.PositiveInfinity)]
	public float Length
	{
		get;
		set
		{
			if (value < 0)
				throw new ArgumentException($"Length must be greater than 0. Provided value: {value}");
			field = value;
		}
	} = 1;
	/// <summary>
	/// Gets or sets the action of the stutter event.
	/// </summary>
	public StutterAction Action { get; set; } = StutterAction.Add;
	/// <summary>
	/// Gets or sets the number of loops for the stutter event.
	/// </summary>
	/// <remarks>
	/// Must be a value greater than 1 to avoid potential issues with non-positive values.
	/// </remarks>
	[ValueRange<int>(1, int.MaxValue)]
	public int Loops
	{
		get;
		set
		{
			if (value < 0)
				throw new ArgumentException($"SourceBeat must be greater than 0. Provided value: {value}");
			field = value;
		}
	} = 1;
	///<inheritdoc/>
	public override EventType Type => EventType.Stutter;
	///<inheritdoc/>
	public override Tab Tab => Tab.Actions;
}