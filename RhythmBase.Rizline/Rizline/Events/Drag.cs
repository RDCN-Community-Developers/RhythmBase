using RhythmBase.Rizline.Components;
using RhythmBase.Rizline.Rizline;

namespace RhythmBase.Rizline.Events;

/// <summary>
/// Drag note implementation. 
/// </summary>
[JsonObjectSerializable]
public record class Drag : BaseEvent, INote
{
    /// <inheritdoc/>
    public override EventType Type => EventType.Drag;
    /// <inheritdoc/>
    public float FloorPosition { get; set; }
}
