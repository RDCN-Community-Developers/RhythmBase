namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Show Results
/// </summary>
/// <remarks>
/// Shows the results screen, ending the level
/// </remarks>
[JsonObjectSerializable]
public record class ShowResults : BaseEvent, IPureEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.ShowResults;
    /// <summary>
    /// Skip the cranky grow transition?
    /// </summary>
    public bool SkipGrow { get; set; } 
    /// <summary>
    /// Fade out the song? set to 0 to let it play until the end.
    /// </summary>
    public float? FadeTime { get; set; } 
}
