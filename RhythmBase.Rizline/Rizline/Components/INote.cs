using RhythmBase.Rizline.Events;
using RhythmBase.Rizline.Rizline;

namespace RhythmBase.Rizline.Components;

/// <summary>
/// Common interface implemented by all note types. 
/// </summary>
public interface INote : IBaseEvent
{
    /// <summary>
    /// Kind of the note (Tap/Drag/Hold). 
    /// </summary>
    public EventType Type { get; }

    /// <summary>
    /// Cumulative floor position (vertical offset) of the note in its canvas. 
    /// </summary>
    public float FloorPosition { get; set; }
}
