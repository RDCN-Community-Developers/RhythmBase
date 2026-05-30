using RhythmBase.BeatBlock;
using RhythmBase.BeatBlock.Events;

[assembly: RhythmBase.JsonConverterId(nameof(RhythmBase.BeatBlock))]
[assembly: RhythmBase.JsonConverterSourceType(typeof(IBaseEvent), typeof(EventType), nameof(IBaseEvent.Type))]