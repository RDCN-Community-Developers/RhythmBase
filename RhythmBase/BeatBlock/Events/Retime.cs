namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Retime
/// </summary>
/// <remarks>
/// Skip a certain amount of time during playback
/// </remarks>
public record class Retime : BaseEvent, IPureEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.Retime;
    /// <summary>
    /// Beats to skip
    /// </summary>
    public float Offset { get; set; } 
}
