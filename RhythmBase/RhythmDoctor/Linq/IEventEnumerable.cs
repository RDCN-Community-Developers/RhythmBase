using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace RhythmBase.RhythmDoctor.Linq
{
    /// <summary>
    /// Represents a collection of events that can be enumerated.
    /// </summary>
    /// <typeparam name="TEvent">The type of event.</typeparam>
    public interface IEventEnumerable<out TEvent> : RhythmBase.Global.Linq.IEventEnumerable<TEvent, EventType, RDBeat>
        where TEvent : IBaseEvent
    {
    }
}
