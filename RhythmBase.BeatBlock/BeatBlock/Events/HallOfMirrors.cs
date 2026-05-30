namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Hall of Mirrors
/// </summary>
/// <remarks>
/// When enabled, the screen will no longer be cleared between frames, creating a "hall of mirrors" effect
/// </remarks>
[JsonObjectSerializable]
public record class HallOfMirrors : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.HallOfMirrors;
    /// <summary>
    /// Use HOM?
    /// </summary>
    public bool Enable { get; set; }
}
