namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Set Bounce Height
/// </summary>
/// <remarks>
/// Set how high bounces bounce based on delay
/// </remarks>
public record class SetBounceHeight : BaseEvent, IChartEvent, IPureEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.SetBounceHeight;
}
