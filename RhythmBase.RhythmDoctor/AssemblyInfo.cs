using RhythmBase.Global.Serialization;
using RhythmBase.RhythmDoctor.Events;
using System.Runtime.CompilerServices;

[assembly: RhythmBase.JsonConverterId(nameof(RhythmBase.RhythmDoctor))]
#if RHYTHMBASE_NO_GENERIC_ATTRIBUTES
[assembly: RhythmBase.JsonConverterSourceType(typeof(IBaseEvent), typeof(RhythmBase.RhythmDoctor.EventType), typeof(RhythmBase.RhythmDoctor.Serialization.MemberConverter<>), nameof(IBaseEvent.Type))]
[assembly: RhythmBase.JsonConverterLink(typeof(Color), typeof(ColorConverter.RgbaHex))]
[assembly: RhythmBase.AdapterType(
	typeof(RhythmBase.RhythmDoctor.Components.Chart),
	typeof(RhythmBase.RhythmDoctor.Components.Level),
	typeof(RhythmBase.RhythmDoctor.Utils.BeatCalculator),
	typeof(RhythmBase.RhythmDoctor.Components.TickTime),
	typeof(RhythmBase.RhythmDoctor.EventType),
	typeof(RhythmBase.RhythmDoctor.Events.IBaseEvent)
)]
#else
[assembly: RhythmBase.JsonConverterSourceType<IBaseEvent, RhythmBase.RhythmDoctor.EventType>(typeof(RhythmBase.RhythmDoctor.Serialization.MemberConverter<>), nameof(IBaseEvent.Type))]
[assembly: RhythmBase.JsonConverterLink<Color, ColorConverter.RgbaHex>]
[assembly: RhythmBase.AdapterType<
	RhythmBase.RhythmDoctor.Components.Chart,
	RhythmBase.RhythmDoctor.Components.Level,
	RhythmBase.RhythmDoctor.Utils.BeatCalculator,
	RhythmBase.RhythmDoctor.Components.TickTime,
	RhythmBase.RhythmDoctor.EventType,
	RhythmBase.RhythmDoctor.Events.IBaseEvent
>]
#endif
[assembly: InternalsVisibleTo("FastTest")]