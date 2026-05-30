namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Advance Text Decoration
/// </summary>
/// <remarks>
/// Adds a new syllable to a targeted TextDeco object
/// </remarks>
[JsonObjectSerializable]
public record class AdvanceTextDecoration : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.AdvanceTextDecoration;
    /// <summary>
    /// ID of the text deco to advance lyrics for
    /// </summary>
    public string Id { get; set; } = string.Empty;
}
