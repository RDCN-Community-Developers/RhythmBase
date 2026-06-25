////using RhythmBase.Global.Linq;
//using RhythmBase.RhythmDoctor;
//using RhythmBase.RhythmDoctor.Components;
//using RhythmBase.RhythmDoctor.Converters;
//using RhythmBase.RhythmDoctor.Events;
////using System.Collections;
////using System.ComponentModel;

//namespace RhythmBase.Global.Components;


///// <summary>
///// Represents a collection of events that can be enumerated.
///// </summary>
///// <typeparam name="TEvent">The type of events in the collection. Must implement <see cref="IBaseEvent"/>.</typeparam>
//public interface IEventEnumerable<out TEvent>
//		: IEnumerable<TEvent>
//		where TEvent : IBaseEvent
//{
//	/// <summary>
//	/// Gets the events organized by beat order.
//	/// </summary>
//	RedBlackTree<TickTime, TypedEventCollection> EventsBeatOrder { get; }
//	/// <summary>
//	/// Gets the bounds of the events in the collection.
//	/// </summary>
//	public TickTimeRange Range { get; }
//	/// <summary>
//	/// Gets the type of events in the collection.
//	/// </summary>
//	public ReadOnlyEnumCollection<EventType> Types { get; }
//}


///// <summary>
///// Represents a collection of typed events.
///// </summary>
///// <typeparam name="TType">The type of the event type. Must be a struct and an enum.</typeparam>
///// <typeparam name="TBeat">The type of the beat. Must be a struct and implement <see cref="TickTime"/>.</typeparam>
//public class TypedEventCollection : IEnumerable<IBaseEvent>
//{
//	/// <summary>
//	/// Gets the number of events in the collection.
//	/// </summary>
//	public int Count => list.Count;
//	/// <summary>
//	/// Initializes a new instance of the <see cref="TypedEventCollection"/> class.
//	/// </summary>
//	public TypedEventCollection() { }
//	/// <summary>
//	/// Adds an event to the collection.
//	/// </summary>
//	/// <param name="item">The event to add.</param>
//	/// <returns>true if the event was added; otherwise, false if it already exists.</returns>
//	public virtual bool Add(IBaseEvent item)
//	{
//		if (list.Contains(item))
//			return false;
//		list.Add(item);
//		_types.Add(item.Type);
//		return true;
//	}
//	/// <summary>
//	/// Removes an event from the collection.
//	/// </summary>
//	/// <param name="item">The event to remove.</param>
//	/// <returns>true if the event was removed; otherwise, false.</returns>
//	public virtual bool Remove(IBaseEvent item)
//	{
//		bool result = list.Remove(item);
//		if (!result)
//			return false;
//		if (!list.Any(i => i.Type == item.Type))
//			_types.Remove(item.Type);
//		return true;
//	}
//	/// <summary>Determines whether this bucket contains any event of the specified type.</summary>
//	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
//	public bool ContainsType(IBaseEvent type) => _types.Contains(type.Type);
//	/// <summary>Determines whether this bucket contains any event of the specified types.</summary>
//	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
//	public bool ContainsTypes(IBaseEvent[] types) => _types.ContainsAny(types.Select(t => t.Type).ToArray());
//	/// <summary>Determines whether this bucket contains any event matching the given type collection.</summary>
//	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
//	public bool ContainsTypes(ReadOnlyEnumCollection<EventType> types) => _types.AsReadOnly().ContainsAny(types);
//	/// <summary>Compares the insertion order of two events within this bucket.</summary>
//	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
//	public bool CompareTo(IBaseEvent item1, IBaseEvent item2) =>
//			list.IndexOf(item1) < list.IndexOf(item2);
//	/// <summary>
//	/// Returns a string that represents the current collection.
//	/// </summary>
//	/// <returns>A string that represents the current collection.</returns>
//	public override string ToString()
//	{
//		string result = $"Count={list.Count}";
//		return result;
//	}
//	/// <summary>
//	/// Returns an enumerator that iterates through the collection.
//	/// </summary>
//	/// <returns>An enumerator that can be used to iterate through the collection.</returns>
//	public IEnumerator<IBaseEvent> GetEnumerator() =>
//			list.GetEnumerator();
//	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() =>
//			list.GetEnumerator();
//	private readonly List<IBaseEvent> list = [];
//	private readonly EnumCollection<EventType> _types = [];
//}

