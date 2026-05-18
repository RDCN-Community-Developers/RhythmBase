using RhythmBase.Global.Extensions;
using System.Collections;
using RhythmBase.Global.Linq;

namespace RhythmBase.Global.Components;

internal class EventEnumerator<TEvent, TType, TBeat>(RedBlackTree<TBeat, TypedEventCollection<TType, TBeat>> collection, ReadOnlyEnumCollection<TType> types, IBeatRange<TBeat> range)
    : IEventEnumerable<TEvent, TType, TBeat>, IEnumerator<TEvent>
    where TEvent : IEvent<TType, TBeat>
    where TType : struct, Enum
    where TBeat : struct, IBeat<TBeat>
{
    protected readonly IEnumerator<KeyValuePair<TBeat, TypedEventCollection<TType, TBeat>>> beats = collection.GetEnumerator();
    protected IEnumerator<IEvent>? events;
    protected readonly RedBlackTree<TBeat, TypedEventCollection<TType, TBeat>> collection = collection;
    private ReadOnlyEnumCollection<TType> types = types;
    private IBeatRange<TBeat> range = range;
    public TEvent Current => ((TEvent?)events?.Current) ?? throw new InvalidOperationException();
    object IEnumerator.Current => Current;
    RedBlackTree<TBeat, TypedEventCollection<TType, TBeat>> IEventEnumerable<TEvent, TType, TBeat>.EventsBeatOrder => collection;
    ReadOnlyEnumCollection<TType> IEventEnumerable<TEvent, TType, TBeat>.Types => types;
    IBeatRange<TBeat> IEventEnumerable<TEvent, TType, TBeat>.Range => range;
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
    public void Dispose()
    {
    }
    public void Reset() => throw new NotSupportedException();
    public IEventEnumerable<TTarget, TType, TBeat> OfEvent<TTarget>() where TTarget : TEvent
    {
        ReadOnlyEnumCollection<TType> types = this.types.Intersect(Extensions.Extensions.TypesOf<TEvent, TTarget, TType, TBeat>());
        this.types = types;
        return new EventEnumerator<TTarget, TType, TBeat>(collection, types, range);
    }
    public IEventEnumerable<TEvent, TType, TBeat> OfEvents(ReadOnlyEnumCollection<TType> types)
    {
        this.types = types;
        return this;
    }
    public EventEnumerator<TEvent, TType, TBeat> InRange(IBeatRange<TBeat> range)
    {
        this.range = this.range.Intersect(range);
        return this;
    }
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
    public IEnumerator<TEvent> GetEnumerator() => this;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
