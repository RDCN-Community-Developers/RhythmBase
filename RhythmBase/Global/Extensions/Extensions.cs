using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.Global.Extensions;

public static class Extensions
{
    extension<TBeat>(IBeatRange<TBeat>)
        where TBeat : struct, IBeat<TBeat>
    {
        public static IBeatRange<TBeat> CreateRange(TBeat? start, TBeat? end)
        {
            switch (start, end)
            {
                case (RDBeat s1, RDBeat e1):
                    return (IBeatRange<TBeat>)(object)new RDRange(s1, e1);
                case (RDBeat s2, null):
                    return (IBeatRange<TBeat>)(object)new RDRange(s2, null);
                case (null, RDBeat e2):
                    return (IBeatRange<TBeat>)(object)new RDRange(null, e2);

                case (null, null):
                    return IBeatRange<TBeat>.Infinity;
                default:
                    throw new NotSupportedException();
            }
        }
        public static IBeatRange<TBeat> Infinity => CreateRange<TBeat>(null, null);
#if NETSTANDARD
        public static TBeat FromBeatOnly(float beatOnly) => throw new NotSupportedException("This method is only supported in .NET 8.0 or later.");
        public static TBeat FromTimeSpan(TimeSpan timeSpan) => throw new NotSupportedException("This method is only supported in .NET 8.0 or later.");
#endif
    }
    public static ReadOnlyEnumCollection<TType> TypesOf<TEvent, TTarget, TType, TBeat>()
        where TEvent : IEvent<TType, TBeat>
        where TTarget : IEvent<TType, TBeat>
        where TType : struct, Enum
        where TBeat : struct, IBeat<TBeat>
    {
        if (typeof(TType) == typeof(RhythmDoctor.EventType))
            return RhythmDoctor.Utils.EventTypeUtils.ToEnums(typeof(TType)) as ReadOnlyEnumCollection<TType>? ?? [];
        else if (typeof(TType) == typeof(Adofai.EventType))
            return Adofai.Utils.EventTypeUtils.ToEnums(typeof(TType)) as ReadOnlyEnumCollection<TType>? ?? [];
        else if (typeof(TType) == typeof(BeatBlock.EventType))
            //return BeatBlock.Utils.EventTypeUtils.ToEnums(typeof(TType)) as ReadOnlyEnumCollection<TType>? ?? [];
            throw new NotImplementedException();
        else
            throw new NotSupportedException($"Unsupported event type enum: {typeof(TType)}");
    }
}
