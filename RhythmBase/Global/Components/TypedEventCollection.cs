using System.Collections;
using System.ComponentModel;
namespace RhythmBase.Global.Components;

/// <summary>
/// Represents a collection of typed events.
/// </summary>
/// <typeparam name="TType">The type of the event type. Must be a struct and an enum.</typeparam>
/// <typeparam name="TBeat">The type of the beat. Must be a struct and implement <see cref="ITickTime{TBeat}"/>.</typeparam>
public class TypedEventCollection<TType, TBeat> : IEnumerable<IEvent>
    where TType : unmanaged, Enum
    where TBeat : struct, ITickTime<TBeat>
{
    /// <summary>
    /// Gets the number of events in the collection.
    /// </summary>
    public int Count => list.Count;
    /// <summary>
    /// Initializes a new instance of the <see cref="TypedEventCollection{TType, TBeat}"/> class.
    /// </summary>
    public TypedEventCollection() { }
    /// <summary>
    /// Adds an event to the collection.
    /// </summary>
    /// <param name="item">The event to add.</param>
    /// <returns>true if the event was added; otherwise, false if it already exists.</returns>
    public virtual bool Add(IEvent item)
    {
        if (list.Contains(item))
            return false;
        list.Add(item);
        _types.Add(EventTypeOf(item));
        return true;
    }
    /// <summary>
    /// Removes an event from the collection.
    /// </summary>
    /// <param name="item">The event to remove.</param>
    /// <returns>true if the event was removed; otherwise, false.</returns>
    public virtual bool Remove(IEvent item)
    {
        bool result = list.Remove(item);
        if (!result)
            return false;
        if (!list.Any(i => EventTypeOf(i).Equals(EventTypeOf(item))))
            _types.Remove(EventTypeOf(item));
        return true;
    }
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static TType EventTypeOf(IEvent item) => ((item as IEvent<TType, TBeat>) ?? throw new NotImplementedException()).Type;
    /// <summary>Determines whether this bucket contains any event of the specified type.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ContainsType(TType type) => _types.Contains(type);
    /// <summary>Determines whether this bucket contains any event of the specified types.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ContainsTypes(TType[] types) => _types.ContainsAny(types);
    /// <summary>Determines whether this bucket contains any event matching the given type collection.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ContainsTypes(ReadOnlyEnumCollection<TType> types) => _types.AsReadOnly().ContainsAny(types);
    /// <summary>Compares the insertion order of two events within this bucket.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool CompareTo(IEvent item1, IEvent item2) =>
        list.IndexOf(item1) < list.IndexOf(item2);
    /// <summary>
    /// Returns a string that represents the current collection.
    /// </summary>
    /// <returns>A string that represents the current collection.</returns>
    public override string ToString()
    {
        string result = $"Count={list.Count}";
        return result;
    }
    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<IEvent> GetEnumerator() =>
        list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() =>
        list.GetEnumerator();
    private readonly List<IEvent> list = [];
    private readonly EnumCollection<TType> _types = [];
}
