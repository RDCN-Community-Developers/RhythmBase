using RhythmBase.Global.Components.RichText;
using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event to show a dialogue in the game.
/// </summary>
[JsonObjectSerializable]
public record class ShowDialogue : BaseEvent, IRoomEvent
{
	private DialogueExchange dialogueList = [];
	private string text = "";
	/// <summary>
	/// Gets or sets the text of the dialogue.
	/// </summary>
	[JsonAlias("text")]
	internal string Text
	{
		get => text; set
		{
			text = value;
			dialogueList = DialogueExchange.Deserialize(value);
		}
	}
	/// <summary>
	/// Gets or sets the dialogue list. When set, the Text property is updated with the serialized value of the dialogue list.
	/// </summary>
	/// <value>The dialogue list.</value>
	[JsonIgnore]
	public DialogueExchange DialogueList
	{
		get => dialogueList; set
		{
			dialogueList = value;
			text = dialogueList.Serialize();
		}
	}
	/// <summary>
	/// Gets or sets the side of the panel where the dialogue will be shown.
	/// </summary>
	public DialogueSide PanelSide { get; set; } = DialogueSide.Bottom;
	/// <summary>
	/// Gets or sets the side of the portrait in the dialogue.
	/// </summary>
	public DialoguePortraitSide PortraitSide { get; set; } = DialoguePortraitSide.Left;
	/// <summary>
	/// Gets or sets the speed of the dialogue display.
	/// </summary>
	public float Speed { get; set; } = 1;
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
	///<inheritdoc/>
	public override string ToString() => base.ToString() + $" {Text}";
}
