using RhythmBase.RhythmDoctor.Extensions;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// The interface for events that advance the text display in the game.
/// </summary>
public interface IAdvanceText : IBaseEvent, IDurationEvent
{
	/// <summary>
	/// The duration of the text advance event. This property is nullable, allowing for events that may not have a specific duration.
	/// </summary>
	new float? Duration { get; set; }
}