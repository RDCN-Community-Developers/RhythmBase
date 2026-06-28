using RhythmBase.BeatBlock;
using RhythmBase.BeatBlock.Events;

[assembly: RhythmBase.JsonConverterId(nameof(RhythmBase.BeatBlock))]
[assembly: RhythmBase.JsonConverterSourceType(typeof(IBaseEvent), typeof(EventType), typeof(RhythmBase.BeatBlock.Converters.MemberConverter<>), nameof(IBaseEvent.Type))]
[assembly: RhythmBase.JsonConverterLink(typeof(Color), typeof(ColorConverter.RgbObject))]
[assembly: RhythmBase.AdapterType(
	typeof(RhythmBase.BeatBlock.Components.Chart),
	typeof(RhythmBase.BeatBlock.Components.BeatCalculator),
	typeof(RhythmBase.BeatBlock.Components.TickTime),
	typeof(RhythmBase.BeatBlock.EventType),
	typeof(RhythmBase.BeatBlock.Events.IBaseEvent)
	)]
[assembly: RhythmBase.JsonEnumCasting(typeof(RhythmBase.Global.Components.Easing.EaseType), false)]