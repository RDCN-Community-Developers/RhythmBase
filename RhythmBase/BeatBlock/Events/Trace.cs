using RhythmBase.BeatBlock.Components;
using RhythmBase.Global.Components.Easing;
namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Trace
/// </summary>
/// <remarks>
/// No description
/// </remarks>
[RDJsonObjectSerializable]
public record class Trace : BaseEvent, IChartEvent, IPureEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.Trace;
    /// <summary>
    /// Angle for end of the trace
    /// </summary>
    public float Angle2 { get; set; }
    /// <summary>
    /// How many beats the trace lasts
    /// </summary>
    public float Duration { get; set; }
    /// <summary>
    /// Force a certain number of line segments
    /// </summary>
    public int? Segments { get; set; }
    /// <summary>
    /// Change ease from angle1 to angle2
    /// </summary>
    public EaseType? TraceEase { get; set; }
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
    /// Dither the trace note?
    /// </summary>
    public bool DoDithering { get; set; }
    /// <summary>
    /// Color channel 1 (default black)
    /// </summary>
    public ColorIndex? Color1 { get; set; }
    /// <summary>
    /// Ease sequence to use for the hold head, if any
    /// </summary>
    public string? EaseSequence { get; set; } = string.Empty;
    /// <summary>
    /// Ease sequence to use for the hold tail, if any
    /// </summary>
    public string? TailEaseSequence { get; set; } = string.Empty;
}
