using RhythmBase.BeatBlock;
using RhythmBase.BeatBlock.Events;

[assembly: RhythmBase.JsonConverterId(nameof(RhythmBase.BeatBlock))]
#if RHYTHMBASE_NO_GENERIC_ATTRIBUTES
[assembly: RhythmBase.JsonConverterSourceType(typeof(IBaseEvent), typeof(EventType), typeof(RhythmBase.BeatBlock.Serialization.MemberConverter<>), nameof(IBaseEvent.Type))]
[assembly: RhythmBase.JsonConverterLink(typeof(Color), typeof(ColorConverter.RgbObject))]
[assembly: RhythmBase.AdapterType(
	typeof(RhythmBase.BeatBlock.Components.Chart),
	typeof(RhythmBase.BeatBlock.Components.Level),
	typeof(RhythmBase.BeatBlock.Components.BeatCalculator),
	typeof(RhythmBase.BeatBlock.Components.TickTime),
	typeof(RhythmBase.BeatBlock.EventType),
	typeof(RhythmBase.BeatBlock.Events.IBaseEvent)
	)]
#else
[assembly: RhythmBase.JsonConverterSourceType<IBaseEvent, EventType>(typeof(RhythmBase.BeatBlock.Serialization.MemberConverter<>), nameof(IBaseEvent.Type))]
[assembly: RhythmBase.JsonConverterLink<Color, ColorConverter.RgbObject>]
[assembly: RhythmBase.AdapterType<
	RhythmBase.BeatBlock.Components.Chart,
	RhythmBase.BeatBlock.Components.Level,
	RhythmBase.BeatBlock.Components.BeatCalculator,
	RhythmBase.BeatBlock.Components.TickTime,
	RhythmBase.BeatBlock.EventType,
	RhythmBase.BeatBlock.Events.IBaseEvent
	>]
#endif
[assembly: RhythmBase.JsonEnumCasting(typeof(RhythmBase.Global.Components.Easing.EaseType), false)]