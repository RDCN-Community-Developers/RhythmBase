using System.Numerics;
using System.Collections;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Represents a list of <see cref="BaseConditional"/> objects that can be accessed by logical indices,
/// allowing for sparse storage and efficient management of conditions.
/// </summary>
public class ConditionalList : ICollection<BaseConditional>
{
	private const int _defaultCapacity = 4;
	private const int _ulongSize = sizeof(ulong) * 8;
	private int _count;
	private int[] _valueIndexMap;
	private int[] _reverseMap;
	private ulong[] _hasValue;
	private BaseConditional[] _value;
	private int _firstEmptyIndex;
	private int _trailingEmptyIndex;

	/// <summary>
	/// Initializes a new instance of the <see cref="ConditionalList"/> class with default capacity.
	/// </summary>
	internal ConditionalList(Level level)
	{
		_valueIndexMap = new int[_defaultCapacity];
		_reverseMap = new int[_defaultCapacity];
		Array.Fill(_reverseMap, -1);
		_hasValue = new ulong[1];
		_value = new BaseConditional[_defaultCapacity];
		_firstEmptyIndex = 0;
		_trailingEmptyIndex = 0;
	}

	private void EnsureCapacity(int minLogicalIndex)
	{
		if (_value.Length > minLogicalIndex)
			return;

		int newCapacity = _value.Length == 0 ? _defaultCapacity : _value.Length * 2;
		while (newCapacity <= minLogicalIndex)
			newCapacity *= 2;

		Array.Resize(ref _value, newCapacity);

		int oldReverseLength = _reverseMap.Length;
		Array.Resize(ref _reverseMap, newCapacity);
		Array.Fill(_reverseMap, -1, oldReverseLength, newCapacity - oldReverseLength);

		if (_valueIndexMap.Length < newCapacity)
			Array.Resize(ref _valueIndexMap, newCapacity);

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

	public bool Insert(BaseConditional item, int logicalIndex)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(logicalIndex);
		EnsureCapacity(logicalIndex);
		if (GetHasValue(logicalIndex))
			return false;

		_value[logicalIndex] = item;
		item.ParentCollection = this;
		_valueIndexMap[_count] = logicalIndex;
		_reverseMap[logicalIndex] = _count;
		_count++;
		SetHasValue(logicalIndex, true);

		if (logicalIndex >= _trailingEmptyIndex)
			_trailingEmptyIndex = logicalIndex + 1;

		if (logicalIndex == _firstEmptyIndex)
			FindNextEmpty();

		return true;
	}
	internal BaseConditional? GetByPhysicalIndex(int physicalIndex)
	{
		if (physicalIndex < 0 || physicalIndex >= _reverseMap.Length)
			return null;

		int mapPos = _reverseMap[physicalIndex];
		if (mapPos < 0 || mapPos >= _count)
			return null;

		int logicalIndex = _valueIndexMap[mapPos];
		if (logicalIndex != physicalIndex)
			return null;

		return _value[logicalIndex];
	}
	public BaseConditional?[] GetConditionals(Condition condition)
	{
		BaseConditional?[] result = new BaseConditional?[condition.Count];
		for (int i = 0; i < condition.Indices.Length; i++)
		{
			int index = condition.Indices[i];
			result[i] = GetByPhysicalIndex(index);
		}
		return result;
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
		if (logicalIndex < 0 || !GetHasValue(logicalIndex))
			return false;

		SetHasValue(logicalIndex, false);
		_value[logicalIndex].ParentCollection = null;
		_value[logicalIndex] = default!;

		int mapPos = _reverseMap[logicalIndex];
		int lastPos = --_count;

		if (mapPos != lastPos)
		{
			int lastLogicalIndex = _valueIndexMap[lastPos];
			_valueIndexMap[mapPos] = lastLogicalIndex;
			_reverseMap[lastLogicalIndex] = mapPos;
		}

		_reverseMap[logicalIndex] = -1;

		if (logicalIndex < _firstEmptyIndex)
			_firstEmptyIndex = logicalIndex;

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
			int logicalIndex = _valueIndexMap[i];
			_value[logicalIndex].ParentCollection = null;
			_value[logicalIndex] = default!;
		}

		Array.Clear(_hasValue, 0, _hasValue.Length);
		Array.Fill(_reverseMap, -1);
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
			int logicalIndex = _valueIndexMap[i];
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
			int logicalIndex = _valueIndexMap[i];
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
			int logicalIndex = _valueIndexMap[i];
			if (comparer.Equals(_value[logicalIndex], item))
				return RemoveAt(logicalIndex);
		}

		return false;
	}
	public void Defrag()
	{
		if (_count <= 1)
			return;
		Span<int> span = _valueIndexMap.AsSpan(0, _count);
		span.Sort();
		for (int i = 0; i < _count; i++)
			_reverseMap[_valueIndexMap[i]] = i;
		for (int i = _count; i < _reverseMap.Length; i++)
		{
			if (_reverseMap[i] >= 0)
				_reverseMap[i] = -1;
		}
	}
	public int DataIndexOf(BaseConditional item)
	{
		var comparer = EqualityComparer<BaseConditional>.Default;

		for (int i = 0; i < _count; i++)
		{
			int logicalIndex = _valueIndexMap[i];
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
			int logicalIndex = _valueIndexMap[i];
			yield return _value[logicalIndex];
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	/// <inheritdoc/>
	public bool IsReadOnly => false;
}