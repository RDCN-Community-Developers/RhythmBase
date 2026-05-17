using RhythmBase.Global.Extensions;
using RhythmBase.Global.Events;
using static RhythmBase.Global.Extensions.Extensions;
using System.Collections;
using RhythmBase.RhythmDoctor.Linq;
using RhythmBase.Global.Linq;
namespace RhythmBase.Global.Components;

/// <summary>  
/// Represents a collection of ordered events.  
/// </summary>  
/// <typeparam name="TEvent">The type of event.</typeparam>  
/// <typeparam name="TType">The type of event type.</typeparam>
/// <typeparam name="TBeat">The type of beat.</typeparam>
/// 
public abstract class OrderedEventCollection<TEvent, TType, TBeat> : ICollection<TEvent>, IEventEnumerable<TEvent, TType, TBeat>
    where TEvent : IEvent<TType, TBeat>
    where TType : struct, Enum
    where TBeat : struct, IBeat<TBeat>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrderedEventCollection{TEvent, TType, TBeat}"/> class.
    /// </summary>
    public OrderedEventCollection()
    {
        eventsBeatOrder = [];
        IsReadOnly = false;
    }
    ///// <summary>  
    ///// Initializes a new instance of the <see cref="OrderedEventCollection{TEvent, TType, TBeat}"/> class with the specified items.  
    ///// </summary>  
    ///// <param name="items">The items to add to the collection.</param>  
    //public OrderedEventCollection(IEnumerable<TEvent> items)
    //{
    //	foreach (TEvent item in items)
    //		Add(item);
    //}
    /// <summary>  
    /// Concatenates all events in the collection.  
    /// </summary>  
    /// <returns>An <see cref="IEnumerable{TEvent}"/> that contains all events in the collection.</returns>  
    public IEnumerable<TEvent> ConcatAll() => eventsBeatOrder.SelectMany(i => i.Value).Cast<TEvent>();
    ///// <summary>  
    ///// Adds an event to the collection.  
    ///// </summary>  
    ///// <param name="item">The event to add.</param>  
    //public virtual bool Add(TEvent item) => Add((TBeat)(object)item);
    void ICollection<TEvent>.Add(TEvent item) => Add(item);
    ///// <inheritdoc/>  
    //public virtual bool Contains(TEvent item) => Contains((IBaseEvent)(object)item);
    ///// <inheritdoc/>  
    //public void CopyTo(TEvent[] array, int arrayIndex) => CopyTo((IBaseEvent[])(object)array, arrayIndex);
    ///// <inheritdoc/>  
    //public virtual bool Remove(TEvent item) => Remove((BaseEvent)(object)item);
    /// <inheritdoc/>  
    public override string ToString() => $"Count = {Count}";
    ///// <inheritdoc/>
    //public override IEnumerator<IBaseEvent> GetEnumerator() => (IEnumerator<IBaseEvent>)new EventEnumerator<TEvent>(this);
    /// <inheritdoc/>  
    public IEnumerator<TEvent> GetEnumerator()
    {
        foreach (KeyValuePair<TBeat, TypedEventCollection<TType, TBeat>> pair in eventsBeatOrder)
            foreach (TEvent item in pair.Value.Select(v => v))
                yield return item;
    }
    /// <summary>
    /// Gets the total count of events in the collection.
    /// </summary>
    public virtual int Count => eventsBeatOrder.Sum(i => i.Value.Count);
    /// <summary>
    /// Gets a value indicating whether the collection is read-only.
    /// </summary>
    public bool IsReadOnly { get; }
    /// <summary>
    /// Returns the beat of the last event.
    /// </summary>
    /// <returns>The beat of the last event.</returns>
    public TBeat Duration => eventsBeatOrder.LastOrDefault().Key;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderedEventCollection{TEvent, TType, TBeat}"/> class with the specified items.
    /// </summary>
    /// <param name="items">The items to add to the collection.</param>
    public OrderedEventCollection(IEnumerable<TEvent> items)
    {
        eventsBeatOrder = [];
        IsReadOnly = false;
        foreach (TEvent item in items)
            Add(item);
    }
    /// <summary>
    /// Adds an event to the collection.
    /// </summary>
    /// <param name="item">The event to add.</param>
    public virtual bool Add(TEvent item)
    {
        if (eventsBeatOrder.TryGetValue(item.Beat, out TypedEventCollection<TType, TBeat>? value))
            return value.Add(item);
        eventsBeatOrder.Insert(item.Beat, [item]);
        return true;
    }
    //void ICollection<TEvent>.Add(TEvent item) => Add(item);
    /// <summary>
    /// Clears all events from the collection.
    /// </summary>
    public void Clear() => eventsBeatOrder.Clear();
    /// <summary>
    /// Determines whether the collection contains a specific event.
    /// </summary>
    /// <param name="item">The event to locate in the collection.</param>
    /// <returns>true if the event is found in the collection; otherwise, false.</returns>
    public virtual bool Contains(TEvent item) => eventsBeatOrder.FindNode(item.Beat)?.Value.Contains(item) ?? false;
    /// <summary>
    /// Copies the elements of the collection to an array, starting at a particular array index.
    /// </summary>
    /// <param name="array">The array to copy the elements to.</param>
    /// <param name="arrayIndex">The zero-based index in the array at which copying begins.</param>
    public void CopyTo(TEvent[] array, int arrayIndex)
    {
        if (arrayIndex < 0 || arrayIndex > array.Length)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < Count)
            throw new ArgumentException("The number of elements in the source collection is greater than the available space from arrayIndex to the end of the destination array.");
        foreach (KeyValuePair<TBeat, TypedEventCollection<TType, TBeat>> pair in eventsBeatOrder)
        {
            foreach (TEvent item in pair.Value)
            {
                array[arrayIndex++] = item;
            }
        }
    }
    /// <summary>
    /// Removes the first occurrence of a specific event from the collection.
    /// </summary>
    /// <param name="item">The event to remove from the collection.</param>
    /// <returns>true if the event was successfully removed; otherwise, false.</returns>
    public virtual bool Remove(TEvent item)
    {
        bool Remove;
        if (Contains(item))
        {
            bool result = eventsBeatOrder[item.Beat].Remove(item);
            if (eventsBeatOrder[item.Beat].Count == 0)
                eventsBeatOrder.Remove(item.Beat);
            Remove = result;
        }
        else
            Remove = false;
        return Remove;
    }
    internal IEnumerator<TEvent> GetEnumerator(float? start, float? end) => new EventEnumerator<TEvent, TType, TBeat>(eventsBeatOrder, Types, CreateRange(
        start, end));
    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator for the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    /// <summary>
    /// Removes the first occurrence of a specific event from the collection.
    /// </summary>
    /// <param name="item">The event to remove from the collection.</param>
    /// <returns>true if the event was successfully removed; otherwise, false.</returns>
    bool ICollection<TEvent>.Remove(TEvent item) => throw new NotImplementedException();
    /// <summary>
    /// The dictionary that maintains the order of events based on their beats.
    /// </summary>
    internal RedBlackTree<TBeat, TypedEventCollection<TType, TBeat>> eventsBeatOrder = [];
    RedBlackTree<TBeat, TypedEventCollection<TType, TBeat>> IEventEnumerable<TEvent, TType, TBeat>.EventsBeatOrder => eventsBeatOrder;
    internal abstract TBeat CreateInstance(float beat);
    internal abstract IBeatRange<TBeat> CreateRange(float? start, float? end);
    internal abstract ReadOnlyEnumCollection<TType> Types { get; }
    ReadOnlyEnumCollection<TType> IEventEnumerable<TEvent, TType, TBeat>.Types => Types;
    IBeatRange<TBeat> IEventEnumerable<TEvent, TType, TBeat>.Range => CreateRange(null, null);

    internal abstract ReadOnlyEnumCollection<TType> TypesOf<TTarget>() where TTarget : IEvent<TType, TBeat>;
}
