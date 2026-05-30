using RhythmBase.RhythmDoctor.Converters;
using System.Text.Json.Serialization;
namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Represents a single room that can be applied to one room only.
/// </summary>
[System.Text.Json.Serialization.JsonConverter(typeof(SingleRoomConverter))]
public struct SingleRoom(RoomIndex index) : IEquatable<SingleRoom>
{
	/// <summary>
	/// Gets a value indicating whether it can be used in the top room.
	/// </summary>
	public bool EnableTop { get; }
	/// <summary>
	/// Gets or sets the applied room.
	/// </summary>
	public RoomIndex Room
	{
		readonly get => _data;
		set => _data = value;
	}
	/// <summary>
	/// Gets or sets the applied room index as a byte.
	/// </summary>
	public byte Value
	{
		readonly get
		{
			for (int i = 0; i < 5; i++)
			{
				if (_data == (RoomIndex)(1 << i))
					return (byte)i;
			}
			return byte.MaxValue;
		}
		set => _data = (RoomIndex)(1 << value);
	}
	/// <summary>
	/// Returns a string that represents the current object.
	/// </summary>
	/// <returns>A string that represents the current object.</returns>
	public readonly override string ToString() => $"[{_data}]";
	/// <summary>
	/// Gets the default single room which represents room 0.
	/// </summary>
	public static SingleRoom Default => new((RoomIndex)255);
	/// <summary>
	/// Initializes a new instance of the <see cref="SingleRoom"/> struct with the specified room index.
	/// </summary>
	/// <param name="room">The room index.</param>
	public SingleRoom(byte room) : this((RoomIndex)(1 << room)) { }

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
	private RoomIndex _data = index;
}
