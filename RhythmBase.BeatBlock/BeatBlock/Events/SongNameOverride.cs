namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Song Name Override
/// </summary>
/// <remarks>
/// Override the song name shown in the corner of the screen. leave empty to use the default song name.
/// </remarks>
[JsonObjectSerializable]
public record class SongNameOverride : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.SongNameOverride;
    /// <summary>
    /// new name
    /// </summary>
    public string? Newname { get; set; } = string.Empty;
    /// <summary>
    /// glitch in
    /// </summary>
    public bool Glitch { get; set; }
    /// <summary>
    /// glitch time
    /// </summary>
    public float Duration { get; set; }
    /// <summary>
    /// glitch end percent
    /// </summary>
    public float? Glitchend { get; set; }
}
