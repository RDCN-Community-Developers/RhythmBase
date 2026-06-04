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
	private static readonly int bw = sizeof(nuint) * 8;
	private readonly nuint signMask;
	internal nuint[] _lowerBits;
	internal nuint[] _upperBits;
	private readonly nuint[] Lower => _lowerBits ?? Array.Empty<nuint>();
	private readonly nuint[] Upper => _upperBits ?? Array.Empty<nuint>();
	private static nuint ComputeSignMask()
	{
		int bitWidth = sizeof(TEnum) * 8;
		return (bitWidth >= bw) ? 0u : ((nuint)1 << (bitWidth - 1));
	}
	private static void Classify(nuint raw, nuint signMask, out nuint index, out bool isNegative)
	{
		isNegative = (raw & signMask) != 0;
		index = isNegative ? (~raw & (signMask - 1)) : (raw & (signMask - 1));
	}
	private readonly TEnum ToEnum(nuint index, bool isNegative)
	{
		nuint value = isNegative ? (~index & (signMask - 1)) | signMask : (index & (signMask - 1));
		return Unsafe.As<nuint, TEnum>(ref value);
	}
	/// <summary>
	/// Initializes a new instance of the EnumCollection class, sizing automatically from the enum type.
	/// </summary>
	/// <exception cref="InvalidOperationException">Thrown if the enum values exceed the supported range.</exception>
	public EnumCollection()
	{
		signMask = ComputeSignMask();
		nuint enumLowerMax = 0;
		nuint enumUpperMax = 0;
		foreach (TEnum value in Enum.GetValues<TEnum>())
		{
			TEnum vt = value;
			nuint v = Unsafe.As<TEnum, nuint>(ref vt);
			Classify(v, signMask, out nuint idx, out bool neg);
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
			lowerSize = checked((int)(enumLowerMax / (nuint)bw) + 1);
			upperSize = checked((int)(enumUpperMax / (nuint)bw) + 1);
		}
		catch
		{
			throw new InvalidOperationException("The enum value is too big.");
		}
		_lowerBits = new nuint[lowerSize];
		_upperBits = new nuint[upperSize];
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
	internal EnumCollection(nuint[] lowerBits, nuint[] upperBits, nuint signMask)
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
			foreach (nuint block in Lower)
			{
				nuint b = block;
				while (b != 0) { b &= (b - 1); count++; }
			}
			foreach (nuint block in Upper)
			{
				nuint b = block;
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
			foreach (nuint block in Lower)
				if (block != 0) return false;
			foreach (nuint block in Upper)
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
		nuint v = Unsafe.As<TEnum, nuint>(ref item);
		Classify(v, signMask, out nuint idx, out bool neg);
		int div = (int)(idx / (nuint)bw);
		int rem = (int)(idx % (nuint)bw);
		ref nuint[] bits = ref neg ? ref _upperBits : ref _lowerBits;
		if (bits is null || div >= bits.Length)
			Array.Resize(ref bits, div + 1);
		if ((bits[div] & ((nuint)1 << rem)) != 0)
			return false;
		bits[div] |= ((nuint)1 << rem);
		return true;
	}
	/// <summary>
	/// Removes the specified enumeration value from the collection if it exists.
	/// </summary>
	/// <param name="item">The enumeration value to remove.</param>
	/// <returns>true if the value was present and successfully removed; otherwise, false.</returns>
	public readonly bool Remove(TEnum item)
	{
		nuint v = Unsafe.As<TEnum, nuint>(ref item);
		Classify(v, signMask, out nuint idx, out bool neg);
		nuint[] bits = neg ? Upper : Lower;
		int div = (int)(idx / (nuint)bw);
		int rem = (int)(idx % (nuint)bw);
		if (div >= bits.Length) return false;
		if ((bits[div] & ((nuint)1 << rem)) == 0)
			return false;
		bits[div] &= ~((nuint)1 << rem);
		return true;
	}
	/// <summary>
	/// Determines whether the specified enumeration value is present in the current set.
	/// </summary>
	/// <param name="type">The enumeration value to locate.</param>
	/// <returns>true if the value is contained; otherwise, false.</returns>
	public readonly bool Contains(TEnum type)
	{
		nuint v = Unsafe.As<TEnum, nuint>(ref type);
		Classify(v, signMask, out nuint idx, out bool neg);
		nuint[] bits = neg ? Upper : Lower;
		int div = (int)(idx / (nuint)bw);
		int rem = (int)(idx % (nuint)bw);
		return div >= 0 && div < bits.Length && (bits[div] & ((nuint)1 << rem)) != 0;
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
		nuint[] thisLower = Lower, thisUpper = Upper;
		nuint[] otherLower = types.Lower, otherUpper = types.Upper;
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
		nuint[] thisLower = Lower, thisUpper = Upper;
		nuint[] otherLower = types.Lower, otherUpper = types.Upper;
		for (int i = 0; i < Math.Min(thisLower.Length, otherLower.Length); i++)
		{
			nuint tBits = otherLower[i];
			if (tBits != 0 && (thisLower[i] & tBits) != tBits) return false;
		}
		for (int i = thisLower.Length; i < otherLower.Length; i++)
			if (otherLower[i] != 0) return false;
		for (int i = 0; i < Math.Min(thisUpper.Length, otherUpper.Length); i++)
		{
			nuint tBits = otherUpper[i];
			if (tBits != 0 && (thisUpper[i] & tBits) != tBits) return false;
		}
		for (int i = thisUpper.Length; i < otherUpper.Length; i++)
			if (otherUpper[i] != 0) return false;
		return true;
	}
	private static nuint[] BitwiseAnd(ReadOnlySpan<nuint> a, ReadOnlySpan<nuint> b)
	{
		int size = Math.Min(a.Length, b.Length);
		nuint[] result = new nuint[size];
		for (int i = 0; i < size; i++)
			result[i] = a[i] & b[i];
		return result;
	}
	private static nuint[] BitwiseAndNot(ReadOnlySpan<nuint> a, ReadOnlySpan<nuint> b)
	{
		int size = Math.Min(a.Length, b.Length);
		nuint[] result = new nuint[a.Length];
		for (int i = 0; i < size; i++)
			result[i] = a[i] & ~b[i];
		for (int i = size; i < a.Length; i++)
			result[i] = a[i];
		return result;
	}
	private static nuint[] BitwiseXor(ReadOnlySpan<nuint> a, ReadOnlySpan<nuint> b)
	{
		int size = Math.Max(a.Length, b.Length);
		nuint[] result = new nuint[size];
		for (int i = 0; i < size; i++)
		{
			nuint va = (i < a.Length) ? a[i] : 0u;
			nuint vb = (i < b.Length) ? b[i] : 0u;
			result[i] = va ^ vb;
		}
		return result;
	}
	private static nuint[] BitwiseOr(ReadOnlySpan<nuint> a, ReadOnlySpan<nuint> b)
	{
		int size = Math.Max(a.Length, b.Length);
		nuint[] result = new nuint[size];
		for (int i = 0; i < size; i++)
		{
			nuint va = (i < a.Length) ? a[i] : 0u;
			nuint vb = (i < b.Length) ? b[i] : 0u;
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
		nuint[] srcLower = Lower;
		nuint[] srcUpper = Upper;
		nuint[] newLower = new nuint[srcLower.Length];
		Array.Copy(srcLower, newLower, srcLower.Length);
		nuint[] newUpper = new nuint[srcUpper.Length];
		Array.Copy(srcUpper, newUpper, srcUpper.Length);
		return new ReadOnlyEnumCollection<TEnum>(newLower, newUpper, signMask);
	}
	/// <summary>
	/// Returns an enumerator that iterates through the set of values contained in the collection.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public readonly IEnumerator<TEnum> GetEnumerator()
	{
		nuint[] upper = Upper, lower = Lower;
		for (int div = 0; div < lower.Length; div++)
		{
			nuint block = lower[div];
			while (block != 0)
			{
				int rem = TrailingZeroCount(block);
				nuint value = (nuint)(div * bw + rem);
				yield return ToEnum(value, false);
				block &= (block - 1);
			}
		}
		for (int div = upper.Length - 1; div >= 0; div--)
		{
			nuint block = upper[div];
			while (block != 0)
			{
				int rem = TrailingZeroCount(block);
				nuint value = (nuint)(div * bw + rem);
				yield return ToEnum(value, true);
				block &= (block - 1);
			}
		}
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	private static int TrailingZeroCount(nuint v)
	{
#if NET8_0_OR_GREATER
		return System.Numerics.BitOperations.TrailingZeroCount((ulong)v);
#else
		int c = 0;
		while ((v & (nuint)1) == 0)
		{
			v >>= 1;
			c++;
		}
		return c;
#endif
	}
}
