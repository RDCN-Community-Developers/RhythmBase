using System.Collections;

namespace RhythmBase.Global.Components;

/// <summary>
/// A collection that maintains its elements in sorted order based on a key selector function.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class OrderedCollection<TKey, TValue> : ICollection<TValue>
    where TKey : IComparable<TKey>
{
    private readonly Func<TValue, TKey> _selector;
    private int _count;
    private long _sequence;
    private Entry[] _entries;
    private const int DefaultCapacity = 4;
    private readonly struct Entry(TValue value, TKey key, long seq) : IComparable<Entry>
    {
        public readonly TValue Value = value;
        public readonly TKey Key = key;
        public readonly long Seq = seq;
        public int CompareTo(Entry other)
        {
            int keyCompare = Key.CompareTo(other.Key);
            return keyCompare != 0 ? keyCompare : Seq.CompareTo(other.Seq);
        }
    }
    ///<inheritdoc/>
    public int Count => _count;
    ///<inheritdoc/>
    public bool IsReadOnly => false;
    /// <summary>
    /// Initializes a new instance of the OrderedCollection class using the specified key selector function.
    /// </summary>
    /// <remarks>The collection is initialized with a default capacity and contains no
    /// entries.</remarks>
    /// <param name="selector">A function that extracts the key from each value in the collection. This parameter cannot be null.</param>
    /// <exception cref="ArgumentNullException">Thrown if selector is null.</exception>
    public OrderedCollection(Func<TValue, TKey> selector)
    {
        _selector = selector ?? throw new ArgumentNullException(nameof(selector));
        _entries = new Entry[DefaultCapacity];
        _count = 0;
        _sequence = 0;
    }
    /// <summary>
    /// Initializes a new instance of the OrderedCollection class with a specified key selector function and initial
    /// capacity.
    /// </summary>
    /// <remarks>If the specified capacity is less than or equal to zero, the collection uses a
    /// default capacity.</remarks>
    /// <param name="selector">A function that extracts the key from each value in the collection. This parameter cannot be null.</param>
    /// <param name="capacity">The initial number of elements that the collection can contain. Must be a non-negative integer.</param>
    /// <exception cref="ArgumentNullException">Thrown when the selector parameter is null.</exception>
    public OrderedCollection(Func<TValue, TKey> selector, int capacity)
    {
        _selector = selector ?? throw new ArgumentNullException(nameof(selector));
        ArgumentOutOfRangeException.ThrowIfLessThan(0, capacity, nameof(capacity));
        _entries = new Entry[capacity > 0 ? capacity : DefaultCapacity];
        _count = 0;
        _sequence = 0;
    }
    ///<inheritdoc/>
    public void Add(TValue item)
    {
        if (_count == _entries.Length)
        {
            Grow(_count + 1);
        }

        TKey key = _selector(item);
        var newEntry = new Entry(item, key, _sequence++);
        int index = BinarySearchForInsert(newEntry);
        if (index < _count)
        {
            Array.Copy(_entries, index, _entries, index + 1, _count - index);
        }

        _entries[index] = newEntry;
        _count++;
    }
    private int BinarySearchForInsert(Entry entry)
    {
        int lo = 0, hi = _count - 1;
        while (lo <= hi)
        {
            int mid = lo + ((hi - lo) >> 1);
            int cmp = _entries[mid].CompareTo(entry);
            if (cmp < 0) 
                lo = mid + 1;
            else 
                hi = mid - 1;
        }
        return lo;
    }
    ///<inheritdoc/>
    public bool Remove(TValue item)
    {
        TKey key = _selector(item);
        int index = FindIndex(item, key);
        if (index < 0) return false;
        _count--;
        if (index < _count)
        {
            Array.Copy(_entries, index + 1, _entries, index, _count - index);
        }
        _entries[_count] = default;
        return true;
    }
    private int FindIndex(TValue item, TKey key)
    {
        int lo = 0, hi = _count;
        while (lo < hi)
        {
            int mid = lo + ((hi - lo) >> 1);
            if (_entries[mid].Key.CompareTo(key) < 0)
                lo = mid + 1;
            else
                hi = mid;
        }
        int start = lo;
        hi = _count;
        while (lo < hi)
        {
            int mid = lo + ((hi - lo) >> 1);
            if (_entries[mid].Key.CompareTo(key) <= 0)
                lo = mid + 1;
            else
                hi = mid;
        }
        int end = lo;
        var comparer = EqualityComparer<TValue>.Default;
        for (int i = start; i < end; i++)
        {
            if (comparer.Equals(_entries[i].Value, item))
                return i;
        }

        return -1;
    }
    ///<inheritdoc/>
    public bool Contains(TValue item)
    {
        return FindIndex(item, _selector(item)) >= 0;
    }
    ///<inheritdoc/>
    public void Clear()
    {
        if (_count > 0)
        {
            Array.Clear(_entries, 0, _count);
            _count = 0;
        }
        _sequence = 0;
    }
    ///<inheritdoc/>
    public void CopyTo(TValue[] array, int arrayIndex) => array.CopyTo(_entries, arrayIndex);
    ///<inheritdoc/>
    public IEnumerator<TValue> GetEnumerator()
    {
        for (int i = 0; i < _count; i++)
            yield return _entries[i].Value;
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    private void Grow(int requiredCapacity)
    {
        int newCapacity = _entries.Length == 0 ? DefaultCapacity : _entries.Length * 2;
        if (newCapacity < requiredCapacity) newCapacity = requiredCapacity;

        Array.Resize(ref _entries, newCapacity);
    }
    //public IEnumerable<TValue> GetByKey(TKey key)
    //{
    //    int lo = 0, hi = _count;

    //    // 找起始
    //    while (lo < hi)
    //    {
    //        int mid = lo + ((hi - lo) >> 1);
    //        if (_entries[mid].Key.CompareTo(key) < 0)
    //            lo = mid + 1;
    //        else
    //            hi = mid;
    //    }
    //    int start = lo;

    //    // 找结束
    //    hi = _count;
    //    while (lo < hi)
    //    {
    //        int mid = lo + ((hi - lo) >> 1);
    //        if (_entries[mid].Key.CompareTo(key) <= 0)
    //            lo = mid + 1;
    //        else
    //            hi = mid;
    //    }

    //    for (int i = start; i < lo; i++)
    //    {
    //        yield return _entries[i].Value;
    //    }
    //}
}