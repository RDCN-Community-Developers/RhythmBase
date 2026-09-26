using System.Collections;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
// Index type: 0-based room ordinal in the range 0..RoomCapacity.
using _it = System.Byte;
// Storage type: bitmask where bit i represents the room at index i.
using _dt = System.UInt32;
namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Represents a room that can be applied to multiple rooms.
/// </summary>
[CollectionBuilder(typeof(CollectionBuilders), nameof(CollectionBuilders.BuildRoom))]
public struct Room :
#if NET7_0_OR_GREATER
	IEqualityOperators<Room, Room, bool>,
#endif
	IEquatable<Room>, IEnumerable<_it>
{
	/// <summary>
	/// The number of valid rooms, including the top room; valid indices are 0..<see cref="RoomCapacity"/>.
	/// </summary>
	private const int TotalRoomCount = RoomCapacity + 1;
	/// <summary>
	/// The bitmask of valid rooms, where bit i represents index i.
	/// </summary>
	private const _dt ValidRoomsMask = ((_dt)1 << TotalRoomCount) - 1;
	/// <summary>
	/// Gets or sets whether the specified room is enabled. 0-base.
	/// </summary>
	/// <param name="index">The index of the room.</param>
	/// <returns>True if the room is enabled; otherwise, false.</returns>
	[IndexerName("Room")]
	public bool this[_it index]
	{
		readonly get => (_data & ((_dt)1 << index)) != 0;
		set
		{
			if (index <= RoomCapacity)
				_data = value ? (_data | ((_dt)1 << index)) : (_data & ~((_dt)1 << index));
		}
	}
	/// <summary>
	/// Gets or sets whether the specified room is enabled.
	/// </summary>
	/// <param name="index">The room flag.</param>
	/// <returns>True if the room is enabled; otherwise, false.</returns>
	[IndexerName("Room")]
	public bool this[RoomIndex index]
	{
		readonly get => (_data & (_dt)index) != 0;
		set
		{
			if (index != RoomIndex.None && ((_dt)index & ~ValidRoomsMask) == 0)
				_data = value ? (_data | (_dt)index) : (_data & ~(_dt)index);
		}
	}
	/// <summary>
	/// Gets the list of enabled rooms as 0-base indices.
	/// </summary>
	public readonly _it[] Rooms
	{
		get
		{
			_dt indexes = _data;
			return [..Enumerable
				.Range(0, TotalRoomCount)
				.Where(x => (indexes & ((_dt)1 << x)) != 0)
				.Select(x => (_it)x)];
		}
	}
	/// <inheritdoc/>
	public readonly override string ToString() => $"[{string.Join(",", Rooms)}]";
	/// <summary>
	/// Returns an instance with only room 1 enabled.
	/// </summary>
	/// <returns>An instance with only room 1 enabled.</returns>
	public static Room Default => new() { _data = 0b1, };
	/// <summary>
	/// Returns an instance with every room enabled.
	/// </summary>
	/// <returns>An instance with every room enabled.</returns>
	public static Room All => new() { _data = ValidRoomsMask };
	/// <summary>
	/// Returns an instance with every room except the top room enabled.
	/// </summary>
	/// <returns>An instance with every room except the top room enabled.</returns>
	public static Room AllExceptTop => new() { _data = ValidRoomsMask & ~((_dt)1 << RoomCapacity) };
	/// <summary>
	/// Initializes a new instance of the <see cref="Room"/> struct with the specified room indices.
	/// </summary>
	/// <remarks>This constructor allows you to specify one or more room indices to initialize the <see
	/// cref="Room"/> instance. If no indices are provided, the room is set to an unavailable state.</remarks>
	/// <param name="rooms">An array of room indices to initialize. Each index represents a specific room to be enabled. If the array is
	/// empty, the room is marked as not available. If the array contains a single element, only that room is enabled. If
	/// the array contains multiple elements, all specified rooms are enabled.</param>
	public Room(params _it[] rooms)
	{
		foreach (_it item in rooms)
			this[item] = true;
	}
	/// <summary>
	/// Checks if the specified rooms are included.
	/// </summary>
	/// <param name="rooms">The rooms to check.</param>
	/// <returns>True if the rooms are included; otherwise, false.</returns>
	public readonly bool Contains(Room rooms)
	{
		return (_data & rooms._data) == rooms._data;
	}
	/// <summary>
	/// Checks if the specified room is included.
	/// </summary>
	/// <param name="room">The room to check.</param>
	/// <returns>True if the room is included; otherwise, false.</returns>
	public readonly bool Contains(RoomIndex room)
	{
		return (_data & (_dt)room) != 0;
	}
	/// <inheritdoc/>
	public static bool operator ==(Room R1, Room R2) => R1._data == R2._data;
	/// <inheritdoc/>
	public static bool operator !=(Room R1, Room R2) => !(R1 == R2);
	/// <summary>
	/// Implicitly converts a SingleRoom to a Room.
	/// </summary>
	/// <param name="room">The SingleRoom instance to convert.</param>
	/// <returns>A Room instance.</returns>
	public static implicit operator Room(SingleRoom room) =>
		room.Value <= RoomCapacity
			? new Room(room.Value)
			: new Room([]);
	/// <summary>
	/// Explicitly converts a Room to a SingleRoom.
	/// </summary>
	/// <param name="room">The Room instance to convert.</param>
	/// <returns>A SingleRoom instance.</returns>
	/// <exception cref="InvalidCastException">Thrown when the Room contains more than one room.</exception>
	public static explicit operator SingleRoom(Room room) =>
		room.Rooms.Length == 1
			? new SingleRoom(room.Rooms[0])
			: throw new InvalidCastException("This object has multiple rooms.");
	/// <inheritdoc/>
	public readonly override bool Equals(object? obj) => obj is Room e && Equals(e);
	/// <inheritdoc/>
#if NETSTANDARD
	public readonly override int GetHashCode()
	{
		int hash = 17;
		hash = hash * 31 + _data.GetHashCode();
		return hash;
	}
#else
	public readonly override int GetHashCode() => HashCode.Combine(_data);
#endif
	/// <inheritdoc/>
	public readonly bool Equals(Room other) => this == other;
	/// <summary>
	/// Returns an enumerator that iterates through the enabled room indices, from 0 to <see cref="RoomCapacity"/>.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public readonly IEnumerator<_it> GetEnumerator()
	{
		for (int i = 0; i < TotalRoomCount; i++)
			if (this[(_it)i])
				yield return (_it)i;
		yield break;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	readonly IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	private _dt _data;
}
