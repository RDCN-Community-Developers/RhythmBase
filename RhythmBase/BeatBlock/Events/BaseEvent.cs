namespace RhythmBase.BeatBlock.Events
{
    public abstract record class BaseEvent : IBaseEvent
    {
        public abstract Enums Type { get; }
        public float Time { get; set; }
        public float Angle { get; set; }
    }
}
