namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Run tag
/// </summary>
/// <remarks>
/// Runs a tag, which is a collection of events
/// </remarks>
[JsonObjectSerializable]
public record class Tag : BaseEvent, IPureEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.Tag;
    /// <summary>
    /// Name of tag
    /// </summary>

    [JsonAlias("tag")]
    public string TagName { get; set; } = string.Empty;
    /// <summary>
    /// Times to repeat
    /// </summary>
    public int? Repeats { get; set; } 
    /// <summary>
    /// Beats between repeats
    /// </summary>
    public float? RepeatDelay { get; set; } 
}
