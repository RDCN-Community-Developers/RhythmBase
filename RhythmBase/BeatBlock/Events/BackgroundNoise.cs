namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// BG noise (DEPRECIATED)
/// </summary>
/// <remarks>
/// No description
/// </remarks>
[RDJsonObjectSerializable]
public record class BackgroundNoise : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.BackgroundNoise;
    /// <summary>
    /// Use BG noise?
    /// </summary>
    public bool Enable { get; set; } 
    /// <summary>
    /// Noise image to load
    /// </summary>
    public string Filename { get; set; } = string.Empty;
}
