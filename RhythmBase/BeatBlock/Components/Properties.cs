namespace RhythmBase.BeatBlock.Components
{
    /// <summary>
    /// Represents the properties of a BeatBlock level.
    /// </summary>
    public record class Properties
    {
        /// <summary>
        /// Gets or sets the offset of the level.
        /// </summary>
        public int Offset { get; set; }
        /// <summary>
        /// Gets or sets the speed of the level.
        /// </summary>
        public int Speed { get; set; }
        /// <summary>
        /// Gets or sets the starting beat of the level.
        /// </summary>
        public int StartingBeat { get; set; }
        public int LoadBeat { get; set; }
        /// <summary>
        /// Gets or sets the format version of the level.
        /// </summary>
        public int FormatVersion { get; set; } = DefaultVersionBeatBlock;
    }
}
