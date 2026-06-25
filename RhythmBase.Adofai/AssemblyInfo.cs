using RhythmBase.Adofai;
using RhythmBase.Adofai.Components.Filters;
using RhythmBase.Adofai.Converters;
using RhythmBase.Adofai.Events;

[assembly: RhythmBase.JsonConverterId(nameof(RhythmBase.Adofai))]
[assembly: RhythmBase.JsonConverterSourceType(typeof(IBaseEvent), typeof(EventType), typeof(RhythmBase.Adofai.Converters.MemberConverter<>), nameof(IBaseEvent.Type))]
[assembly: RhythmBase.JsonConverterSourceType(typeof(IFilter), typeof(AdvancedFilter), typeof(FilterMemberConverter<>), nameof(IFilter.Type))]
[assembly: RhythmBase.JsonConverterLink(typeof(Color), typeof(ColorConverter.RgbaHex))]
[assembly: RhythmBase.TickTime(
	typeof(RhythmBase.Adofai.Components.Level),
	typeof(RhythmBase.Adofai.Utils.BeatCalculator),
	typeof(RhythmBase.Adofai.Components.TickTime),
	typeof(RhythmBase.Adofai.EventType),
	typeof(RhythmBase.Adofai.Events.IBaseEvent)
	)]