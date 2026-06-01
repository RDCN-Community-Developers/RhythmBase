using RhythmBase.Rizline.Rizline;

namespace RhythmBase.Rizline.Events;

/// <summary>
/// Represents a Riztime section's timing boundaries and transition duration.
/// </summary>
[JsonObjectSerializable]
public record class ChallengeTime : BaseEvent
{
    public override EventType Type => EventType.ChallengeTime;
    /// <summary>
    /// Checkpoint time used for internal checks (default 0.0). 
    /// </summary>
    public float CheckPoint { get; set; }

    /// <summary>
    /// Riztime start time in ticks. 
    /// </summary>
    public float Start { get; set; }

    /// <summary>
    /// Riztime end time in ticks. 
    /// </summary>
    public float End { get; set; }

    /// <summary>
    /// Transition time in seconds when switching into/out of Riztime. 
    /// </summary>
    public float TransTime { get; set; }
}
