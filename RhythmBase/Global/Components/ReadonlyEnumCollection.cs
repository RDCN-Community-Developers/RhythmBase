using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RhythmBase.Global.Components;

/// <summary>
/// Represents a read-only collection of unique enumeration values of type <typeparamref name="TEnum"/>.
/// </summary>
/// <remarks>This collection provides efficient storage and lookup for sets of enumeration values. It is
/// immutable after creation; values cannot be added or removed. The collection supports enumeration and membership
/// checks, making it suitable for scenarios where a fixed set of enum values needs to be referenced or queried without
/// modification.</remarks>
/// <typeparam name="TEnum">The enumeration type contained in the collection. Must be a value type that derives from <see cref="Enum"/>.</typeparam>
[CollectionBuilder(typeof(CollectionBuilders), nameof(CollectionBuilders.BuildReadOnlyEnumCollection))]
public readonly unsafe struct ReadOnlyEnumCollection<TEnum> : IEnumerable<TEnum> where TEnum : unmanaged, Enum
{
	private const int bw = sizeof(ulong) * 8;
	private readonly ulong[] _lowerBits;
	private readonly ulong[] _upperBits;
	private readonly ulong signMask;
	private readonly ulong[] Lower => _lowerBits ?? [];
	private readonly ulong[] Upper => _upperBits ?? [];
	private static ulong ComputeSignMask()
	{
		int bitWidth = sizeof(TEnum) * 8;
		return (bitWidth >= bw) ? 0u : ((ulong)1 << (bitWidth - 1));
	}
	private static void Classify(ulong raw, ulong signMask, out ulong index, out bool isNegative)
	{
		isNegative = (raw & signMask) != 0;
		index = isNegative ? (~raw & (signMask - 1)) : (raw & (signMask - 1));
	}
	/// <summary>
	/// Initializes a new instance of the <see cref="ReadOnlyEnumCollection{TEnum}"/> class from a <see cref="ReadOnlySpan{T}"/> source.
	/// </summary>
	/// <param name="values">The sequence of enum values to include in the collection.</param>
	/// <exception cref="InvalidOperationException">Thrown when the maximum enum value cannot be represented in the collection.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when a provided enum value falls outside the computed bounds.</exception>
	public ReadOnlyEnumCollection(params ReadOnlySpan<TEnum> values)
	{
		signMask = ComputeSignMask();
		ulong enumLowerMax = 0;
		ulong enumUpperMax = 0;
		foreach (TEnum value in values)
		{
			var temp = value;
			ulong v = Unsafe.As<TEnum, ulong>(ref temp);
			Classify(v, signMask, out ulong idx, out bool neg);
			if (neg)
			{
				if (idx > enumUpperMax) enumUpperMax = idx;
			}
			else
			{
				if (idx > enumLowerMax) enumLowerMax = idx;
			}
		}
		int lowerSize, upperSize;
		try
		{
			lowerSize = (values.Length == 0) ? 0 : checked((int)(enumLowerMax / (ulong)bw) + 1);
			upperSize = (values.Length == 0) ? 0 : checked((int)(enumUpperMax / (ulong)bw) + 1);
		}
		catch
		{
			throw new InvalidOperationException("The enum value is too big.");
		}
		_lowerBits = new ulong[lowerSize];
		_upperBits = new ulong[upperSize];
		foreach (TEnum value in values)
		{
			var temp = value;
			ulong v = Unsafe.As<TEnum, ulong>(ref temp);
			Classify(v, signMask, out ulong idx, out bool neg);
			ulong[] bits = neg ? _upperBits : _lowerBits;
			int div = (int)(idx / (ulong)bw);
			int rem = (int)(idx % (ulong)bw);
			if (div < 0 || div >= bits.Length) throw new ArgumentOutOfRangeException(nameof(values), "Enum value out of collection range.");
			bits[div] |= ((ulong)1 << rem);
		}
	}
	/// <summary>
	/// Gets the number of elements contained in the collection.
	/// </summary>
	public readonly int Count
	{
		get
		{
			int count = 0;
			foreach (ulong block in Lower)
			{
				ulong b = block;
				while (b != 0) { b &= (b - 1); count++; }
			}
			foreach (ulong block in Upper)
			{
				ulong b = block;
				while (b != 0) { b &= (b - 1); count++; }
			}
			return count;
		}
	}
	/// <summary>
	/// Gets a value indicating whether the collection contains no elements.
	/// </summary>
	public readonly bool IsEmpty
	{
		get
		{
			foreach (ulong block in Lower)
				if (block != 0) return false;
			foreach (ulong block in Upper)
				if (block != 0) return false;
			return true;
		}
	}
	private readonly TEnum ToEnum(ulong index, bool isNegative)
	{
		ulong value = isNegative ? (~index & (signMask - 1)) | signMask : (index & (signMask - 1));
		return Unsafe.As<ulong, TEnum>(ref value);
	}
	/// <summary>
	/// Determines whether the specified enumeration value is present in the set.
	/// </summary>
	/// <param name="type">The enumeration value to locate within the set.</param>
	/// <returns>true if the set contains the specified enumeration value; otherwise, false.</returns>
	public readonly bool Contains(TEnum type)
	{
		ulong v = Unsafe.As<TEnum, ulong>(ref type);
		Classify(v, signMask, out ulong idx, out bool neg);
		ulong[] bits = neg ? Upper : Lower;
		int div = (int)(idx / (ulong)bw);
		int rem = (int)(idx % (ulong)bw);
		return div >= 0 && div < bits.Length && (bits[div] & ((ulong)1 << rem)) != 0;
	}
	/// <summary>
	/// Determines whether the collection contains any of the specified enumeration values.
	/// </summary>
	public readonly bool ContainsAny(params TEnum[] types)
	{
		foreach (TEnum type in types)
			if (Contains(type)) return true;
		return false;
	}
	/// <summary>
	/// Determines whether the collection contains any of the specified enumeration values.
	/// </summary>
	public readonly bool ContainsAny(params IEnumerable<TEnum> types)
	{
		foreach (TEnum type in types)
			if (Contains(type)) return true;
		return false;
	}
	/// <summary>
	/// Determines whether the collection contains any of the specified enumeration values.
	/// </summary>
	public readonly bool ContainsAny(ReadOnlyEnumCollection<TEnum> types)
	{
		ulong[] thisLower = Lower, thisUpper = Upper;
		ulong[] otherLower = types.Lower, otherUpper = types.Upper;
		for (int i = 0; i < Math.Min(thisLower.Length, otherLower.Length); i++)
			if ((thisLower[i] & otherLower[i]) != 0) return true;
		for (int i = 0; i < Math.Min(thisUpper.Length, otherUpper.Length); i++)
			if ((thisUpper[i] & otherUpper[i]) != 0) return true;
		return false;
	}
	/// <summary>
	/// Determines whether the collection contains all of the specified enumeration values.
	/// </summary>
	public readonly bool ContainsAll(params TEnum[] types)
	{
		foreach (TEnum type in types)
			if (!Contains(type)) return false;
		return true;
	}
	/// <summary>
	/// Determines whether the collection contains all of the specified enumeration values.
	/// </summary>
	public readonly bool ContainsAll(params IEnumerable<TEnum> types)
	{
		foreach (TEnum type in types)
			if (!Contains(type)) return false;
		return true;
	}
	/// <summary>
	/// Determines whether the collection contains all of the specified enumeration values.
	/// </summary>
	public readonly bool ContainsAll(ReadOnlyEnumCollection<TEnum> types)
	{
		ulong[] thisLower = Lower, thisUpper = Upper;
		ulong[] otherLower = types.Lower, otherUpper = types.Upper;
		for (int i = 0; i < Math.Min(thisLower.Length, otherLower.Length); i++)
		{
			ulong tBits = otherLower[i];
			if (tBits != 0 && (thisLower[i] & tBits) != tBits) return false;
		}
		for (int i = thisLower.Length; i < otherLower.Length; i++)
			if (otherLower[i] != 0) return false;
		for (int i = 0; i < Math.Min(thisUpper.Length, otherUpper.Length); i++)
		{
			ulong tBits = otherUpper[i];
			if (tBits != 0 && (thisUpper[i] & tBits) != tBits) return false;
		}
		for (int i = thisUpper.Length; i < otherUpper.Length; i++)
			if (otherUpper[i] != 0) return false;
		return true;
	}
	private static ulong[] BitwiseAnd(ReadOnlySpan<ulong> a, ReadOnlySpan<ulong> b)
	{
		int size = Math.Min(a.Length, b.Length);
		ulong[] result = new ulong[size];
		for (int i = 0; i < size; i++)
			result[i] = a[i] & b[i];
		return result;
	}
	private static ulong[] BitwiseAndNot(ReadOnlySpan<ulong> a, ReadOnlySpan<ulong> b)
	{
		int size = Math.Min(a.Length, b.Length);
		ulong[] result = new ulong[a.Length];
		for (int i = 0; i < size; i++)
			result[i] = a[i] & ~b[i];
		for (int i = size; i < a.Length; i++)
			result[i] = a[i];
		return result;
	}
	private static ulong[] BitwiseXor(ReadOnlySpan<ulong> a, ReadOnlySpan<ulong> b)
	{
		int size = Math.Max(a.Length, b.Length);
		ulong[] result = new ulong[size];
		for (int i = 0; i < size; i++)
		{
			ulong va = (i < a.Length) ? a[i] : 0u;
			ulong vb = (i < b.Length) ? b[i] : 0u;
			result[i] = va ^ vb;
		}
		return result;
	}
	private static ulong[] BitwiseOr(ReadOnlySpan<ulong> a, ReadOnlySpan<ulong> b)
	{
		int size = Math.Max(a.Length, b.Length);
		ulong[] result = new ulong[size];
		for (int i = 0; i < size; i++)
		{
			ulong va = (i < a.Length) ? a[i] : 0u;
			ulong vb = (i < b.Length) ? b[i] : 0u;
			result[i] = va | vb;
		}
		return result;
	}
	/// <summary>
	/// Returns a new collection containing the elements that exist in both this collection and the specified collection.
	/// </summary>
	public readonly ReadOnlyEnumCollection<TEnum> Intersect(ReadOnlyEnumCollection<TEnum> other)
	{
		return new ReadOnlyEnumCollection<TEnum>(
			BitwiseAnd(Lower, other.Lower),
			BitwiseAnd(Upper, other.Upper),
			signMask);
	}
	/// <summary>
	/// Returns a new collection containing the elements of this collection that are not present in the specified collection.
	/// </summary>
	public readonly ReadOnlyEnumCollection<TEnum> Except(ReadOnlyEnumCollection<TEnum> other)
	{
		return new ReadOnlyEnumCollection<TEnum>(
			BitwiseAndNot(Lower, other.Lower),
			BitwiseAndNot(Upper, other.Upper),
			signMask);
	}
	/// <summary>
	/// Returns a new collection containing the symmetric difference between this collection and the specified collection.
	/// </summary>
	public readonly ReadOnlyEnumCollection<TEnum> SymmetricExcept(ReadOnlyEnumCollection<TEnum> other)
	{
		return new ReadOnlyEnumCollection<TEnum>(
			BitwiseXor(Lower, other.Lower),
			BitwiseXor(Upper, other.Upper),
			signMask);
	}
	/// <summary>
	/// Returns a new collection that represents the union of the current collection and the specified collection.
	/// </summary>
	public readonly ReadOnlyEnumCollection<TEnum> Union(ReadOnlyEnumCollection<TEnum> other)
	{
		return new ReadOnlyEnumCollection<TEnum>(
			BitwiseOr(Lower, other.Lower),
			BitwiseOr(Upper, other.Upper),
			signMask);
	}
	/// <summary>
	/// Casts the collection to a different enumeration type.
	/// </summary>
	public readonly ReadOnlyEnumCollection<TEnum2> Cast<TEnum2>() where TEnum2 : unmanaged, Enum
	{
		if (typeof(TEnum) == typeof(TEnum2))
			return Unsafe.As<ReadOnlyEnumCollection<TEnum>, ReadOnlyEnumCollection<TEnum2>>(ref Unsafe.AsRef(in this));
		int byteWidth = sizeof(TEnum2);
		ulong enumMask = (byteWidth * 8 >= bw) ? ~(ulong)0 : (((ulong)1 << (byteWidth * 8)) - 1u);
		ulong[] lower = Lower, upper = Upper;
		ulong[] newLower = new ulong[lower.Length];
		ulong[] newUpper = new ulong[upper.Length];
		for (int i = 0; i < lower.Length; i++)
			newLower[i] = lower[i] & enumMask;
		for (int i = 0; i < upper.Length; i++)
			newUpper[i] = upper[i] & enumMask;
		return new ReadOnlyEnumCollection<TEnum2>(newLower, newUpper, ComputeSignMask());
	}
	/// <summary>
	/// Returns an enumerator that iterates through the set of values contained in the collection.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public readonly IEnumerator<TEnum> GetEnumerator()
	{
		ulong[] upper = Upper, lower = Lower;
		for (int div = upper.Length - 1; div >= 0; div--)
		{
			ulong block = upper[div];
			while (block != 0)
			{
				int rem = TrailingZeroCount(block);
				ulong value = (ulong)(div * bw + rem);
				yield return ToEnum(value, true);
				block &= (block - 1);
			}
		}
		for (int div = 0; div < lower.Length; div++)
		{
			ulong block = lower[div];
			while (block != 0)
			{
				int rem = TrailingZeroCount(block);
				ulong value = (ulong)(div * bw + rem);
				yield return ToEnum(value, false);
				block &= (block - 1);
			}
		}
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	internal ReadOnlyEnumCollection(ulong[] lowerBits, ulong[] upperBits, ulong signMask)
	{
		this.signMask = signMask;
		_lowerBits = lowerBits;
		_upperBits = upperBits;
	}
	private static int TrailingZeroCount(ulong v)
	{
#if NET8_0_OR_GREATER
		return System.Numerics.BitOperations.TrailingZeroCount((ulong)v);
#else
		int c = 0;
		while ((v & (ulong)1) == 0)
		{
			v >>= 1;
			c++;
		}
		return c;
#endif
	}
}
