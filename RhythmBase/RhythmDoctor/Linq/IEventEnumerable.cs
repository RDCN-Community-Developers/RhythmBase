using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace RhythmBase.RhythmDoctor.Linq
{
    public interface IEventEnumerable<out TEvent> : RhythmBase.Global.Linq.IEventEnumerable<TEvent, EventType, RDBeat>
        where TEvent : IBaseEvent
    {
    }
}