///// <summary>
///// Provides an enumerator over events in a <see cref="RedBlackTree{TKey, TValue}"/> collection, filtered by
///// event types and an optional tick range.
///// </summary>
///// <typeparam name="TEvent">The event interface type to enumerate.</typeparam>
//public class EventEnumerator<TEvent>(RedBlackTree<TickTime, TypedEventCollection> collection, ReadOnlyEnumCollection<EventType> types, TickTimeRange range)
//		: IEventEnumerable<TEvent>, IEnumerator<TEvent>
//	where TEvent : IBaseEvent
//{
//	/// <summary>
//	/// The enumerator over beat-keyed buckets in the tree.
//	/// </summary>
//	protected readonly IEnumerator<KeyValuePair<TickTime, TypedEventCollection>> beats = collection.GetEnumerator();
//	/// <summary>
//	/// The enumerator over events within the current bucket, or <c>null</c> if no bucket is active.
//	/// </summary>
//	protected IEnumerator<IBaseEvent>? events;
//	/// <summary>
//	/// The underlying tree collection.
//	/// </summary>
//	protected readonly RedBlackTree<TickTime, TypedEventCollection> collection = collection;
//	private ReadOnlyEnumCollection<EventType> types = types;
//	private TickTimeRange range = range;
//	/// <inheritdoc/>
//	public TEvent Current => ((TEvent?)events?.Current) ?? throw new InvalidOperationException();
//	/// <inheritdoc/>
//	object System.Collections.IEnumerator.Current => Current;
//	RedBlackTree<TickTime, TypedEventCollection> IEventEnumerable<TEvent>.EventsBeatOrder => collection;
//	/// <inheritdoc/>
//	public bool MoveNext()
//	{
//		if (events != null)
//		{
//			while (events.MoveNext())
//			{
//				if (types.Contains(events.Current.Type))
//					return true;
//			}
//			events = null;
//		}
//		while (beats.MoveNext())
//		{
//			var currentKey = beats.Current.Key;

//			if (range.Start.HasValue && currentKey.CompareTo(range.Start.Value) < 0)
//				continue;
//			if (range.End.HasValue && currentKey.CompareTo(range.End.Value) >= 0)
//				return false;
//			if (!beats.Current.Value.ContainsTypes(types))
//				continue;
//			events = beats.Current.Value.GetEnumerator();
//			while (events.MoveNext())
//			{
//				if (types.Contains(events.Current.Type))
//					return true;
//			}
//			events = null;
//		}
//		return false;
//	}
//	private bool _disposed = false;
//	/// <summary>
//	/// Disposes the enumerator, releasing any resources held by the beat and event enumerators.
//	/// </summary>
//	/// <param name="disposing"></param>
//	protected virtual void Dispose(bool disposing)
//	{
//		if (_disposed) return;
//		if (disposing)
//		{
//			beats.Dispose();
//			events?.Dispose();
//		}
//		_disposed = true;
//	}
//	/// <inheritdoc/>
//	public void Dispose()
//	{
//		Dispose(true);
//		GC.SuppressFinalize(this);
//	}
//	/// <inheritdoc/>
//	public void Reset() => throw new NotSupportedException();
//	/// <summary>
//	/// Narrows the enumeration to events of the specified target type, intersecting the current type filter.
//	/// </summary>
//	/// <typeparam name="TTarget">The more specific event type to filter for.</typeparam>
//	/// <returns>A new <see cref="IEventEnumerable{TEvent, EventType, TickTime}"/> with the narrowed filter.</returns>
//	public IEventEnumerable<TTarget> OfEvent<TTarget>() where TTarget : IBaseEvent
//	{
//		ReadOnlyEnumCollection<EventType> types = this.types.Intersect(EventTypeRegistry.ToEnums<TTarget>());
//		this.types = types;
//		return new EventEnumerator<TTarget>(collection, types, range);
//	}
//	/// <summary>
//	/// Replaces the current type filter with the specified collection of event types.
//	/// </summary>
//	/// <param name="types">The event types to enumerate.</param>
//	/// <returns>This enumerator with the updated filter.</returns>
//	public IEventEnumerable<IBaseEvent> OfEvents(ReadOnlyEnumCollection<EventType> types)
//	{
//		this.types = types;
//		return new EventEnumerator<IBaseEvent>(collection, types, range);
//	}
//	/// <summary>
//	/// Narrows the enumeration to the specified tick range, intersecting with the current range.
//	/// </summary>
//	/// <param name="range">The tick range to filter for.</param>
//	/// <returns>This enumerator with the narrowed range.</returns>
//	public IEventEnumerable<TEvent> InRange(TickTimeRange range)
//	{
//		this.range = this.range.Intersect(range);
//		return this;
//	}
//	/// <summary>
//	/// Returns all events at the specified beat that match the current type filter.
//	/// </summary>
//	/// <param name="beat">The beat at which to retrieve events.</param>
//	/// <returns>An enumerable of matching events at the given beat.</returns>
//	public IEnumerable<IBaseEvent> AtBeat(TickTime beat)
//	{
//		if (!range.Contains(beat))
//			yield break;
//		if (!collection.TryGetValue(beat, out var events))
//			yield break;
//		if (!events.ContainsTypes(types))
//			yield break;
//		foreach (var ev in events)
//			if (types.Contains(ev.Type))
//				yield return ev;
//	}
//	/// <inheritdoc/>
//	public IEnumerator<TEvent> GetEnumerator() => this;
//	/// <inheritdoc/>
//	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
//}

