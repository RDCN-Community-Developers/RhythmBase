using System.Collections;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Represents a room that can be applied to multiple rooms.
/// </summary>
[CollectionBuilder(typeof(CollectionBuilders), nameof(CollectionBuilders.BuildRoom))]
public struct Room :
#if NET7_0_OR_GREATER
	IEqualityOperators<Room, Room, bool>,
#endif
	IEquatable<Room>, IEnumerable<byte>
{
	/// <summary>
	/// Gets or sets whether the specified room is enabled. 0-base.
	/// </summary>
	/// <param name="index">The index of the room.</param>
	/// <returns>True if the room is enabled; otherwise, false.</returns>
	[IndexerName("Room")]
	public bool this[byte index]
	{
		readonly get => _data.HasFlag((RoomIndex)(1 << index));
		set
		{
			if (index <= 4)
				_data = value ? _data | (RoomIndex)(1 << index) : _data & (RoomIndex)(~(1 << index));
		}
	}
	/// <summary>
	/// Gets or sets whether the specified room is enabled.
	/// </summary>
	/// <param name="index">The index of the room.</param>
	/// <returns>True if the room is enabled; otherwise, false.</returns>
	[IndexerName("Room")]
	public bool this[RoomIndex index]
	{
		readonly get => _data.HasFlag(index);
		set
		{
			if (((byte)index & 0b1111) != 0)
				_data = value ? _data | index : _data & (~index);
		}
	}
	/// <summary>
	/// Gets the list of enabled rooms.
	/// </summary>
	public readonly byte[] Rooms
	{
		get
		{
			RoomIndex indexes = _data;
			return [..Enumerable
				.Range(0, 5)
				.Where(x => indexes.HasFlag((RoomIndex)(1 << x)))
				.Select(x => (byte)x)];
		}
	}
	/// <inheritdoc/>
	public readonly override string ToString() => $"[{string.Join(",", Rooms)}]";
	/// <summary>
	/// Returns an instance with only room 1 enabled.
	/// </summary>
	/// <returns>An instance with only room 1 enabled.</returns>
	public static Room Default => new([])
	{
		_data = RoomIndex.Room1
	};
	///// <summary>
	///// Initializes a new instance of the <see cref="RDRoom"/> struct.
	///// </summary>
	///// <param name="enableTop">Indicates if the top room can be applied.</param>
	//public RDRoom(bool enableTop) => EnableTop = enableTop;
	/// <summary>
	/// Initializes a new instance of the <see cref="Room"/> struct with the specified room indices.
	/// </summary>
	/// <remarks>This constructor allows you to specify one or more room indices to initialize the <see
	/// cref="Room"/> instance. If no indices are provided, the room is set to an unavailable state.</remarks>
	/// <param name="rooms">An array of room indices to initialize. Each index represents a specific room to be enabled. If the array is
	/// empty, the room is marked as not available. If the array contains a single element, only that room is enabled. If
	/// the array contains multiple elements, all specified rooms are enabled.</param>
	public Room(params byte[] rooms)
	{
		this = default;
		//EnableTop = enableTop;
		int num = rooms.Length;
		if (num != 0)
			if (num != 1)
				foreach (byte item in rooms)
					this[item] = true;
			else
				this[rooms.Single()] = true;
		else
			_data = RoomIndex.RoomNotAvaliable;
	}
	/// <summary>
	/// Checks if the specified rooms are included.
	/// </summary>
	/// <param name="rooms">The rooms to check.</param>
	/// <returns>True if the rooms are included; otherwise, false.</returns>
	public readonly bool Contains(Room rooms)
	{
		if (_data == RoomIndex.RoomNotAvaliable)
			return false;
		else
		{
			for (int i = 0; i < 5; i++)
			{
				if (this[(byte)i] != rooms[(byte)i])
					break;
				if (i > 4)
					return true;
			}
			return false;
		}
	}
	/// <summary>
	/// Checks if the specified room is included.
	/// </summary>
	/// <param name="room">The room to check.</param>
	/// <returns>True if the room is included; otherwise, false.</returns>
	public readonly bool Contains(RoomIndex room)
	{
		return _data.HasFlag(room);
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
	public static implicit operator Room(SingleRoom room) => new([((byte)room.Room)]);
	/// <summary>
	/// Explicitly converts a Room to a SingleRoom.
	/// </summary>
	/// <param name="room">The Room instance to convert.</param>
	/// <returns>A SingleRoom instance.</returns>
	/// <exception cref="RhythmBaseException">Thrown when the Room contains more than one room.</exception>
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
    /// Returns an enumerator that iterates through the collection of bytes with indices from 0 to 4 for which the
    /// corresponding value is set to <see langword="true"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public readonly IEnumerator<byte> GetEnumerator()
	{
		for (int i = 0; i < 5; i++)
			if (this[(byte)i])
				yield return (byte)i;
		yield break;
	}

    [EditorBrowsable(EditorBrowsableState.Never)]
    readonly IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	private RoomIndex _data;
}
