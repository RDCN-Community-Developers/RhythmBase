namespace RhythmBase.BeatBlock.Events
{
    public record class Tag
    {
        [RDJsonAlias("Tag")]
        public string TagName { get; set; }
        public bool AngleOffset { get; set; }
    }
}
