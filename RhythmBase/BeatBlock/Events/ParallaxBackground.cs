namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Parallax Background
/// </summary>
/// <remarks>
/// Initializes an object
/// </remarks>
[RDJsonObjectSerializable]
public record class ParallaxBackground : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.ParallaxBackground;
    /// <summary>
    /// Name of the json file
    /// </summary>
    public string FileName { get; set; } = string.Empty;
    /// <summary>
    /// the name this is stored under in cs.vfx.objects
    /// </summary>
    public string Id { get; set; } = string.Empty;
}
