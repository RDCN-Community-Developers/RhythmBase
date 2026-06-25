using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace RhythmBase.BeatBlock.Components;

/// <summary>
/// Represents a collection of chart variants in a BeatBlock level.
/// </summary>
public class ChartCollection : ICollection<Chart>
{
	internal Level _parent;
	/// <summary>
	/// Gets the default chart variant.
	/// </summary>
	public Chart Default { get; private set; } = new() { isDefault = true };
	internal readonly List<Chart> _variants = [];
	/// <inheritdoc/>
	public int Count => _variants.Count;
	/// <inheritdoc/>
	public bool IsReadOnly { get; } = false;
	/// <summary>
	/// Gets or sets the chart at the specified index.
	/// </summary>
	/// <param name="index">The zero-based index of the chart.</param>
	/// <returns>The <see cref="Chart"/> at the specified index.</returns>
	public Chart this[int index]
	{
		get => _variants[index];
		set
		{
			value._parent = this;
			_variants[index] = value;
		}
	}
	/// <summary>
	/// Gets or sets the chart with the specified name.
	/// </summary>
	/// <param name="name">The name of the chart.</param>
	/// <returns>The <see cref="Chart"/> with the specified name.</returns>
	public Chart this[string name]
	{
		get => _variants.FirstOrDefault(v => v.Name == name)
				?? throw new KeyNotFoundException($"No variant with the name '{name}' was found.");
		set
		{
			var index = _variants.FindIndex(v => v.Name == name);
			value._parent = this;
			if (index == -1)
				_variants.Add(value);
			else
				_variants[index] = value;
		}
	}
	/// <summary>
	/// Tries to get a chart variant by name.
	/// </summary>
	/// <param name="name">The name of the variant to find.</param>
	/// <param name="variant">When this method returns, contains the chart with the specified name, or <see langword="null"/> if no such chart was found.</param>
	/// <returns><see langword="true"/> if a chart with the specified name was found; otherwise, <see langword="false"/>.</returns>
	public bool TryGetVariant(string name, [NotNullWhen(true)] out Chart? variant)
	{
		variant = _variants.FirstOrDefault(v => v.Name == name);
		return variant != null;
	}
	/// <inheritdoc/>
	public ChartCollection(Level level)
	{
		_parent = level;
	}

	/// <inheritdoc/>
	public void Clear()
	{
		_variants.Clear();
	}

	/// <inheritdoc/>
	public bool Contains(Chart item)
	{
		return _variants.Contains(item);
	}

	/// <inheritdoc/>
	public void CopyTo(Chart[] array, int arrayIndex)
	{
		_variants.CopyTo(array, arrayIndex);
	}

	/// <inheritdoc/>
	public bool Remove(Chart item)
	{
		item._parent = null;
		return _variants.Remove(item);
	}

	/// <inheritdoc/>
	public IEnumerator<Chart> GetEnumerator()
	{
		return _variants.GetEnumerator();
	}

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	/// <inheritdoc/>
	public void Add(Chart item)
	{
		item._parent = this;
		_variants.Add(item);
	}
}