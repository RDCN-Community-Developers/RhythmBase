namespace RhythmBase.BeatBlock.Components
{
    public record class Properties
    {
        public int Offset { get; set; }
        public int Speed { get; set; }
        public int StartingBeat { get; set; }
        public int FormatVersion { get; set; } = DefaultVersionBeatBlock;
    }
}
