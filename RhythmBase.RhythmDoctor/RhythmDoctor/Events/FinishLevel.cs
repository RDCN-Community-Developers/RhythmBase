namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event that shows the level rank screen after finishing a level.
/// This event is used to indicate the end of a level and trigger the display of the rank screen.
/// </summary>
[JsonObjectSerializable]
public record class FinishLevel : BaseEvent
{
	///<inheritdoc/>
	public override EventType Type => EventType.FinishLevel;
	///<inheritdoc/>
	public override Tab Tab => Tab.Actions;
}
