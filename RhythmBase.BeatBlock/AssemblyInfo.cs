using RhythmBase.BeatBlock;
using RhythmBase.BeatBlock.Events;

[assembly: RhythmBase.JsonConverterId(nameof(RhythmBase.BeatBlock))]
[assembly: RhythmBase.JsonConverterSourceType(typeof(IBaseEvent), typeof(EventType), typeof(RhythmBase.BeatBlock.Converters.MemberConverter<>), nameof(IBaseEvent.Type))]
[assembly: RhythmBase.JsonConverterLink(typeof(Color), typeof(ColorConverter.RgbObject))]