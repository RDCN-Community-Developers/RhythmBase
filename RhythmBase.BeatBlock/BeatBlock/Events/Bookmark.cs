namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Bookmark
/// </summary>
/// <remarks>
/// Marks a section of the chart
/// </remarks>
[JsonObjectSerializable]
public record class Bookmark : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.Bookmark;
    /// <summary>
    /// Name of the section after the bookmark
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Information about the section after the bookmark
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
