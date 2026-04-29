namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Multipulse
/// </summary>
/// <remarks>
/// No description
/// </remarks>
public record class MultiPulse : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.MultiPulse;
}
