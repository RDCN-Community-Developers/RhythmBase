namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Stamp
/// </summary>
/// <remarks>
/// Stamps an image onto an offscreen canvas, which can then be called as a deco.
/// </remarks>
[RDJsonObjectSerializable]
public record class Stamp : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.Stamp;
    /// <summary>
    /// The name of your stamp
    /// </summary>
    public string StampName { get; set; } = string.Empty;
    /// <summary>
    /// The image of your stamp. Only set this once!
    /// </summary>
    public string? Sprite { get; set; } = string.Empty;
    /// <summary>
    /// Unique ID for the stamp canvas. Accessible in a deco by prefixing "@stamp" to the ID.
    /// </summary>
    public string CanvasID { get; set; } = string.Empty;
    /// <summary>
    /// X position
    /// </summary>
    public float X { get; set; }
    /// <summary>
    /// Y position
    /// </summary>
    public float Y { get; set; }
    /// <summary>
    /// Rotation
    /// </summary>
    public float? R { get; set; }
    /// <summary>
    /// X scale
    /// </summary>
    public float? Sx { get; set; }
    /// <summary>
    /// Y scale
    /// </summary>
    public float? Sy { get; set; }
    /// <summary>
    /// X offset
    /// </summary>
    public float? Ox { get; set; }
    /// <summary>
    /// Y offset
    /// </summary>
    public float? Oy { get; set; }
    /// <summary>
    /// X skew
    /// </summary>
    public float? Kx { get; set; }
    /// <summary>
    /// Y skew
    /// </summary>
    public float? Ky { get; set; }
    /// <summary>
    /// Clear all stamps on the selected canvas.
    /// </summary>
    public bool Clear { get; set; }
    /// <summary>
    /// Randomise Stamp
    /// </summary>
    public bool Randomise { get; set; }
    /// <summary>
    /// The minimum frame for random selection. (0-indexed)
    /// </summary>
    public float RandomMin { get; set; }
    /// <summary>
    /// The maximum frame for random selection. (0-indexed)
    /// </summary>
    public float RandomMax { get; set; }
}
