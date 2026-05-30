namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event that sets the main application window.
/// </summary>
[JsonObjectSerializable]
public record class SetMainWindow : BaseWindowEvent
{
	///<inheritdoc/>
	public override EventType Type => EventType.SetMainWindow;
}
