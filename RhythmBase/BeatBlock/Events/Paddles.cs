using RhythmBase.Global.Components.Easing;

namespace RhythmBase.BeatBlock.Events
{
    public record class Paddles : BaseEvent, IEaseEvent
    {
        public override Enums Type { get; }
        public EaseType Ease { get; set; }
        public float Duration { get; set; }
        public int Paddle { get; set; }
        public bool Enabled { get; set; }
        public int NewWidth { get; set; }
        //public int NewHeight { get; set; }
        public float NewAngle { get; set; }
    }
}
