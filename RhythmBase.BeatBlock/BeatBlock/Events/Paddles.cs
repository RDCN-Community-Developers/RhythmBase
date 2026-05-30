using RhythmBase.Global.Components.Easing;
namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Edit Paddles
/// </summary>
/// <remarks>
/// Change paddle properties
/// </remarks>
[JsonObjectSerializable]
public record class Paddles : BaseEvent, IChartEvent, IPureEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.Paddles;
    /// <summary>
    /// What paddle to change? 0 = all paddles
    /// </summary>
    public int Paddle { get; set; } 
    /// <summary>
    /// New width to ease to
    /// </summary>
    public int NewWidth { get; set; } 
    /// <summary>
    /// New height to ease to
    /// </summary>
    public int NewHeight { get; set; } 
    /// <summary>
    /// New angle to ease to
    /// </summary>
    public int NewAngle { get; set; } 
    /// <summary>
    /// Length of ease (in beats)
    /// </summary>
    public float Duration { get; set; } 
    /// <summary>
    /// Ease function to use
    /// </summary>
    public EaseType Ease { get; set; } 
}
