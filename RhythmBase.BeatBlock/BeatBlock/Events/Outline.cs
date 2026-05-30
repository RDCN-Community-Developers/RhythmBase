using RhythmBase.BeatBlock.Components;
namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Outline
/// </summary>
/// <remarks>
/// Adds an outline to objects
/// </remarks>
[JsonObjectSerializable]
public record class Outline : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.Outline;
    /// <summary>
    /// Show an outline?
    /// </summary>
    public bool Enable { get; set; }
    /// <summary>
    /// Color index
    /// </summary>
    public ColorIndex Color { get; set; }
}
