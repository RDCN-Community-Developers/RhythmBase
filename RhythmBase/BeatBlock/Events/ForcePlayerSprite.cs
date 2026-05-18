namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Force Player Sprite
/// </summary>
/// <remarks>
/// Forces the player to use a specific sprite
/// </remarks>
[RDJsonObjectSerializable]
public record class ForcePlayerSprite : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.ForcePlayerSprite;
    /// <summary>
    /// Name of sprite, blank for no force, or "none" for no sprite. Can also be a PNG file
    /// </summary>
    public string? SpriteName { get; set; } = string.Empty;
    /// <summary>
    /// Name of deco shader
    /// </summary>
    public string? Shader { get; set; } = string.Empty;
}
