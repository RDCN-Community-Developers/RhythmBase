namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Play Videobg
/// </summary>
/// <remarks>
/// No description
/// </remarks>
[RDJsonObjectSerializable]
public record class VideoBackground : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.VideoBackground;
}
