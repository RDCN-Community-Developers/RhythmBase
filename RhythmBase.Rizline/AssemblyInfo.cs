using RhythmBase.Rizline.Events;
using RhythmBase.Rizline;
[assembly: RhythmBase.JsonConverterId(nameof(RhythmBase.Rizline))]
#if RHYTHMBASE_NO_GENERIC_ATTRIBUTES
[assembly: RhythmBase.JsonConverterSourceType(typeof(IBaseEvent), typeof(EventType), typeof(RhythmBase.Rizline.Serialization.MemberConverter<>), nameof(IBaseEvent.Type))]
[assembly: RhythmBase.JsonConverterLink(typeof(Color), typeof(ColorConverter.ArgbObject))]
[assembly: RhythmBase.AdapterType(
	typeof(RhythmBase.Rizline.Components.Chart),
	typeof(RhythmBase.Rizline.Components.Level),
	typeof(RhythmBase.Rizline.Components.BeatCalculator),
	typeof(RhythmBase.Rizline.Components.TickTime),
	typeof(RhythmBase.Rizline.EventType),
	typeof(RhythmBase.Rizline.Events.IBaseEvent)
	)]
#else
[assembly: RhythmBase.JsonConverterSourceType<IBaseEvent, EventType>(typeof(RhythmBase.Rizline.Serialization.MemberConverter<>), nameof(IBaseEvent.Type))]
[assembly: RhythmBase.JsonConverterLink<Color, ColorConverter.ArgbObject>]
[assembly: RhythmBase.AdapterType<
	RhythmBase.Rizline.Components.Chart,
	RhythmBase.Rizline.Components.Level,
	RhythmBase.Rizline.Components.BeatCalculator,
	RhythmBase.Rizline.Components.TickTime,
	RhythmBase.Rizline.EventType,
	RhythmBase.Rizline.Events.IBaseEvent
	>]
#endif