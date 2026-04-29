namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Force MIDI Note
/// </summary>
/// <remarks>
/// Plays a single MIDI note
/// </remarks>
public record class ForceMidiNote : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.ForceMidiNote;
    /// <summary>
    /// Use the object at vfx.objects[variableName].
    /// </summary>
    public string VariableName { get; set; } = string.Empty;
    /// <summary>
    /// Note pitch to play
    /// </summary>
    public int Note { get; set; } 
    /// <summary>
    /// Note velocity
    /// </summary>
    public int Velocity { get; set; } 
    /// <summary>
    /// Note duration
    /// </summary>
    public float Duration { get; set; } 
    /// <summary>
    /// Note channel
    /// </summary>
    public int Channel { get; set; } 
}
