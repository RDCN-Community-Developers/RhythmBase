using RhythmBase.Rizline.Events;
[assembly: RhythmBase.JsonConverterId(nameof(RhythmBase.Rizline))]
[assembly: RhythmBase.JsonConverterSourceType(typeof(IBaseEvent), typeof(RhythmBase.Rizline.Components.EventType), typeof(RhythmBase.Rizline.Converters.MemberConverter<>), nameof(IBaseEvent.Type))]