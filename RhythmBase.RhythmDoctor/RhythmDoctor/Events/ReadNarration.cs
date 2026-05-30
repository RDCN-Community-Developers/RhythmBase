namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event for reading narration.
/// </summary>
[JsonObjectSerializable]
public record class ReadNarration : BaseEvent
{
	///<inheritdoc/>
	public override EventType Type => EventType.ReadNarration;
	/// <summary>
	/// Gets or sets the text of the narration.
	/// </summary>
	public string Text { get; set; } = "";
	/// <summary>
	/// Gets or sets the category of the narration.
	/// </summary>
	public NarrationCategory Category { get; set; } = NarrationCategory.Description;
	///<inheritdoc/>
	public override Tab Tab => Tab.Sounds;
	///<inheritdoc/>
	public override string ToString() => base.ToString() + $" {Text}";
}
