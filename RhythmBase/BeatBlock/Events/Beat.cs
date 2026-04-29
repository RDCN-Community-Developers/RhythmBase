namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Spawn Block (LEGACY)
/// </summary>
/// <remarks>
/// No description
/// </remarks>
public record class Beat : BaseEvent, IChartEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.Beat;
}
