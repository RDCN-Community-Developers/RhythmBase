namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Pulse
/// </summary>
/// <remarks>
/// No description
/// </remarks>
[RDJsonObjectSerializable]
public record class SinglePulse : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.SinglePulse;
}
