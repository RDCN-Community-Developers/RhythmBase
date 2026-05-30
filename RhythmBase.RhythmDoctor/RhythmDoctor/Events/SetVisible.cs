namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an action to set the visibility of a decoration.
/// </summary>
[JsonObjectSerializable]
public record class SetVisible : BaseDecorationAction
{
	///<inheritdoc/>
	public override EventType Type => EventType.SetVisible;
	///<inheritdoc/>
	public override Tab Tab => Tab.Decorations;

	/// <summary>
	/// Gets or sets a value indicating whether the decoration is visible.
	/// </summary>
	public bool Visible { get; set; }
	///<inheritdoc/>
	public override string ToString() => base.ToString() + $" {Visible}";
}
