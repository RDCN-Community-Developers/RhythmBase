namespace RhythmBase.Global.Linq;

///// <summary>
///// Represents a collection of events that can be enumerated.
///// </summary>
///// <typeparam name="TEvent">The type of events in the collection. Must implement <see cref="IEvent{TType, TBeat}"/>.</typeparam>
///// <typeparam name="TType">The type of the event type. Must be a struct and an enum.</typeparam>
///// <typeparam name="TBeat">The type of the beat. Must be a struct and implement <see cref="ITickTime{TBeat}"/>.</typeparam>
//public interface IEventEnumerable<out TEvent, TType, TBeat>
//    : IEnumerable<TEvent>
//    where TEvent : IEvent<TType, TBeat>
//    where TType : unmanaged, Enum
//    where TBeat : struct, ITickTime<TBeat>
//{
//    /// <summary>
//    /// Gets the events organized by beat order.
//    /// </summary>
//    RedBlackTree<TBeat, TypedEventCollection<TType, TBeat>> EventsBeatOrder { get; }
//    /// <summary>
//    /// Gets the collection of event types present in the enumerable.
//    /// </summary>
//    ReadOnlyEnumCollection<TType> Types { get; }
//    /// <summary>
//    /// Gets the beat range of the enumerable.
//    /// </summary>
//    ITickRange<TBeat> Range { get; }
//}