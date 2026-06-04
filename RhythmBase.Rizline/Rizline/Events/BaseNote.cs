namespace RhythmBase.Rizline.Events;

/// <summary>
/// Common interface implemented by all note types. 
/// </summary>
[JsonObjectHasSerializer(typeof(RhythmBase.Rizline.Converters.MemberConverterBaseNote<>))]
public abstract record class BaseNote : BaseEvent, IBaseEvent
{
    /// <summary>
    /// Cumulative floor position (vertical offset) of the note in its canvas. 
    /// </summary>
    public float FloorPosition { get; set; }
}
