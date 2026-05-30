using RhythmBase.BeatBlock.Components;
using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components;
namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Set joystick color
/// </summary>
/// <remarks>
/// Sets the Joystick LED to a color channel
/// </remarks>
[JsonObjectSerializable]
public record class SetJoystickColor : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.SetJoystickColor;
    /// <summary>
    /// Color index for the joystick
    /// </summary>
    public ColorIndex? Color { get; set; }
    /// <summary>
    /// Starting flash value
    /// </summary>
    public float? StartFlash { get; set; }
    /// <summary>
    /// Ending flash value
    /// </summary>
    public float? EndFlash { get; set; }
    /// <summary>
    /// Starting flash color value
    /// </summary>
    public Color? StartColor { get; set; }
    /// <summary>
    /// Ending flash color value
    /// </summary>
    public Color? EndColor { get; set; }
    /// <summary>
    /// Length of ease
    /// </summary>
    public float? Duration { get; set; }
    /// <summary>
    /// Ease function to use
    /// </summary>
    public EaseType? Ease { get; set; }
    /// <summary>
    /// Times to repeat
    /// </summary>
    public int? Repeats { get; set; }
    /// <summary>
    /// Beats between repeats
    /// </summary>
    public float? RepeatDelay { get; set; }
    /// <summary>
    /// let ColorGlitchRects be active
    /// </summary>
    public bool? ECColorGlitch { get; set; }
}
