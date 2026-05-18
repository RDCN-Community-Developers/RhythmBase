using RhythmBase.BeatBlock.Components;
using RhythmBase.Global.Components.Easing;
namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Mine
/// </summary>
/// <remarks>
/// Similar to a basic note, but must NOT be hit.
/// </remarks>
[RDJsonObjectSerializable]
public record class Mine : BaseEvent, IChartEvent, IPureEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.Mine;
    /// <summary>
    /// Angle to end up at
    /// </summary>
    public float? EndAngle { get; set; }
    /// <summary>
    /// Ease to use while rotating
    /// </summary>
    public EaseType? SpinEase { get; set; }
    /// <summary>
    /// Speed multiplier for approach
    /// </summary>
    public float? SpeedMult { get; set; }
    /// <summary>
    /// Color channel 0 (default white)
    /// </summary>
    public ColorIndex? Color0 { get; set; }
    /// <summary>
    /// Color channel 1 (default black)
    /// </summary>
    public ColorIndex? Color1 { get; set; }
    /// <summary>
    /// Ease sequence to use, if any
    /// </summary>
    public string? EaseSequence { get; set; } = string.Empty;
}
