namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Comment
/// </summary>
/// <remarks>
/// Has a textbox to put text in, but does nothing to the level itself.
/// </remarks>
[RDJsonObjectSerializable]
public record class Comment : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.Comment;
    /// <summary>
    /// Your comment goes here.
    /// </summary>
    public string Text { get; set; } = string.Empty;
}
