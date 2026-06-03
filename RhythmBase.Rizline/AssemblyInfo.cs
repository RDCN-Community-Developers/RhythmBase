using RhythmBase.Rizline.Events;
using RhythmBase.Rizline;
[assembly: RhythmBase.JsonConverterId(nameof(RhythmBase.Rizline))]
[assembly: RhythmBase.JsonConverterSourceType(typeof(IBaseEvent), typeof(EventType), typeof(RhythmBase.Rizline.Converters.MemberConverter<>), nameof(IBaseEvent.Type))]
[assembly: RhythmBase.JsonConverterLink(typeof(Color), typeof(ColorConverter.ArgbObject))]