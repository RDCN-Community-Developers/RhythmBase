using RhythmBase.Adofai;
using RhythmBase.Adofai.Components.Filters;
using RhythmBase.Adofai.Events;

[assembly: RhythmBase.JsonConverterId(nameof(RhythmBase.Adofai))]
[assembly: RhythmBase.JsonConverterSourceType(typeof(IBaseEvent), typeof(EventType), typeof(), nameof(IBaseEvent.Type))]
[assembly: RhythmBase.JsonConverterSourceType(typeof(IFilter), typeof(FilterType), nameof(IFilter.Type))]