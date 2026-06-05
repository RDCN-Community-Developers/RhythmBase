using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Converters;
using RhythmBase.RhythmDoctor.Events;
using RhythmBase.RhythmDoctor.Linq;

namespace RhythmBase.RhythmDoctor.Extensions;

internal class EventEnumerator<TEvent> : EventEnumerator<TEvent, EventType, TickTime>, IEventEnumerable<TEvent>
    where TEvent : IBaseEvent
{
    public EventEnumerator(RedBlackTree<TickTime, TypedEventCollection<EventType, TickTime>> source, ReadOnlyEnumCollection<EventType> types, Components.Range range)
        : base(source, types, range)
    {
    }
}

partial class Extensions
{
    //private static IEventEnumerable<TEvent> OfEventTyped<TEvent>(this Global.Linq.IEventEnumerable<IBaseEvent, EventType, RDBeat> source)
    //    where TEvent : IBaseEvent =>
    //    source as IEventEnumerable<TEvent>
    //    ?? new EventEnumerator<TEvent>(source);
    //private static IEventEnumerable<TEvent> OfEventTyped<TEvent>(this Global.Linq.IEventEnumerable<TEvent, EventType, RDBeat> source)
    //    where TEvent : IBaseEvent =>
    //    source as IEventEnumerable<TEvent>
    //    ?? throw new NotSupportedException("The provided IEventEnumerable is not supported.");
    extension<TEvent>(IEventEnumerable<TEvent> source) where TEvent : IBaseEvent
    {
        /// <summary>
        /// Filters the event enumerable to only include events of type <typeparamref name="TEvent"/> within the specified beat range.
        /// </summary>
        /// <param name="start">The start beat of the range (inclusive).</param>
        /// <param name="end">The end beat of the range (inclusive).</param>
        /// <returns>An <see cref="IEventEnumerable{TEvent}"/> containing only events of type <typeparamref name="TEvent"/> within the specified range.</returns>
        /// <exception cref="NotSupportedException">Thrown if the provided <see cref="IEventEnumerable{TEvent}"/> is not supported.</exception>
        public IEventEnumerable<TEvent> InRange(TickTime? start, TickTime? end)
        {
            Components.Range merged = new Components.Range(start, end).Intersect(source.Range as Components.Range? ?? new Components.Range(null, null));
            return new EventEnumerator<TEvent>(source.EventsBeatOrder, source.Types, merged);
        }

        /// <summary>
        /// Filters the event enumerable to only include events of type <typeparamref name="TEvent"/> within the specified <see cref="Components.Range"/>.
        /// </summary>
        /// <param name="range">The beat range to filter for.</param>
        /// <returns>An <see cref="IEventEnumerable{TEvent}"/> containing only events of type <typeparamref name="TEvent"/> within the specified range.</returns>
        /// <exception cref="NotSupportedException">Thrown if the provided <see cref="IEventEnumerable{TEvent}"/> is not supported.</exception>
        public IEventEnumerable<TEvent> InRange(Components.Range range)
        {
            Components.Range merged = range.Intersect(source.Range as Components.Range? ?? new Components.Range(null, null));
            return new EventEnumerator<TEvent>(source.EventsBeatOrder, source.Types, merged);
        }
        /// <summary>
        /// Filters the event enumerable to only include events of type <typeparamref name="TEvent"/> at the specified beat.
        /// </summary>
        /// <param name="beat">The beat to filter for.</param>
        /// <returns>An <see cref="IEnumerable{TEvent}"/> containing only events of type <typeparamref name="TEvent"/> at the specified beat.</returns>
        /// <exception cref="NotImplementedException">Always thrown as this method is not implemented.</exception>
        public IEnumerable<TEvent> AtBeat(TickTime beat)
        {
            if (source is EventEnumerator<TEvent, EventType, TickTime> casted)
                return casted.AtBeat(beat);
            if (source is OrderedEventCollection<TEvent, EventType, TickTime> ordered)
            {
                var collection = ordered.EventsBeatOrder.TryGetValue(beat, out TypedEventCollection<EventType, TickTime>? b) ? b : [];
                var types = EventTypeRegistry.ToEnums(typeof(TEvent));
                return collection.ContainsTypes(types) ?
                    collection.Where(i => types.Contains(((IBaseEvent)i).Type)).OfType<TEvent>() :
                    [];
            }
            throw new NotSupportedException("The provided IEventEnumerable is not supported.");
        }
    }
    extension(IEventEnumerable<IBaseEvent> source)
    {
        /// <summary>
        /// Filters the event enumerable to only include events of the specified type <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent">The type of event to filter for.</typeparam>
        /// <returns>An <see cref="IEventEnumerable{TEvent}"/> containing only events of type <typeparamref name="TEvent"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown if the provided <see cref="IEventEnumerable{TEvent}"/> is not supported.</exception>
        public IEventEnumerable<TEvent> OfEvent<TEvent>() where TEvent : IBaseEvent
        {
            ReadOnlyEnumCollection<EventType> merged = EventTypeRegistry.ToEnums(typeof(TEvent)).Intersect(source.Types);
            return new EventEnumerator<TEvent>(source.EventsBeatOrder, merged, source.Range as Components.Range? ?? new Components.Range(null, null));
        }

        /// <summary>
        /// Filters the event enumerable to only include events of the specified event types.
        /// </summary>
        /// <param name="types">The event types to filter for.</param>
        /// <returns>An <see cref="IEventEnumerable{IBaseEvent}"/> containing only events of the specified types.</returns>
        /// <exception cref="NotSupportedException">Thrown if the provided <see cref="IEventEnumerable{IBaseEvent}"/> is not supported.</exception>
        public IEventEnumerable<IBaseEvent> OfEvents(params EventType[] types)
        {
            ReadOnlyEnumCollection<EventType> merged = source.Types.Intersect([..types]);
            return new EventEnumerator<IBaseEvent>(source.EventsBeatOrder, merged, source.Range as Components.Range? ?? new Components.Range(null, null));
        }


        /// <summary>
        /// Filters the event enumerable to only include events within the specified beat range.
        /// </summary>
        /// <param name="start">The start beat of the range (inclusive).</param>
        /// <param name="end">The end beat of the range (inclusive).</param>
        /// <returns>An <see cref="IEventEnumerable{IBaseEvent}"/> containing only events within the specified range.</returns>
        /// <exception cref="NotSupportedException">Thrown if the provided <see cref="IEventEnumerable{IBaseEvent}"/> is not supported.</exception>
        public IEventEnumerable<IBaseEvent> InRange(TickTime? start, TickTime? end)
        {
            Components.Range merged = new Components.Range(start, end).Intersect(source.Range as Components.Range? ?? new Components.Range(null, null));
            return new EventEnumerator<IBaseEvent>(source.EventsBeatOrder, source.Types, merged);
        }

        /// <summary>
        /// Filters the event enumerable to only include events within the specified <see cref="Components.Range"/>.
        /// </summary>
        /// <param name="range">The beat range to filter for.</param>
        /// <returns>An <see cref="IEventEnumerable{IBaseEvent}"/> containing only events within the specified range.</returns>
        /// <exception cref="NotSupportedException">Thrown if the provided <see cref="IEventEnumerable{IBaseEvent}"/> is not supported.</exception>
        public IEventEnumerable<IBaseEvent> InRange(Components.Range range)
        {
            Components.Range merged = range.Intersect(source.Range as Components.Range? ?? new Components.Range(null, null));
            return new EventEnumerator<IBaseEvent>(source.EventsBeatOrder, source.Types, merged);
        }

    }
}
