using RhythmBase.RhythmDoctor.Events;
using System.Collections;
namespace RhythmBase.Global.Components;

public class TypedEventCollection<TType, TBeat> : IEnumerable<IEvent>
    where TType : struct, Enum
    where TBeat : struct, IBeat<TBeat>
{
    public int Count => list.Count;
    public TypedEventCollection() { }
    public virtual bool Add(IEvent item)
    {
        if (list.Contains(item))
            return false;
        list.Add(item);
        _types.Add(EventTypeOf(item));
        return true;
    }
    public virtual bool Remove(IEvent item)
    {
        bool result = list.Remove(item);
        if (!result)
            return false;
        if (!list.Any(i => EventTypeOf(i).Equals(EventTypeOf(item))))
            _types.Remove(EventTypeOf(item));
        return true;
    }
    internal static TType EventTypeOf(IEvent item) => ((item as IEvent<TType, TBeat>) ?? throw new NotImplementedException()).Type;
    internal bool ContainsType(TType type) => _types.Contains(type);
    internal bool ContainsTypes(TType[] types) => _types.ContainsAny(types);
    internal bool ContainsTypes(ReadOnlyEnumCollection<TType> types) => _types.AsReadOnly().ContainsAny(types);
    internal bool CompareTo(IEvent item1, IEvent item2) =>
        list.IndexOf(item1) < list.IndexOf(item2);
    public override string ToString()
    {
        string result = $"Count={list.Count}";
        return result;
    }
    public IEnumerator<IEvent> GetEnumerator() =>
        list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() =>
        list.GetEnumerator();
    private readonly List<IEvent> list = [];
    private readonly EnumCollection<TType> _types = new(2);
}
