using RhythmBase.BeatBlock.Components;
using RhythmBase.Global.Components.Easing;
namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Set color
/// </summary>
/// <remarks>
/// Changes a color in the palette
/// </remarks>
[JsonObjectSerializable]
public record class SetColor : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.SetColor;
    /// <summary>
    /// Use HOM?
    /// </summary>
    public bool Enable { get; set; }
    /// <summary>
    /// Color index
    /// </summary>
    public ColorIndex Color { get; set; }
    /// <summary>
    /// Length of ease
    /// </summary>
    public float? Duration { get; set; }
    /// <summary>
    /// Ease function to use
    /// </summary>
    public EaseType? Ease { get; set; }
}
