namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Play MIDI
/// </summary>
/// <remarks>
/// Plays a MIDI file with a compatible object
/// </remarks>
public record class PlayMidi : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.PlayMidi;
    /// <summary>
    /// filename of the converted MIDI json file
    /// </summary>
    public string Filename { get; set; } = string.Empty;
    /// <summary>
    /// Use the object at vfx.objects[variableName].
    /// </summary>
    public string VariableName { get; set; } = string.Empty;
    /// <summary>
    /// Unique ID for playback tracking. If unsetm uses variableName.
    /// </summary>
    public string? PlaybackID { get; set; } = string.Empty;
}
