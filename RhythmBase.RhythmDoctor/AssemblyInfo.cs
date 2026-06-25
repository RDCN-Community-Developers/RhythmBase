using RhythmBase.Global.Components.RichText;
using RhythmBase.Global.Converters;
using RhythmBase.RhythmDoctor.Events;

[assembly: RhythmBase.JsonConverterId(nameof(RhythmBase.RhythmDoctor))]
[assembly: RhythmBase.JsonConverterSourceType(typeof(IBaseEvent), typeof(RhythmBase.RhythmDoctor.EventType), typeof(RhythmBase.RhythmDoctor.Converters.MemberConverter<>), nameof(IBaseEvent.Type))]
[assembly: RhythmBase.JsonConverterLink(typeof(Color), typeof(ColorConverter.RgbaHex))]
[assembly: RhythmBase.JsonConverterLink(typeof(RichLine<RichStringStyle>), typeof(RichTextConverter<RichStringStyle>))]
[assembly: RhythmBase.TickTime(
	typeof(RhythmBase.RhythmDoctor.Components.Level),
	typeof(RhythmBase.RhythmDoctor.Utils.BeatCalculator),
	typeof(RhythmBase.RhythmDoctor.Components.TickTime),
	typeof(RhythmBase.RhythmDoctor.EventType),
	typeof(RhythmBase.RhythmDoctor.Events.IBaseEvent)
)]