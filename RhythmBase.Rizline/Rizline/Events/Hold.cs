using RhythmBase.Rizline.Components;
using RhythmBase.Rizline.Rizline;

namespace RhythmBase.Rizline.Events;

/// <summary>
/// Hold (sustain) note implementation. Stores tail information as well. 
/// </summary>
[JsonObjectSerializable]
public record class Hold : BaseEvent, INote
{
    /// <inheritdoc/>
    public override EventType Type => EventType.Hold;
    /// <inheritdoc/>
    public float FloorPosition { get; set; }

    /// <summary>
    /// End time (tail) of the hold note in ticks. 
    /// </summary>
    public float EndTime { get; set; }

    /// <summary>
    /// Canvas index where the hold tail resides. 
    /// </summary>
    public float EndCanvasindex { get; set; }

    /// <summary>
    /// Height (cumulative) of the hold tail. 
    /// </summary>
    public float Height { get; set; }
}
