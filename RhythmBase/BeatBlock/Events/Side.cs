using RhythmBase.BeatBlock.Components;
using RhythmBase.Global.Components.Easing;
namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Side
/// </summary>
/// <remarks>
/// A note that must be hit from the side
/// </remarks>
public record class Side : BaseEvent, IChartEvent, IPureEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.Side;
    /// <summary>
    /// Show a directional indicator?
    /// </summary>
    public SideIndicatorTypes? Indicator { get; set; }
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
    /// Make this note a Tap note
    /// </summary>
    public bool Tap { get; set; }
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
