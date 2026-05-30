using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RhythmBase.Global.Components;

public static class CollectionBuilders
{
    public static EnumCollection<TEnum> BuildEnumCollection<TEnum>(ReadOnlySpan<TEnum> values) where TEnum : struct, Enum
    {
        EnumCollection<TEnum> collection = new(values);
        return collection;
    }
    public static ReadOnlyEnumCollection<TEnum> BuildReadOnlyEnumCollection<TEnum>(ReadOnlySpan<TEnum> values) where TEnum : struct, Enum
    {
        ReadOnlyEnumCollection<TEnum> collection = new(values);
        return collection;
    }
}
/// <summary>
/// Represents a compact set-like collection for storing values of an enum type, providing efficient add, remove, and
/// enumeration operations.
/// </summary>
/// <remarks>This collection optimized for scenarios where you need to track membership of enum values
/// with minimal memory overhead. It is suitable for enums with underlying values that fit within the supported range.
/// The collection does not allow duplicate values and does not preserve insertion order. Thread safety is not
/// guaranteed; external synchronization is required for concurrent access.</remarks>
/// <typeparam name="TEnum">The enum type to be stored in the collection. Must be a value type that derives from Enum.</typeparam>
[CollectionBuilder(typeof(CollectionBuilders), nameof(CollectionBuilders.BuildEnumCollection))]
public struct EnumCollection<TEnum> : IEnumerable<TEnum> where TEnum : struct, Enum
{
    private const int bw = sizeof(ulong) * 8;
    private readonly ulong mask;
    internal readonly ulong[] _bits;
    /// <summary>
    /// Initializes a new instance of the EnumCollection class with the specified capacity.
    /// </summary>
    /// <remarks>The capacity determines the initial size of the underlying storage. If more elements are added
    /// than the specified capacity, the collection may need to resize internally, which can impact performance.</remarks>
    /// <param name="capacity">The number of elements that the collection can initially store. Must be greater than zero.</param>
    public EnumCollection(int capacity)
    {
        _bits = new ulong[capacity];
        int byteWidth = Unsafe.SizeOf<TEnum>();
        int bitWidth = byteWidth * 8;
        mask = (bitWidth >= 64) ? ulong.MaxValue : ((1ul << bitWidth) - 1ul);
    }
    /// <summary>
    /// Initializes a new instance of the EnumCollection class for the specified enum type.
    /// </summary>
    /// <remarks>This constructor determines the required storage size based on the maximum value of the provided
    /// enum type. The enum type must have underlying values that fit within the supported bit width; otherwise, an
    /// exception is thrown.</remarks>
    /// <exception cref="InvalidOperationException">Thrown if the maximum value of the enum type exceeds the supported range for internal storage.</exception>
    public EnumCollection()
    {
        ulong enumMax = 0;
        int byteWidth = Unsafe.SizeOf<TEnum>();
        ulong enumMask = (byteWidth * 8 >= 64) ? ulong.MaxValue : ((1ul << (byteWidth * 8)) - 1ul);
        foreach (TEnum value in Enum.GetValues<TEnum>())
        {
            TEnum vt = value;
            ulong v = Convert.ToUInt64(vt) & enumMask;
            if (v > enumMax)
                enumMax = v;
        }
        int size;
        try
        {
            size = checked((int)(enumMax / bw) + 1);
        }
        catch
        {
            throw new InvalidOperationException("The enum value is too big.");
        }
        _bits = new ulong[size];
        mask = enumMask;
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
    /// Initializes a new instance of the collection with the supplied enumerable sequence.
    /// </summary>
    /// <param name="values">An enumerable sequence whose values are immediately added to the collection.</param>
    public EnumCollection(IEnumerable<TEnum> values) : this()
    {
        foreach (TEnum value in values)
            Add(value);
    }
    /// <summary>
    /// Initializes a new instance of the collection with the supplied read-only span.
    /// </summary>
    /// <param name="values">A read-only span whose values are immediately added to the collection.</param>
    public EnumCollection(ReadOnlySpan<TEnum> values) : this()
    {
        foreach (TEnum value in values)
            Add(value);
    }
    private readonly ulong ToUL(TEnum value) => Unsafe.As<TEnum, ulong>(ref value) & mask;
    private static TEnum ToEnum(ulong value) => Unsafe.As<ulong, TEnum>(ref value);
    /// <summary>
    /// Gets the number of elements contained in the collection.
    /// </summary>
    public readonly int Count
    {
        get
        {
            int count = 0;
            foreach (ulong block in _bits)
            {
                ulong b = block;
                while (b != 0)
                {
                    b &= (b - 1);
                    count++;
                }
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
            foreach (ulong block in _bits)
                if (block != 0) return false;
            return true;
        }
    }
    /// <summary>
    /// Attempts to add the specified enum value to the collection if it is not already present.
    /// </summary>
    /// <param name="item">The enum value to add to the collection. Must be within the valid range of the collection.</param>
    /// <returns>true if the value was successfully added; otherwise, false if the value was already present.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if item is outside the valid range of enum values supported by the collection.</exception>
    public readonly bool Add(TEnum item)
    {
        ulong v = ToUL(item);
        int div = (int)(v / bw);
        int rem = (int)(v % bw);
        if (div >= _bits.Length || div < 0) throw new ArgumentOutOfRangeException(nameof(item), "Enum value out of collection range.");
        if ((_bits[div] & (1ul << rem)) != 0)
            return false;
        _bits[div] |= (1ul << rem);
        return true;
    }
    /// <summary>
    /// Removes the specified enumeration value from the collection if it exists.
    /// </summary>
    /// <param name="item">The enumeration value to remove from the collection.</param>
    /// <returns>true if the value was present and successfully removed; otherwise, false.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the specified enumeration value is outside the valid range of the collection.</exception>
    public readonly bool Remove(TEnum item)
    {
        ulong v = ToUL(item);
        int div = (int)(v / bw);
        int rem = (int)(v % bw);
        if (div >= _bits.Length || div < 0) throw new ArgumentOutOfRangeException(nameof(item), "Enum value out of collection range.");
        if ((_bits[div] & (1ul << rem)) == 0)
            return false;
        _bits[div] &= ~(1ul << rem);
        return true;
    }
    /// <summary>
    /// Determines whether the specified enumeration value is present in the current set.
    /// </summary>
    /// <param name="type">The enumeration value to locate within the set.</param>
    /// <returns>true if the specified value is contained in the set; otherwise, false.</returns>
    public readonly bool Contains(TEnum type)
    {
        ulong v = ToUL(type);
        int div = (int)(v / bw);
        int rem = (int)(v % bw);
        return div >= _bits.Length || div < 0 ? false : (_bits[div] & (1ul << rem)) != 0;
    }
    /// <summary>
    /// Determines whether the collection contains any of the specified enumeration values.
    /// </summary>
    /// <param name="types">An array of enumeration values to check for presence in the collection. Can be empty.</param>
    /// <returns>true if at least one of the specified enumeration values is contained in the collection; otherwise, false.</returns>
    public readonly bool ContainsAny(params TEnum[] types)
    {
        foreach (TEnum type in types)
            if (Contains(type)) return true;
        return false;
    }
    /// <summary>
    /// Determines whether the collection contains any of the specified enumeration values.
    /// </summary>
    /// <param name="types">A parameter array of enumeration value collections to check for presence in the collection. Each enumerable may
    /// contain one or more values to test.</param>
    /// <returns>true if at least one of the specified enumeration values is present in the collection; otherwise, false.</returns>
    public readonly bool ContainsAny(params IEnumerable<TEnum> types)
    {
        foreach (TEnum type in types)
            if (Contains(type)) return true;
        return false;
    }
    /// <summary>
    /// Determines whether the collection contains any of the specified enumeration values.
    /// </summary>
    /// <param name="types">A parameter array of enumeration value collections to check for presence in the collection. Each enumerable may
    /// contain one or more values to test.</param>
    /// <returns>true if at least one of the specified enumeration values is present in the collection; otherwise, false.</returns>
    public readonly bool ContainsAny(EnumCollection<TEnum> types)
    {
        for (int i = 0; i < Math.Min(_bits.Length, types._bits.Length); i++)
            if ((_bits[i] & types._bits[i]) != 0)
                return true;
        return false;
    }
    /// <summary>
    /// Determines whether all specified enumeration values are contained within the current set.
    /// </summary>
    /// <param name="types">An array of enumeration values to check for containment. Cannot be null. The method returns false if any value in
    /// the array is not present.</param>
    /// <returns>true if all specified enumeration values are contained; otherwise, false.</returns>
    public readonly bool ContainsAll(params TEnum[] types)
    {
        foreach (TEnum type in types)
            if (!Contains(type)) return false;
        return true;
    }
    /// <summary>
    /// Determines whether all specified enumeration values are contained within the current set.
    /// </summary>
    /// <remarks>If any collection in the parameter array is null, the method may throw an exception. The method
    /// returns false as soon as a missing value is found, which may improve performance for large sets.</remarks>
    /// <param name="types">A parameter array of enumeration value collections to check for containment. Each collection must not be null.</param>
    /// <returns>true if every enumeration value in all provided collections is contained; otherwise, false.</returns>
    public readonly bool ContainsAll(params IEnumerable<TEnum> types)
    {
        foreach (TEnum type in types)
            if (!Contains(type)) return false;
        return true;
    }
    /// <summary>
    /// Determines whether all specified enumeration values are contained within the current set.
    /// </summary>
    /// <remarks>If any collection in the parameter array is null, the method may throw an exception. The method
    /// returns false as soon as a missing value is found, which may improve performance for large sets.</remarks>
    /// <param name="types">A parameter array of enumeration value collections to check for containment. Each collection must not be null.</param>
    /// <returns>true if every enumeration value in all provided collections is contained; otherwise, false.</returns>
    public readonly bool ContainsAll(EnumCollection<TEnum> types)
    {
        for (int i = 0; i < Math.Min(_bits.Length, types._bits.Length); i++)
        {
            ulong tBits = types._bits[i];
            if (tBits != 0 && (_bits[i] & tBits) != tBits)
                return false;
        }
        return true;
    }
    /// <summary>
    /// Returns a new collection containing the elements that exist in both this collection and the specified collection.
    /// </summary>
    /// <param name="other">The collection to compare with the current collection. Only elements present in both collections will be included
    /// in the result.</param>
    /// <returns>An <see cref="EnumCollection{TEnum}"/> containing the intersection of the two collections. The result will be empty if there are
    /// no common elements.</returns>
    public readonly EnumCollection<TEnum> Intersect(EnumCollection<TEnum> other)
    {
        int size = Math.Min(_bits.Length, other._bits.Length);
        EnumCollection<TEnum> result = new(size);
        for (int i = 0; i < size; i++)
            result._bits[i] = _bits[i] & other._bits[i];
        return result;
    }
    /// <summary>
    /// Returns a new collection containing the elements of this collection that are not present in the specified
    /// collection.
    /// </summary>
    /// <param name="other">The collection whose elements should be excluded from the result. Cannot be null.</param>
    /// <returns>A new <see cref="EnumCollection{TEnum}"/> containing the elements that exist in this collection but not in
    /// <paramref name="other"/>.</returns>
    public readonly EnumCollection<TEnum> Except(EnumCollection<TEnum> other)
    {
        int size = Math.Min(_bits.Length, other._bits.Length);
        EnumCollection<TEnum> result = new(_bits.Length);
        for (int i = 0; i < size; i++)
            result._bits[i] = _bits[i] & ~other._bits[i];
        for (int i = size; i < _bits.Length; i++)
            result._bits[i] = _bits[i];
        return result;
    }
    /// <summary>
    /// Returns a new collection containing the symmetric difference between this collection and the specified collection.
    /// </summary>
    /// <param name="other">The collection to compare with the current collection. The symmetric difference will include elements present in
    /// either collection but not in both.</param>
    /// <returns>An <see cref="EnumCollection{TEnum}"/> containing elements that are present in either this collection or <paramref
    /// name="other"/>, but not in both.</returns>
    public readonly EnumCollection<TEnum> SymmetricExcept(EnumCollection<TEnum> other)
    {
        int size = Math.Max(_bits.Length, other._bits.Length);
        EnumCollection<TEnum> result = new(size);
        for (int i = 0; i < size; i++)
        {
            ulong a = (i < _bits.Length) ? _bits[i] : 0ul;
            ulong b = (i < other._bits.Length) ? other._bits[i] : 0ul;
            result._bits[i] = a ^ b;
        }
        return result;
    }
    /// <summary>
    /// Returns a new collection that contains all elements present in either this collection or the specified collection.
    /// </summary>
    /// <param name="other">The collection whose elements will be combined with those of this collection. Cannot be null.</param>
    /// <returns>An <see cref="EnumCollection{TEnum}"/> containing the union of elements from this collection and <paramref
    /// name="other"/>.</returns>
    public readonly EnumCollection<TEnum> Union(EnumCollection<TEnum> other)
    {
        int size = Math.Max(_bits.Length, other._bits.Length);
        EnumCollection<TEnum> result = new(size);
        for (int i = 0; i < size; i++)
        {
            ulong a = (i < _bits.Length) ? _bits[i] : 0ul;
            ulong b = (i < other._bits.Length) ? other._bits[i] : 0ul;
            result._bits[i] = a | b;
        }
        return result;
    }
    /// <summary>
    /// Returns a read-only wrapper for the current collection.
    /// </summary>
    /// <remarks>The returned collection reflects changes made to the underlying collection. To prevent
    /// modifications, ensure that the original collection is not altered after creating the read-only wrapper.</remarks>
    /// <returns>A <see cref="ReadOnlyEnumCollection{TEnum}"/> that provides a read-only view of the current collection.</returns>
    public readonly ReadOnlyEnumCollection<TEnum> AsReadOnly()
    {
        return new ReadOnlyEnumCollection<TEnum>(this);
    }
    /// <summary>
    /// Returns an enumerator that iterates through the set of values contained in the collection.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public readonly IEnumerator<TEnum> GetEnumerator()
    {
        for (int div = 0; div < _bits.Length; div++)
        {
            ulong block = _bits[div];
            while (block != 0)
            {
                int rem = TrailingZeroCount(block);
                ulong value = (ulong)(div * bw + rem);
                yield return ToEnum(value);
                block &= (block - 1);
            }
        }
    }
    [EditorBrowsable(EditorBrowsableState.Never)]
    readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    private static int TrailingZeroCount(ulong v)
    {
#if NET8_0_OR_GREATER
		return System.Numerics.BitOperations.TrailingZeroCount(v);
#else
        int c = 0;
        while ((v & 1ul) == 0)
        {
            v >>= 1;
            c++;
        }
        return c;
#endif
    }
}
