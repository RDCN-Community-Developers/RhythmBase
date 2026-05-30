using System.Collections;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Represents a collection of <see cref="BaseConditional"/> objects that maintains a relationship between each item
/// and the collection it belongs to.
/// </summary>
public class ConditionalCollection : ICollection<BaseConditional>
{
	private readonly List<BaseConditional> cs = [];
	/// <inheritdoc/>
	public int Count => cs.Count;
	/// <inheritdoc/>
	public bool IsReadOnly => false;
	/// <inheritdoc/>
	public void Add(BaseConditional item)
	{
		cs.Add(item);
		item.ParentCollection = this;
	}
	/// <inheritdoc/>
	public void Clear()
	{
		foreach (BaseConditional item in cs)
			item.ParentCollection = null;
		cs.Clear();
	}
	/// <inheritdoc/>
	public bool Contains(BaseConditional item) => cs.Contains(item);
	/// <inheritdoc/>
	public void CopyTo(BaseConditional[] array, int arrayIndex) => cs.CopyTo(array, arrayIndex);
	/// <inheritdoc/>
	public IEnumerator<BaseConditional> GetEnumerator() => cs.GetEnumerator();
	/// <inheritdoc/>
	public bool Remove(BaseConditional item)
	{
		if (item.ParentCollection == this)
			item.ParentCollection = null;
		return cs.Remove(item);
	}
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	/// <summary>
	/// Returns the zero-based index of the first occurrence of the specified condition in the collection.
	/// </summary>
	/// <param name="condition">The condition to locate in the collection. Cannot be <see langword="null"/>.</param>
	/// <returns>The zero-based index of the first occurrence of <paramref name="condition"/> in the collection,  or -1 if the
	/// condition is not found.</returns>
	public int IndexOf(BaseConditional condition) => cs.IndexOf(condition);
}
