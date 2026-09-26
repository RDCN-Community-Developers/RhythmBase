using RhythmBase.RhythmDoctor.Serialization;
// Index type: 0-based room ordinal in the range 0..RoomCapacity.
using _it = System.Byte;
// Storage type: bitmask where bit i represents the room at index i.
using _dt = System.UInt32;
namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Represents a single room that can be applied to one room only.
/// </summary>
[System.Text.Json.Serialization.JsonConverter(typeof(SingleRoomConverter))]
public struct SingleRoom(RoomIndex index) : IEquatable<SingleRoom>
{
	/// <summary>
	/// The number of valid rooms, including the top room; valid indices are 0..<see cref="RoomCapacity"/>.
	/// </summary>
	private const int TotalRoomCount = RoomCapacity + 1;
	/// <summary>
	/// The sentinel value for an unavailable room.
	/// </summary>
	private const _dt NotAvailable = byte.MaxValue;
	/// <summary>
	/// Gets a value indicating whether this room is the top room.
	/// </summary>
	public readonly bool EnableTop => _data == (uint)RoomIndex.RoomTop;
	/// <summary>
	/// Gets or sets the applied room.
	/// </summary>
	public RoomIndex Room
	{
		readonly get => (RoomIndex)_data;
		set => _data = (_dt)value;
	}
	/// <summary>
	/// Gets or sets the applied room index as a byte. 0-base.
	/// </summary>
	public _it Value
	{
		readonly get
		{
			for (int i = 0; i < TotalRoomCount; i++)
				if (_data == ((_dt)1 << i))
					return (_it)i;
			return (_it)NotAvailable;
		}
		set => _data = value <= RoomCapacity ? (_dt)1 << value : NotAvailable;
	}
	/// <summary>
	/// Returns a string that represents the current object.
	/// </summary>
	/// <returns>A string that represents the current object.</returns>
	public readonly override string ToString() => $"[{_data}]";
	/// <summary>
	/// Gets the default single room, which represents an unavailable room.
	/// </summary>
	public static SingleRoom Default => new((RoomIndex)NotAvailable);
	/// <summary>
	/// Initializes a new instance of the <see cref="SingleRoom"/> struct with the specified room index.
	/// </summary>
	/// <param name="room">The 0-base room index, or 255 for an unavailable room.</param>
	public SingleRoom(_it room) : this(room <= RoomCapacity ? (RoomIndex)(1 << room) : (RoomIndex)NotAvailable) { }

	/// <inheritdoc/>
	public static bool operator ==(SingleRoom R1, SingleRoom R2) => R1._data == R2._data;
	/// <inheritdoc/>
	public static bool operator !=(SingleRoom R1, SingleRoom R2) => R1._data != R2._data;
	/// <inheritdoc/>
	public static implicit operator SingleRoom(RoomIndex room) => new(room);
	/// <inheritdoc/>
	public readonly override bool Equals(object? obj) => obj is SingleRoom e && Equals(e);
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
	public readonly bool Equals(SingleRoom other) => _data == other._data;
	private _dt _data = (_dt)index;
}
