using RhythmBase.Global.Components.Easing;

namespace RhythmBase.BeatBlock.Events
{
    public record class SetColor : BaseEvent, IEaseEvent
    {
        public override Enums Type => Enums.SetColor;
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte Color { get; set; }
        [RDJsonCondition($"$&.{nameof(Duration)} > 0")]
        public float Duration { get; set; }
        [RDJsonCondition($"$&.{nameof(Duration)} > 0")]
        public EaseType Ease { get; set; }
    }
}
