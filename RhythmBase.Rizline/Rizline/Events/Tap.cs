using RhythmBase.Global.Components.Easing;
using RhythmBase.Rizline.Components;
using RhythmBase.Rizline.Rizline;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.Rizline.Events;

/// <summary>
/// Tap note implementation. 
/// </summary>
[JsonObjectSerializable]
public record class Tap : BaseEvent, INote
{
    /// <inheritdoc/>
    public override EventType Type => EventType.Tap;
    /// <inheritdoc/>
    public float FloorPosition { get; set; }
}
