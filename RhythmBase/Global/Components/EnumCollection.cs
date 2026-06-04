using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RhythmBase.Global.Components;

public static class CollectionBuilders
{
	public static EnumCollection<TEnum> BuildEnumCollection<TEnum>(ReadOnlySpan<TEnum> values) where TEnum : unmanaged, Enum
	{
		EnumCollection<TEnum> collection = new(values);
		return collection;
	}
	public static ReadOnlyEnumCollection<TEnum> BuildReadOnlyEnumCollection<TEnum>(ReadOnlySpan<TEnum> values) where TEnum : unmanaged, Enum
	{
		ReadOnlyEnumCollection<TEnum> collection = new(values);
		return collection;
	}
}
/// <summary>
/// Represents a compact set-like collection for storing values of an enum type, providing efficient add, remove, and
/// enumeration operations.
/// </summary>
/// <typeparam name="TEnum">The enum type to be stored in the collection.</typeparam>
[CollectionBuilder(typeof(CollectionBuilders), nameof(CollectionBuilders.BuildEnumCollection))]
public unsafe struct EnumCollection<TEnum> : IEnumerable<TEnum> where TEnum : unmanaged, Enum
{
	private const int bw = sizeof(ulong) * 8;
	private readonly ulong signMask;
	internal ulong[] _lowerBits;
	internal ulong[] _upperBits;
	private readonly ulong[] Lower => _lowerBits ?? Array.Empty<ulong>();
	private readonly ulong[] Upper => _upperBits ?? Array.Empty<ulong>();
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
	private readonly TEnum ToEnum(ulong index, bool isNegative)
	{
		ulong value = isNegative ? (~index & (signMask - 1)) | signMask : (index & (signMask - 1));
		return Unsafe.As<ulong, TEnum>(ref value);
	}
	/// <summary>
	/// Initializes a new instance of the EnumCollection class, sizing automatically from the enum type.
	/// </summary>
	/// <exception cref="InvalidOperationException">Thrown if the enum values exceed the supported range.</exception>
	public EnumCollection()
	{
		signMask = ComputeSignMask();
		ulong enumLowerMax = 0;
		ulong enumUpperMax = 0;
		foreach (TEnum value in Enum.GetValues<TEnum>())
		{
			TEnum vt = value;
			ulong v = Unsafe.As<TEnum, ulong>(ref vt);
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
			lowerSize = checked((int)(enumLowerMax / (ulong)bw) + 1);
			upperSize = checked((int)(enumUpperMax / (ulong)bw) + 1);
		}
		catch
		{
			throw new InvalidOperationException("The enum value is too big.");
		}
		_lowerBits = new ulong[lowerSize];
		_upperBits = new ulong[upperSize];
	}
	/// <summary>
	/// Initializes a new instance of the collection with the supplied enum values.
	/// </summary>
	/// <param name="values">A parameter array whose values are immediately added to the collection.</param>
	public EnumCollection(params TEnum[] values) : this()
	{
		foreach (TEnum value in values)
			Add(value);
	}
	/// <summary>
	/// Initializes a new instance of the collection with the supplied read-only span.
	/// </summary>
	/// <param name="values">A read-only span whose values are immediately added to the collection.</param>
	public EnumCollection(params ReadOnlySpan<TEnum> values) : this()
	{
		foreach (TEnum value in values)
			Add(value);
	}
	internal EnumCollection(ulong[] lowerBits, ulong[] upperBits, ulong signMask)
	{
		this.signMask = signMask;
		_lowerBits = lowerBits;
		_upperBits = upperBits;
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
	/// <summary>
	/// Attempts to add the specified enum value to the collection if it is not already present.
	/// </summary>
	/// <param name="item">The enum value to add.</param>
	/// <returns>true if the value was successfully added; otherwise, false if already present.</returns>
	public bool Add(TEnum item)
	{
		ulong v = Unsafe.As<TEnum, ulong>(ref item);
		Classify(v, signMask, out ulong idx, out bool neg);
		int div = (int)(idx / (ulong)bw);
		int rem = (int)(idx % (ulong)bw);
		ref ulong[] bits = ref neg ? ref _upperBits : ref _lowerBits;
		if (bits is null || div >= bits.Length)
			Array.Resize(ref bits, div + 1);
		if ((bits[div] & ((ulong)1 << rem)) != 0)
			return false;
		bits[div] |= ((ulong)1 << rem);
		return true;
	}
	/// <summary>
	/// Removes the specified enumeration value from the collection if it exists.
	/// </summary>
	/// <param name="item">The enumeration value to remove.</param>
	/// <returns>true if the value was present and successfully removed; otherwise, false.</returns>
	public readonly bool Remove(TEnum item)
	{
		ulong v = Unsafe.As<TEnum, ulong>(ref item);
		Classify(v, signMask, out ulong idx, out bool neg);
		ulong[] bits = neg ? Upper : Lower;
		int div = (int)(idx / (ulong)bw);
		int rem = (int)(idx % (ulong)bw);
		if (div >= bits.Length) return false;
		if ((bits[div] & ((ulong)1 << rem)) == 0)
			return false;
		bits[div] &= ~((ulong)1 << rem);
		return true;
	}
	/// <summary>
	/// Determines whether the specified enumeration value is present in the current set.
	/// </summary>
	/// <param name="type">The enumeration value to locate.</param>
	/// <returns>true if the value is contained; otherwise, false.</returns>
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
	public readonly bool ContainsAny(EnumCollection<TEnum> types)
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
	/// Determines whether all specified enumeration values are contained within the current set.
	/// </summary>
	public readonly bool ContainsAll(params TEnum[] types)
	{
		foreach (TEnum type in types)
			if (!Contains(type)) return false;
		return true;
	}
	/// <summary>
	/// Determines whether all specified enumeration values are contained within the current set.
	/// </summary>
	public readonly bool ContainsAll(params IEnumerable<TEnum> types)
	{
		foreach (TEnum type in types)
			if (!Contains(type)) return false;
		return true;
	}
	/// <summary>
	/// Determines whether all specified enumeration values are contained within the current set.
	/// </summary>
	public readonly bool ContainsAll(EnumCollection<TEnum> types)
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
	public readonly EnumCollection<TEnum> Intersect(EnumCollection<TEnum> other)
	{
		return new EnumCollection<TEnum>(
				BitwiseAnd(Lower, other.Lower),
				BitwiseAnd(Upper, other.Upper),
				signMask);
	}
	/// <summary>
	/// Returns a new collection containing the elements of this collection that are not present in the specified collection.
	/// </summary>
	public readonly EnumCollection<TEnum> Except(EnumCollection<TEnum> other)
	{
		return new EnumCollection<TEnum>(
				BitwiseAndNot(Lower, other.Lower),
				BitwiseAndNot(Upper, other.Upper),
				signMask);
	}
	/// <summary>
	/// Returns a new collection containing the symmetric difference between this collection and the specified collection.
	/// </summary>
	public readonly EnumCollection<TEnum> SymmetricExcept(EnumCollection<TEnum> other)
	{
		return new EnumCollection<TEnum>(
				BitwiseXor(Lower, other.Lower),
				BitwiseXor(Upper, other.Upper),
				signMask);
	}
	/// <summary>
	/// Returns a new collection that contains all elements present in either this collection or the specified collection.
	/// </summary>
	public readonly EnumCollection<TEnum> Union(EnumCollection<TEnum> other)
	{
		return new EnumCollection<TEnum>(
				BitwiseOr(Lower, other.Lower),
				BitwiseOr(Upper, other.Upper),
				signMask);
	}
	/// <summary>
	/// Returns a read-only wrapper for the current collection.
	/// </summary>
	public readonly ReadOnlyEnumCollection<TEnum> AsReadOnly()
	{
		ulong[] srcLower = Lower;
		ulong[] srcUpper = Upper;
		ulong[] newLower = new ulong[srcLower.Length];
		Array.Copy(srcLower, newLower, srcLower.Length);
		ulong[] newUpper = new ulong[srcUpper.Length];
		Array.Copy(srcUpper, newUpper, srcUpper.Length);
		return new ReadOnlyEnumCollection<TEnum>(newLower, newUpper, signMask);
	}
	/// <summary>
	/// Returns an enumerator that iterates through the set of values contained in the collection.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public readonly IEnumerator<TEnum> GetEnumerator()
	{
		ulong[] upper = Upper, lower = Lower;
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
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
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
