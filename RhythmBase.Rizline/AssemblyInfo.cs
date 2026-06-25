using RhythmBase.Rizline.Events;
using RhythmBase.Rizline;
[assembly: RhythmBase.JsonConverterId(nameof(RhythmBase.Rizline))]
[assembly: RhythmBase.JsonConverterSourceType(typeof(IBaseEvent), typeof(EventType), typeof(RhythmBase.Rizline.Converters.MemberConverter<>), nameof(IBaseEvent.Type))]
[assembly: RhythmBase.JsonConverterLink(typeof(Color), typeof(ColorConverter.ArgbObject))]
[assembly: RhythmBase.TickTime(
	typeof(RhythmBase.Rizline.Components.Chart),
	typeof(RhythmBase.Rizline.Components.BeatCalculator),
	typeof(RhythmBase.Rizline.Components.TickTime),
	typeof(RhythmBase.Rizline.EventType),
	typeof(RhythmBase.Rizline.Events.IBaseEvent)
	)]