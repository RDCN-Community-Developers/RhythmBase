namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event that renames or modifies the name of a game window.
/// </summary>
[JsonObjectSerializable]
public record class RenameWindow : BaseWindowEvent
{
	/// <summary>
	/// The text to set, append, or use when renaming the window.
	/// </summary>
	public string Text { get; set; } = "";

	/// <summary>
	/// Describes how <see cref="Text"/> should be applied to the target window's name.
	/// </summary>
	public WindowNameAction Action { get; set; } = WindowNameAction.Set;

	/// <inheritdoc/>
	public override EventType Type => EventType.RenameWindow;
}