///// <summary>  
///// Represents a collection of ordered events.  
///// </summary>  
///// <typeparam name="TEvent">The type of event.</typeparam>  
///// <typeparam name="EventType">The type of event type.</typeparam>
///// <typeparam name="TickTime">The type of beat.</typeparam>
///// 
//public abstract class OrderedEventCollection<TEvent> : ICollection<TEvent>, IEventEnumerable<TEvent>
//		where TEvent : IBaseEvent
//{
//	/// <summary>
//	/// Initializes a new instance of the <see cref="OrderedEventCollection{TEvent, EventType, TickTime}"/> class.
//	/// </summary>
//	public OrderedEventCollection()
//	{
//		EventsBeatOrder = [];
//		IsReadOnly = false;
//	}
//	/// <summary>  
//	/// Concatenates all events in the collection.  
//	/// </summary>  
//	/// <returns>An <see cref="IEnumerable{TEvent}"/> that contains all events in the collection.</returns>  
//	public IEnumerable<TEvent> ConcatAll() => EventsBeatOrder.SelectMany(i => i.Value).Cast<TEvent>();
//	void ICollection<TEvent>.Add(TEvent item) => Add(item);
//	/// <inheritdoc/>  
//	public override string ToString() => $"Count = {Count}";
//	///// <inheritdoc/>
//	//public override IEnumerator<IBaseEvent> GetEnumerator() => (IEnumerator<IBaseEvent>)new EventEnumerator<TEvent>(this);
//	/// <inheritdoc/>  
//	public IEnumerator<TEvent> GetEnumerator()
//	{
//		foreach (KeyValuePair<TickTime, TypedEventCollection> pair in EventsBeatOrder)
//			foreach (TEvent item in pair.Value)
//				yield return item;
//	}
//	/// <summary>
//	/// Gets the total count of events in the collection.
//	/// </summary>
//	public virtual int Count => EventsBeatOrder.Sum(i => i.Value.Count);
//	/// <summary>
//	/// Gets a value indicating whether the collection is read-only.
//	/// </summary>
//	public bool IsReadOnly { get; }
//	/// <summary>
//	/// Returns the beat of the last event.
//	/// </summary>
//	/// <returns>The beat of the last event.</returns>
//	public TickTime Duration => EventsBeatOrder.LastOrDefault().Key;

