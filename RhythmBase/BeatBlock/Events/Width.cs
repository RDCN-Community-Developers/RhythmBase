namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// [DEPRECATED] Set Width
/// </summary>
/// <remarks>
/// No description
/// </remarks>
[RDJsonObjectSerializable]
public record class Width : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.Width;
    /// <summary>
    /// New width to ease to
    /// </summary>
    public int NewWidth { get; set; } 
    /// <summary>
    /// Length of ease (in beats)
    /// </summary>
    public float Duration { get; set; } 
    /// <summary>
    /// Ease function to use
    /// </summary>
    public Ease? Ease { get; set; } 
}
