namespace RhythmBase.BeatBlock.Events
{
    public interface IBaseEvent
    {
        public Enums Type { get; }
        public float Time { get; set; }
        public float Angle { get; set; }
    }
}
