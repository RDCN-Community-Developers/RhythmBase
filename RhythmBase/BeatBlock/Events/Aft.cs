namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// AFT
/// </summary>
/// <remarks>
/// Freeze frames, feedback loops, and more!
/// </remarks>
public record class Aft : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.Aft;
    /// <summary>
    /// Unique ID for the AFT. Accessible in a deco by prefixing "@aft" to the ID.
    /// </summary>
    public string Id { get; set; } = string.Empty;
    /// <summary>
    /// Duration of the capture. Use 0 for a single frame
    /// </summary>
    public float Duration { get; set; }
}
