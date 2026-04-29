namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Ease Sequence
/// </summary>
/// <remarks>
/// Define a new ease sequence
/// </remarks>
public record class EaseSequence : BaseEvent, IPureEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.EaseSequence;
    /// <summary>
    /// Name of the ease sequence
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