//	/// <summary>
//	/// Initializes a new instance of the <see cref="OrderedEventCollection{TEvent, EventType, TickTime}"/> class with the specified items.
//	/// </summary>
//	/// <param name="items">The items to add to the collection.</param>
//	public OrderedEventCollection(IEnumerable<TEvent> items)
//	{
//		EventsBeatOrder = [];
//		IsReadOnly = false;
//		foreach (TEvent item in items)
//			Add(item);
//	}
//	/// <summary>
//	/// Adds an event to the collection.
//	/// </summary>
//	/// <param name="item">The event to add.</param>
//	public virtual bool Add(TEvent item)
//	{
//		if (EventsBeatOrder.TryGetValue(item.TickTime, out TypedEventCollection? value))
//			return value.Add(item);
//		EventsBeatOrder.Insert(item.TickTime, [item]);
//		return true;
//	}
//	/// <summary>
//	/// Clears all events from the collection.
//	/// </summary>
//	public void Clear() => EventsBeatOrder.Clear();
//	/// <summary>
//	/// Determines whether the collection contains a specific event.
//	/// </summary>
//	/// <param name="item">The event to locate in the collection.</param>
//	/// <returns>true if the event is found in the collection; otherwise, false.</returns>
//	public virtual bool Contains(TEvent item) => EventsBeatOrder.FindNode(item.TickTime)?.Value.Contains(item) ?? false;
//	/// <summary>
//	/// Copies the elements of the collection to an array, starting at a particular array index.
//	/// </summary>
//	/// <param name="array">The array to copy the elements to.</param>
//	/// <param name="arrayIndex">The zero-based index in the array at which copying begins.</param>
//	public void CopyTo(TEvent[] array, int arrayIndex)
//	{
//		if (arrayIndex < 0 || arrayIndex > array.Length)
//			throw new ArgumentOutOfRangeException(nameof(arrayIndex));
//		if (array.Length - arrayIndex < Count)
//			throw new ArgumentException("The number of elements in the source collection is greater than the available space from arrayIndex to the end of the destination array.");
//		foreach (KeyValuePair<TickTime, TypedEventCollection> pair in EventsBeatOrder)
//		{
//			foreach (TEvent item in pair.Value)
//			{
//				array[arrayIndex++] = item;
//			}
//		}
//	}
//	/// <summary>
//	/// Removes the first occurrence of a specific event from the collection.
//	/// </summary>
//	/// <param name="item">The event to remove from the collection.</param>
//	/// <returns>true if the event was successfully removed; otherwise, false.</returns>
//	public virtual bool Remove(TEvent item)
//	{
//		bool Remove;
//		if (Contains(item))
//		{
//			bool result = EventsBeatOrder[item.TickTime].Remove(item);
//			if (EventsBeatOrder[item.TickTime].Count == 0)
//				EventsBeatOrder.Remove(item.TickTime);
//			Remove = result;
//		}
//		else
//			Remove = false;
//		return Remove;
//	}
//	internal IEnumerator<TEvent> GetEnumerator(TickTime? start, TickTime? end) => new EventEnumerator<TEvent>(EventsBeatOrder, Types, new TickTimeRange(start, end));
//	/// <summary>
//	/// Returns an enumerator that iterates through the collection.
//	/// </summary>
//	/// <returns>An enumerator for the collection.</returns>
//	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
//	/// <summary>
//	/// Removes the first occurrence of a specific event from the collection.
//	/// </summary>
//	/// <param name="item">The event to remove from the collection.</param>
//	/// <returns>true if the event was successfully removed; otherwise, false.</returns>
//	bool ICollection<TEvent>.Remove(TEvent item) => throw new NotImplementedException();
//	/// <summary>
//	/// The dictionary that maintains the order of events based on their beats.
//	/// </summary>
//	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
//	public RedBlackTree<TickTime, TypedEventCollection> EventsBeatOrder = [];
//	RedBlackTree<TickTime, TypedEventCollection> IEventEnumerable<TEvent>.EventsBeatOrder => EventsBeatOrder;
//	/// <summary>Gets the collection of event types supported by this collection.</summary>
//	internal protected abstract ReadOnlyEnumCollection<EventType> Types { get; }

//	/// <summary>Gets the event types associated with the specified target type.</summary>
//	/// <typeparam name="TTarget">The target event type to query types for.</typeparam>
//	/// <returns>A <see cref="ReadOnlyEnumCollection{T}"/> of event types for the target.</returns>
//	internal protected abstract ReadOnlyEnumCollection<EventType> TypesOf<TTarget>() where TTarget : IEvent<EventType, TickTime>;
//}

