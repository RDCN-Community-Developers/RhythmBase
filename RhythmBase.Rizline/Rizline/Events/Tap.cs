namespace RhythmBase.Rizline.Events;

/// <summary>
/// Tap note implementation. 
/// </summary>
[JsonObjectSerializable]
public record class Tap : BaseNote
{
    /// <inheritdoc/>
    public override EventType Type => EventType.Tap;
}
