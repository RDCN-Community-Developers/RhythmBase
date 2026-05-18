namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Spawn Block (LEGACY)
/// </summary>
/// <remarks>
/// No description
/// </remarks>
[RDJsonObjectSerializable]
public record class Beat : BaseEvent, IChartEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.Beat;
}
