using RhythmBase.BeatBlock.Components;
using RhythmBase.Global.Components.Easing;
namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// MineHold
/// </summary>
/// <remarks>
/// No description
/// </remarks>
[RDJsonObjectSerializable]
public record class MineHold : BaseEvent, IChartEvent, IPureEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.MineHold;
    /// <summary>
    /// Angle for end of the hold
    /// </summary>
    public float Angle2 { get; set; }
    /// <summary>
    /// How many beats the hold lasts
    /// </summary>
    public float Duration { get; set; }
    /// <summary>
    /// Force a certain number of line segments
    /// </summary>
    public int? Segments { get; set; }
    /// <summary>
    /// Change ease from angle1 to angle2
    /// </summary>
    public EaseType? HoldEase { get; set; }
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
    /// If this mineHold has been hit, how frequently should it check for hits again?
    /// </summary>
    public float? TickRate { get; set; }
    /// <summary>
    /// Color channel 0 (default white)
    /// </summary>
    public ColorIndex? Color0 { get; set; }
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
