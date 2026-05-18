namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// [DEPRECATED] Set Paddle Count
/// </summary>
/// <remarks>
/// No description
/// </remarks>
[RDJsonObjectSerializable]
public record class PaddleCount : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.PaddleCount;
    /// <summary>
    /// New number of Paddles
    /// </summary>
    public int Paddles { get; set; } 
}
