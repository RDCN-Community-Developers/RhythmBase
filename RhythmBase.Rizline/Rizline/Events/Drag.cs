namespace RhythmBase.Rizline.Events;

/// <summary>
/// Drag note implementation. 
/// </summary>
[JsonObjectSerializable]
public record class Drag : BaseNote
{
    /// <inheritdoc/>
    public override EventType Type => EventType.Drag;
}
