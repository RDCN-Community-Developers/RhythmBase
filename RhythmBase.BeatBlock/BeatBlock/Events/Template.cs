namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// _TEMPLATE
/// </summary>
/// <remarks>
/// No description
/// </remarks>
[JsonObjectSerializable]
public record class Template : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.Template;
}
