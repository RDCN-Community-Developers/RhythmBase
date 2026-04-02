using RhythmBase.Global.Components.Easing;

namespace RhythmBase.BeatBlock.Events
{
    public record class Ease : BaseEvent, IEaseEvent
    {
        public override Enums Type => Enums.Ease;
        public float Value { get; set; }
        public float? Start { get; set; }
        public string Var { get; set; }
        public int? Order { get; set; }
        [RDJsonCondition($"$&.{nameof(Duration)} > 0")]
        public float Duration { get; set; }
        [RDJsonCondition($"$&.{nameof(Duration)} > 0")]
        [RDJsonAlias("Ease")]
        public EaseType Easing { get; set; }
        EaseType IEaseEvent.Ease { get; set; }
    }
}
