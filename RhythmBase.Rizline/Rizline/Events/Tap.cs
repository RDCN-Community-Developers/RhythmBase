namespace RhythmBase.Rizline.Events;

/// <summary>
/// Tap note implementation. 
/// </summary>
[JsonObjectSerializable]
public record class Tap : BaseEvent, BaseNote
{
    /// <inheritdoc/>
    public override EventType Type => EventType.Tap;
    /// <inheritdoc/>
    public float FloorPosition { get; set; }
}
