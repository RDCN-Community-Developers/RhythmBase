using RhythmBase.RhythmDoctor.Events;

namespace RhythmBase.Global.Linq;

/// <summary>
/// Represents a collection of events that can be enumerated.
/// </summary>
/// <typeparam name="T">The type of events in the collection. Must implement <see cref="IEvent{TType, TBeat}"/>.</typeparam>
/// <typeparam name="TType">The type of the event type. Must be a struct and an enum.</typeparam>
/// <typeparam name="TBeat">The type of the beat. Must be a struct and implement <see cref="IBeat{TBeat}"/>.</typeparam>
public interface IEventEnumerable<out TEvent, TType, TBeat>
    : IEnumerable<TEvent>
    where TEvent : IEvent<TType, TBeat>
    where TType : struct, Enum
    where TBeat : struct, IBeat<TBeat>
{
    RedBlackTree<TBeat, TypedEventCollection<TType, TBeat>> EventsBeatOrder { get; }
    ReadOnlyEnumCollection<TType> Types { get; }
    IBeatRange<TBeat> Range { get; }
}