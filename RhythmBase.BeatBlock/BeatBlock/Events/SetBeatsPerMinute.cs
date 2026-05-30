namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Set BPM
/// </summary>
/// <remarks>
/// Change the BPM
/// </remarks>
[JsonObjectSerializable]
public record class SetBeatsPerMinute : BaseEvent, IPureEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.SetBeatsPerMinute;
    /// <summary>
    /// BPM to change to
    /// </summary>
    public float Bpm { get; set; } 
}
