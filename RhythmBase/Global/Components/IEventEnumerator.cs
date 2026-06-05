using System.Collections;
using RhythmBase.Global.Linq;

namespace RhythmBase.Global.Components;

/// <summary>
/// Provides an enumerator over events in a <see cref="RedBlackTree{TKey, TValue}"/> collection, filtered by
/// event types and an optional tick range.
/// </summary>
/// <typeparam name="TEvent">The event interface type to enumerate.</typeparam>
/// <typeparam name="TType">The enum type representing event categories.</typeparam>
/// <typeparam name="TBeat">The tick/time type used as the tree key.</typeparam>
public class EventEnumerator<TEvent, TType, TBeat>(RedBlackTree<TBeat, TypedEventCollection<TType, TBeat>> collection, ReadOnlyEnumCollection<TType> types, ITickRange<TBeat> range)
		: IEventEnumerable<TEvent, TType, TBeat>, IEnumerator<TEvent>
		where TEvent : IEvent<TType, TBeat>
		where TType : unmanaged, Enum
		where TBeat : struct, ITickTime<TBeat>
{
	/// <summary>
	/// The enumerator over beat-keyed buckets in the tree.
	/// </summary>
	protected readonly IEnumerator<KeyValuePair<TBeat, TypedEventCollection<TType, TBeat>>> beats = collection.GetEnumerator();
	/// <summary>
	/// The enumerator over events within the current bucket, or <c>null</c> if no bucket is active.
	/// </summary>
	protected IEnumerator<IEvent>? events;
	/// <summary>
	/// The underlying tree collection.
	/// </summary>
	protected readonly RedBlackTree<TBeat, TypedEventCollection<TType, TBeat>> collection = collection;
	private ReadOnlyEnumCollection<TType> types = types;
	private ITickRange<TBeat> range = range;
	/// <inheritdoc/>
	public TEvent Current => ((TEvent?)events?.Current) ?? throw new InvalidOperationException();
	/// <inheritdoc/>
	object IEnumerator.Current => Current;
	RedBlackTree<TBeat, TypedEventCollection<TType, TBeat>> IEventEnumerable<TEvent, TType, TBeat>.EventsBeatOrder => collection;
	ReadOnlyEnumCollection<TType> IEventEnumerable<TEvent, TType, TBeat>.Types => types;
	ITickRange<TBeat> IEventEnumerable<TEvent, TType, TBeat>.Range => range;
	/// <inheritdoc/>
	public bool MoveNext()
	{
		if (events != null)
		{
			while (events.MoveNext())
			{
				if (types.Contains(TypedEventCollection<TType, TBeat>.EventTypeOf(events.Current)))
					return true;
			}
			events = null;
		}
		while (beats.MoveNext())
		{
			var currentKey = beats.Current.Key;

			if (range.Start.HasValue && currentKey.CompareTo(range.Start.Value) < 0)
				continue;
			if (range.End.HasValue && currentKey.CompareTo(range.End.Value) >= 0)
				return false;
			if (!beats.Current.Value.ContainsTypes(types))
				continue;
			events = beats.Current.Value.GetEnumerator();
			while (events.MoveNext())
			{
				if (types.Contains(TypedEventCollection<TType, TBeat>.EventTypeOf(events.Current)))
					return true;
			}
			events = null;
		}
		return false;
	}
	private bool _disposed = false;
	/// <summary>
	/// Disposes the enumerator, releasing any resources held by the beat and event enumerators.
	/// </summary>
	/// <param name="disposing"></param>
	protected virtual void Dispose(bool disposing)
	{
		if (_disposed) return;
		if (disposing)
		{
			beats.Dispose();
			events?.Dispose();
		}
		_disposed = true;
	}
	/// <inheritdoc/>
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
	/// <inheritdoc/>
	public void Reset() => throw new NotSupportedException();
	/// <summary>
	/// Narrows the enumeration to events of the specified target type, intersecting the current type filter.
	/// </summary>
	/// <typeparam name="TTarget">The more specific event type to filter for.</typeparam>
	/// <returns>A new <see cref="IEventEnumerable{TEvent, TType, TBeat}"/> with the narrowed filter.</returns>
	public IEventEnumerable<TTarget, TType, TBeat> OfEvent<TTarget>() where TTarget : TEvent
	{
		ReadOnlyEnumCollection<TType> types = this.types.Intersect(Extensions.Extensions.TypesOf<TEvent, TTarget, TType, TBeat>());
		this.types = types;
		return new EventEnumerator<TTarget, TType, TBeat>(collection, types, range);
	}
	/// <summary>
	/// Replaces the current type filter with the specified collection of event types.
	/// </summary>
	/// <param name="types">The event types to enumerate.</param>
	/// <returns>This enumerator with the updated filter.</returns>
	public IEventEnumerable<TEvent, TType, TBeat> OfEvents(ReadOnlyEnumCollection<TType> types)
	{
		this.types = types;
		return this;
	}
	/// <summary>
	/// Narrows the enumeration to the specified tick range, intersecting with the current range.
	/// </summary>
	/// <param name="range">The tick range to filter for.</param>
	/// <returns>This enumerator with the narrowed range.</returns>
	public EventEnumerator<TEvent, TType, TBeat> InRange(ITickRange<TBeat> range)
	{
		this.range = this.range.Intersect(range);
		return this;
	}
	/// <summary>
	/// Returns all events at the specified beat that match the current type filter.
	/// </summary>
	/// <param name="beat">The beat at which to retrieve events.</param>
	/// <returns>An enumerable of matching events at the given beat.</returns>
	public IEnumerable<TEvent> AtBeat(TBeat beat)
	{
		if (!range.Contains(beat))
			yield break;
		if (!collection.TryGetValue(beat, out var events))
			yield break;
		if (!events.ContainsTypes(types))
			yield break;
		foreach (var ev in events)
			if (types.Contains(TypedEventCollection<TType, TBeat>.EventTypeOf(ev)))
				yield return (TEvent)ev;
	}
	/// <inheritdoc/>
	public IEnumerator<TEvent> GetEnumerator() => this;
	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
