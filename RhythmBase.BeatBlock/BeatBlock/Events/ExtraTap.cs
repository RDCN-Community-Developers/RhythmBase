using RhythmBase.BeatBlock.Components;

namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Extra Tap
/// </summary>
/// <remarks>
/// A tap independent of other notes
/// </remarks>
[JsonObjectSerializable]
public record class ExtraTap : BaseEvent, IChartEvent, IPureEvent, IEaseSequenceEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.ExtraTap;
    /// <summary>
    /// Speed multiplier for approach
    /// </summary>
    public float? SpeedMult { get; set; } 
    /// <summary>
    /// Color channel 1 (default black)
    /// </summary>
    public ColorIndex? Color1 { get; set; } 
    /// <summary>
    /// Ease sequence to use, if any
    /// </summary>
    public string? EaseSequence { get; set; } = string.Empty;
}
