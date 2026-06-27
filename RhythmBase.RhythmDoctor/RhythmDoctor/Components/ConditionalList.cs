using System.Numerics;
using System.Collections;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Represents a list of <see cref="BaseConditional"/> objects that can be accessed by logical indices,
/// allowing for sparse storage and efficient management of conditions.
/// </summary>
public class ConditionalList : ICollection<BaseConditional>
{
	private readonly Level parent;
	private const int _defaultCapacity = 4;
	private const int _ulongSize = sizeof(ulong) * 8;
	private int _count;
	private int[] _physical_index;
	private int[] _logical_index;
	private ulong[] _hasValue;
	private BaseConditional[] _value;
	private int _firstEmptyIndex;
	private int _trailingEmptyIndex;

	/// <summary>
	/// Initializes a new instance of the <see cref="ConditionalList"/> class with default capacity.
	/// </summary>
	internal ConditionalList(Level level)
	{
		_physical_index = new int[_defaultCapacity];
		_logical_index = new int[_defaultCapacity];
		Array.Fill(_logical_index, -1);
		_hasValue = new ulong[1];
		_value = new BaseConditional[_defaultCapacity];
		_firstEmptyIndex = 0;
		_trailingEmptyIndex = 0;
		parent = level;
	}

	private void EnsureCapacity(int minLogicalIndex)
	{
		if (_value.Length > minLogicalIndex)
			return;

		int newCapacity = _value.Length == 0 ? _defaultCapacity : _value.Length * 2;
		while (newCapacity <= minLogicalIndex)
			newCapacity *= 2;

		Array.Resize(ref _value, newCapacity);

		int oldReverseLength = _logical_index.Length;
		Array.Resize(ref _logical_index, newCapacity);
		Array.Fill(_logical_index, -1, oldReverseLength, newCapacity - oldReverseLength);

		if (_physical_index.Length < newCapacity)
			Array.Resize(ref _physical_index, newCapacity);

		int requiredUlongs = minLogicalIndex / _ulongSize + 1;
		if (_hasValue.Length < requiredUlongs)
		{
			int newHasLength = _hasValue.Length * 2;
			while (newHasLength < requiredUlongs)
				newHasLength *= 2;
			Array.Resize(ref _hasValue, newHasLength);
		}
	}

	/// <summary>
	/// Adds a <see cref="BaseConditional"/> item to the collection at the next available logical index.
	/// </summary>
	/// <param name="item">The item to add.</param>
	public void Add(BaseConditional item) => Insert(item, _firstEmptyIndex);

	public bool Insert(BaseConditional item, int index)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		EnsureCapacity(index);
		if (GetHasValue(index))
			return false;

		_value[index] = item;
		item.ParentCollection = this;
		_physical_index[_count] = index;
		_logical_index[index] = _count;
		_count++;
		SetHasValue(index, true);

		if (index >= _trailingEmptyIndex)
			_trailingEmptyIndex = index + 1;

		if (index == _firstEmptyIndex)
			FindNextEmpty();

