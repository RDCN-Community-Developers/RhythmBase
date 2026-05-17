using RhythmBase.RhythmDoctor.Events;
using System.Collections;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>  
/// Represents a collection used to manage <see cref="Row"/> or <see cref="Decoration"/> elements.  
/// </summary>  
/// <typeparam name="T">The type of elements in the collection, constrained to <see cref="OrderedEventCollection{TEvent, EventType, RDBeat}"/>.</typeparam>
/// <typeparam name="TEvent">The type of events contained in the element.</typeparam>  
/// <remarks>  
/// This abstract class provides a base implementation for managing a collection of elements  
/// that are either rows or decorations within a rhythm level.  
/// </remarks>  
/// <param name="parent">The parent <see cref="RDLevel"/> instance associated with this collection.</param>  
/// <param name="limited">A boolean value indicating whether the collection has a fixed size limit.</param>  
public abstract class LevelElementCollection<T, TEvent>(RDLevel parent, bool limited) : ICollection<T>
	where T : OrderedEventCollection<TEvent, EventType, RDBeat>, new()
	where TEvent : IEvent<EventType, RDBeat>
{
	/// <summary>  
	/// The parent <see cref="RDLevel"/> instance associated with this collection.  
	/// </summary>  
	protected private RDLevel parent = parent;
	/// <summary>  
	/// The internal list of items in the collection.  
	/// </summary>  
	protected private readonly List<T> _items = limited ? new(16) : [];
	/// <inheritdoc/>
	public int Count => _items.Count;
	/// <inheritdoc/>
	public bool IsReadOnly { get; internal set; } = false;
	/// <inheritdoc/>
	public abstract void Add(T item);
	/// <inheritdoc/>
	public void Clear() => _items.Clear();
	/// <inheritdoc/>
	public bool Contains(T item) => _items.Contains(item);
	/// <inheritdoc/>
	public void CopyTo(T[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);
	/// <inheritdoc/>
	public IEnumerator<T> GetEnumerator()
	{
		foreach (T item in _items)
			yield return item;
	}
	/// <summary>  
	/// Gets the index of the specified item in the collection.  
	/// </summary>  
	/// <param name="item">The item to locate in the collection.</param>  
	/// <returns>The zero-based index of the item if found; otherwise, -1.</returns>  
	public int IndexOf(T item) => _items.IndexOf(item);
	/// <inheritdoc/>
	public abstract bool Remove(T item);
	/// <summary>  
	/// Retrieves the elements in the collection that are associated with the specified room.  
	/// </summary>  
	/// <param name="room">The <see cref="RDRoom"/> to filter the elements by.</param>  
	/// <returns>A collection of elements associated with the specified room.</returns>  
	public abstract IEnumerable<T> ElementsOf(RDRoom room);
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
