using RhythmBase.BeatBlock.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace RhythmBase.BeatBlock.Components
{
    public class TagEventCollection : OrderedEventCollection<IBaseEvent, EventType, BBBeat>
    {
        internal override ReadOnlyEnumCollection<EventType> Types => Utils.EventTypeUtils.ToEnums<IBaseEvent>();
        internal override BBBeat CreateInstance(float beat) => new BBBeat(beat);
        internal override IBeatRange<BBBeat> CreateRange(float? start, float? end) => new BBRange(start, end);
        internal override ReadOnlyEnumCollection<EventType> TypesOf<TTarget>() => Utils.EventTypeUtils.ToEnums(typeof(TTarget));
    }
}