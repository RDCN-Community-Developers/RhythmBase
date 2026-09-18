using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event to show a dialogue in the game.
/// </summary>
[JsonObjectSerializable]
public record class ShowDialogue : BaseEvent, IRoomEvent
{
	/// <summary>
	/// Gets or sets the text of the dialogue.
	/// </summary>
	public string Text { get; set; } = string.Empty;
	/// <summary>
	/// Gets or sets the side of the panel where the dialogue will be shown.
	/// </summary>
	public DialogueSide PanelSide { get; set; } = DialogueSide.Bottom;
	/// <summary>
	/// Gets or sets the side of the portrait in the dialogue.
	/// </summary>
	public DialoguePortraitSide PortraitSide { get; set; } = DialoguePortraitSide.Left;
	/// <summary>
	/// Gets or sets a value indicating whether text sounds should be played.
	/// </summary>
	public bool PlayTextSounds { get; set; } = true;
	///<inheritdoc/>
	public override EventType Type => EventType.ShowDialogue;
	///<inheritdoc/>
	public override Tab Tab => Tab.Actions;
	///<inheritdoc/>
	public Room Rooms { get; set; } = new Room([4]);
}
