using RhythmBase.Global.Components.RichText;
using RhythmBase.Global.Converters;
using RhythmBase.RhythmDoctor.Converters;
using RhythmBase.RhythmDoctor.Events;

[assembly: RhythmBase.JsonConverterId(nameof(RhythmBase.RhythmDoctor))]
[assembly: RhythmBase.JsonConverterSourceType(typeof(IBaseEvent), typeof(RhythmBase.RhythmDoctor.EventType), typeof(RhythmBase.RhythmDoctor.Converters.InstanceConverter<>), nameof(IBaseEvent.Type))]
[assembly: RhythmBase.JsonConverterLink(typeof(Color), typeof(ColorConverter.RgbaHex))]
[assembly: RhythmBase.JsonConverterLink(typeof(RichLine<RichStringStyle>), typeof(RichTextConverter<RichStringStyle>))]