namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Set BG
/// </summary>
/// <remarks>
/// No description
/// </remarks>
public record class SetBackground : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.SetBackground;
}
