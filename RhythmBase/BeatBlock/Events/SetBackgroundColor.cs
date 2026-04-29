using RhythmBase.BeatBlock.Components;
namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Set BG color
/// </summary>
/// <remarks>
/// Sets the background color channel
/// </remarks>
public record class SetBackgroundColor : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.SetBackgroundColor;
    /// <summary>
    /// Color index for the background
    /// </summary>
    public ColorIndex? Color { get; set; }
    /// <summary>
    /// Color index for the area outside of the camera
    /// </summary>
    public ColorIndex? VoidColor { get; set; }
}
