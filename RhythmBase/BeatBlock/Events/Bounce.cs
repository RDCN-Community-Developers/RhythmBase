using RhythmBase.BeatBlock.Components;
using RhythmBase.Global.Components.Easing;

namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Bounce
/// </summary>
/// <remarks>
/// No description
/// </remarks>
[RDJsonObjectSerializable]
public record class Bounce : BaseEvent, IChartEvent, IPureEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.Bounce;
    /// <summary>
    /// How many times to bounce
    /// </summary>
    public int Bounces { get; set; } 
    /// <summary>
    /// Time between bounces
    /// </summary>
    public float Delay { get; set; } 
    /// <summary>
    /// Rotation between bounces
    /// </summary>
    public float Rotation { get; set; } 
    /// <summary>
    /// Make this note a Tap note
    /// </summary>
    public bool Tap { get; set; } 
    /// <summary>
    /// Angle to end up at for initial approach
    /// </summary>
    public float? EndAngle { get; set; } 
    /// <summary>
    /// Ease to use while rotating on initial approach
    /// </summary>
    public EaseType? SpinEase { get; set; } 
    /// <summary>
    /// Speed multiplier for approach
    /// </summary>
    public float? SpeedMult { get; set; } 
    /// <summary>
    /// Height multiplier for bounce
    /// </summary>
    public float? HeightMult { get; set; } 
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
