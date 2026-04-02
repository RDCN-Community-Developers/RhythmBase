namespace RhythmBase.BeatBlock.Events
{
    public record class Deco
    {
        [RDJsonAlias("Ox")]
        public float Ox { get; set; }
        [RDJsonAlias("Oy")]
        public float Oy { get; set; }
        [RDJsonAlias("Sx")]
        public float Sx { get; set; }
        [RDJsonAlias("Sy")]
        public float Sy { get; set; }
        public string Id { get; set; }
        public int DrawOrder { get; set; }
        public FileReference Sprite { get; set; }
        public Layer DrawLayer { get; set; }

    }
}
