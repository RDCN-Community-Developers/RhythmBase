using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Represents a fixed-size collection of 16 player types, accessible by index.
/// </summary>
/// <remarks>This structure provides indexed access to a collection of player types, where the valid index range
/// is 0 to 15.  Attempting to access an index outside this range will result in an <see
/// cref="IndexOutOfRangeException"/>.</remarks>
[CollectionBuilder(typeof(CollectionBuilders), nameof(CollectionBuilders.BuildPlayerTypeGroup))]
public readonly struct PlayerTypeGroup() : IEquatable<PlayerTypeGroup>, IEnumerable<PlayerType>
{
	private readonly PlayerType[] _players = new PlayerType[RowCapacity];
	/// <summary>
	/// Initializes a new instance of the PlayerTypeGroup class with the specified player types.
	/// </summary>
	/// <remarks>If fewer than 16 player types are provided, the remaining slots in the group are left
	/// uninitialized.</remarks>
	/// <param name="playerTypes">An array of PlayerType instances to include in the group. Only the first 16 elements are used; additional
	/// elements are ignored.</param>
	public PlayerTypeGroup(params PlayerType[] playerTypes) : this()
	{
		for (int i = 0; i < 16 && i < playerTypes.Length; i++)
		{
			_players[i] = playerTypes[i];
		}
	}
	/// <summary>
	/// Initializes a new instance of the PlayerTypeGroup class, assigning the specified player type to all player slots
	/// in the group.
	/// </summary>
	/// <remarks>This constructor sets all 16 player slots to the provided player type. Use this overload to
	/// quickly create a group where all players share the same type.</remarks>
	/// <param name="playerType">The player type to assign to each slot in the group.</param>
	public PlayerTypeGroup(PlayerType playerType) : this()
	{
		for (int i = 0; i < 16; i++)
		{
			_players[i] = playerType;
		}
	}
	/// <summary>
	/// Gets or sets the <see cref="PlayerType"/> at the specified index.
	/// </summary>
	/// <param name="index">The zero-based index of the player. Must be in the range 0 to 15, inclusive.</param>
	/// <returns></returns>
	/// <exception cref="IndexOutOfRangeException">Thrown if <paramref name="index"/> is less than 0 or greater than 15.</exception>
	public readonly PlayerType this[int index]
	{
		get => index is >= 0 and < 16 ? _players[index] : throw new IndexOutOfRangeException(nameof(index));
		set => _players[index] = index is >= 0 and < 16 ? value : throw new IndexOutOfRangeException(nameof(index));
	}
	///<inherteddoc/>
	public IEnumerator<PlayerType> GetEnumerator()
	{
		for (int i = 0; i < 16; i++)
		{
			yield return _players[i];
		}
	}
	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
	///<inherteddoc/>
	public bool Equals(PlayerTypeGroup other)
	{
		for (int i = 0; i < 16; i++)
		{
			if (this[i] != other[i])
				return false;
		}
		return true;
	}
	///<inherteddoc/>
	public override bool Equals([NotNullWhen(true)] object? obj) => obj is PlayerTypeGroup other && Equals(other);
	///<inherteddoc/>
	public override int GetHashCode() => base.GetHashCode();
	/// <summary>
	/// Determines whether two <see cref="PlayerTypeGroup"/> instances are equal.
	/// </summary>
	/// <param name="left">The first <see cref="PlayerTypeGroup"/> to compare.</param>
	/// <param name="right">The second <see cref="PlayerTypeGroup"/> to compare.</param>
	/// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
	public static bool operator ==(PlayerTypeGroup left, PlayerTypeGroup right)
	{
		foreach (var player in left)
		{
			if (!right.Contains(player))
				return false;
		} return true;
	}
	/// <summary>
	/// Determines whether two <see cref="PlayerTypeGroup"/> instances are not equal.
	/// </summary>
	/// <param name="left">The first <see cref="PlayerTypeGroup"/> to compare.</param>
	/// <param name="right">The second <see cref="PlayerTypeGroup"/> to compare.</param>
	/// <returns><c>true</c> if the instances are not equal; otherwise, <c>false</c>.</returns>
	public static bool operator !=(PlayerTypeGroup left, PlayerTypeGroup right)
	{
		return !(left == right);
	}
}