		return true;
	}
	internal BaseConditional? GetByPhysicalIndex(int physicalIndex)
	{
		if (physicalIndex < 0 || physicalIndex >= _logical_index.Length)
			return null;

		int mapPos = _logical_index[physicalIndex];
		if (mapPos < 0 || mapPos >= _count)
			return null;

		int logicalIndex = _physical_index[mapPos];
		if (logicalIndex != physicalIndex)
			return null;

		return _value[logicalIndex];
	}
	private void FindNextEmpty()
	{
		int startUlong = _firstEmptyIndex / _ulongSize;
		int startBit = _firstEmptyIndex % _ulongSize;

		for (int i = startUlong; i < _hasValue.Length; i++)
		{
			ulong mask = _hasValue[i];
			if (mask == ulong.MaxValue)
			{
				_firstEmptyIndex = (i + 1) * _ulongSize;
				continue;
			}

			if (i == startUlong && startBit > 0)
			{
				ulong shifted = mask >> startBit;
				// 检查高位是否有 0（即是否有空位）
				if (shifted != (ulong.MaxValue >> startBit))
				{
					int trailingOnes1 = BitOperations.TrailingZeroCount(~shifted);
					_firstEmptyIndex = i * _ulongSize + startBit + trailingOnes1;
					return;
				}
				_firstEmptyIndex = (i + 1) * _ulongSize;
				continue;
			}

			int trailingOnes = BitOperations.TrailingZeroCount(~mask);
			_firstEmptyIndex = i * _ulongSize + trailingOnes;
			return;
		}

		_firstEmptyIndex = _hasValue.Length * _ulongSize;
	}

	private void SetHasValue(int index, bool value)
	{
		int ulongIndex = index / _ulongSize;
		int bitIndex = index % _ulongSize;
		if (value)
			_hasValue[ulongIndex] |= (1UL << bitIndex);
		else
			_hasValue[ulongIndex] &= ~(1UL << bitIndex);
	}

	private bool GetHasValue(int index)
	{
		if (index < 0 || index >= _hasValue.Length * _ulongSize)
			return false;
		int ulongIndex = index / _ulongSize;
		int bitIndex = index % _ulongSize;
		return (_hasValue[ulongIndex] & (1UL << bitIndex)) != 0;
	}

	/// <summary>
	/// Removes the <see cref="BaseConditional"/> at the specified logical index from the collection.
	/// </summary>
	/// <param name="logicalIndex">The logical index of the item to remove.</param>
	/// <returns></returns>
	public bool RemoveAt(int logicalIndex)
	{
		if (logicalIndex < 0 || logicalIndex >= _count)
			return false;
		int physicalIndex = _physical_index[logicalIndex];
		foreach (var e in parent)
			e.Condition.SetIndexValueToNull(physicalIndex);
		SetHasValue(physicalIndex, false);
		_value[physicalIndex].ParentCollection = null;
		_value[physicalIndex] = default!;

		for (int i = logicalIndex; i < _count - 1; i++)
		{
			_physical_index[i] = _physical_index[i + 1];
			_logical_index[_physical_index[i]] = i; 
		}
		_count--;
		if (physicalIndex < _firstEmptyIndex)
			_firstEmptyIndex = physicalIndex;

		return true;
	}

	/// <summary>
	/// Determines whether the collection contains a <see cref="BaseConditional"/> at the specified logical index.
	/// </summary>
	/// <param name="logicalIndex">The logical index to check.</param>
	/// <returns></returns>
	public bool Contains(int logicalIndex) => GetHasValue(logicalIndex);

	/// <summary>
	/// Gets or sets the <see cref="BaseConditional"/> at the specified logical index in the collection.
	/// </summary>
	/// <param name="logicalIndex">The logical index of the item to get or set.</param>
	/// <returns></returns>
	public BaseConditional? this[int logicalIndex]
	{
		get => GetHasValue(logicalIndex) ? _value[logicalIndex] : default;
		set
		{
			if (GetHasValue(logicalIndex))
			{
				_value[logicalIndex].ParentCollection = null;
				_value[logicalIndex] = value!;
				value!.ParentCollection = this;
			}
			else
			{
				Insert(value!, logicalIndex);
			}
		}
	}

	/// <inheritdoc/>
	public int Count => _count;

	/// <summary>
	/// Clears all items from the collection, resetting its state.
	/// </summary>
	public void Clear()
	{
		if (_count == 0)
			return;

		for (int i = 0; i < _count; i++)
		{
			int logicalIndex = _physical_index[i];
			_value[logicalIndex].ParentCollection = null;
			_value[logicalIndex] = default!;
		}
		foreach (var e in parent)
		{
			if (!e.Condition.IsEmpty)
				e.Condition.Clear();
		}

		Array.Clear(_hasValue, 0, _hasValue.Length);
		Array.Fill(_logical_index, -1);
		_count = 0;
		_firstEmptyIndex = 0;
		_trailingEmptyIndex = 0;
	}

	/// <summary>
	/// Determines whether the collection contains the specified <see cref="BaseConditional"/> item.
	/// </summary>
	/// <param name="item">The item to locate in the collection.</param>
	/// <returns></returns>
	public bool Contains(BaseConditional item)
	{
		var comparer = EqualityComparer<BaseConditional>.Default;

		for (int i = 0; i < _count; i++)
		{
			int logicalIndex = _physical_index[i];
			if (comparer.Equals(_value[logicalIndex], item))
				return true;
		}

		return false;
	}

	/// <summary>
	/// Copies the elements of the collection to an array, starting at a particular index in the destination array.
	/// </summary>
	/// <param name="array">The array to copy elements to.</param>
	/// <param name="arrayIndex">The index in the destination array to start copying from.</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the array index is out of range.</exception>
	/// <exception cref="ArgumentException">Thrown when the destination array does not have enough space.</exception>
	public void CopyTo(BaseConditional[] array, int arrayIndex)
	{
		ArgumentNullException.ThrowIfNull(array, nameof(array));
		if (arrayIndex < 0 || arrayIndex > array.Length)
			throw new ArgumentOutOfRangeException(nameof(arrayIndex));
		if (array.Length - arrayIndex < _count)
			throw new ArgumentException("Destination array does not have enough space.");

		for (int i = 0; i < _count; i++)
		{
			int logicalIndex = _physical_index[i];
			array[arrayIndex + i] = _value[logicalIndex];
		}
	}

	/// <summary>
	/// Removes the specified <see cref="BaseConditional"/> item from the collection.
	/// </summary>
	/// <param name="item">The item to remove from the collection.</param>
	/// <returns></returns>
	public bool Remove(BaseConditional item)
	{
		var comparer = EqualityComparer<BaseConditional>.Default;

		for (int i = 0; i < _count; i++)
		{
			int logicalIndex = _physical_index[i];
			if (comparer.Equals(_value[logicalIndex], item))
				return RemoveAt(logicalIndex);
		}

		return false;
	}
	public void Defrag()
	{
		if (_count <= 1)
			return;

		int[] oldPhys = new int[_count];
		for (int i = 0; i < _count; i++)
			oldPhys[i] = _physical_index[i];

		int[] remap = new int[_trailingEmptyIndex];
		Array.Fill(remap, -1);

		for (int i = 0; i < _count; i++)
		{
			int oldP = oldPhys[i];
			int newP = i;
			_physical_index[i] = newP;
			remap[oldP] = newP;
		}

		BaseConditional[] newValue = new BaseConditional[_count];
		for (int i = 0; i < _count; i++)
			newValue[i] = _value[oldPhys[i]];

		int newCapacity = Math.Max(_count * 2, _defaultCapacity);
		Array.Resize(ref newValue, newCapacity);
		_value = newValue;

		Array.Fill(_logical_index, -1);
		for (int i = 0; i < _count; i++)
			_logical_index[i] = i;

		_trailingEmptyIndex = _count;
		_firstEmptyIndex = _count;

		foreach (var e in parent)
		{
			if (e.Condition.IsEmpty)
				continue;
			e.Condition.Remap(remap, _trailingEmptyIndex);
		}

		return;
	}
	public int DataIndexOf(BaseConditional item)
	{
		var comparer = EqualityComparer<BaseConditional>.Default;

		for (int i = 0; i < _count; i++)
		{
			int logicalIndex = _physical_index[i];
			if (comparer.Equals(_value[logicalIndex], item))
				return logicalIndex;
		}

		return -1;
	}
	/// <inheritdoc/>
	public IEnumerator<BaseConditional> GetEnumerator()
	{
		for (int i = 0; i < _count; i++)
		{
			int logicalIndex = _physical_index[i];
			yield return _value[logicalIndex];
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	/// <inheritdoc/>
	public bool IsReadOnly => false;
}