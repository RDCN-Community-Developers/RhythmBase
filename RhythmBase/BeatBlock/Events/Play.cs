namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Play song
/// </summary>
/// <remarks>
/// Plays a song
/// </remarks>
[RDJsonObjectSerializable]
public record class Play : BaseEvent, IPureEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.Play;
    /// <summary>
    /// Filename of song
    /// </summary>
    public string File { get; set; } = string.Empty;
    /// <summary>
    /// BPM of song
    /// </summary>
    public float Bpm { get; set; } 
    /// <summary>
    /// Volume of song, 1 = 100% volume
    /// </summary>
    public float Volume { get; set; } 
    /// <summary>
    /// Offset of the song, in seconds.
    /// </summary>
    public float? Offset { get; set; } 
}
