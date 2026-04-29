using RhythmBase.BeatBlock.Components;
namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Recolor notes
/// </summary>
/// <remarks>
/// Sets note colors
/// </remarks>
public record class RecolorNotes : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.RecolorNotes;
    /// <summary>
    /// What note type should be recolored?
    /// </summary>
    public NoteRecolorTarget Target { get; set; }
    /// <summary>
    /// Color channel 0 (default white)
    /// </summary>
    public ColorIndex? Color0 { get; set; }
    /// <summary>
    /// Color channel 1 (default black)
    /// </summary>
    public ColorIndex? Color1 { get; set; }
}
