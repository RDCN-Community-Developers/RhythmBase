namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Play Sound
/// </summary>
/// <remarks>
/// Plays a sound effect
/// </remarks>
public record class PlaySound : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.PlaySound;
    /// <summary>
    /// Filename of the sound. Can also be a built in sound.
    /// </summary>
    public string Sound { get; set; } = string.Empty;
    /// <summary>
    /// Volume of the sound. 1 = Full Volume
    /// </summary>
    public float? Volume { get; set; }
    /// <summary>
    /// Pitch Shifting of sound. 1 = Normal Pitch
    /// </summary>
    public float? Pitch { get; set; }
}
