namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event to set the play style.
/// </summary>
[JsonObjectSerializable]
public record class SetPlayStyle : BaseEvent
{
	/// <summary>
	/// Gets or sets the play style.
	/// </summary>
	[JsonAlias("PlayStyle")]
	public PlayStyleType PlayStyle { get; set; } = PlayStyleType.Normal;
	/// <summary>
	/// Gets or sets the next bar.
	/// </summary>
	/// <remarks>
	/// If <see cref="IsRelative"/> is true, the play style will be applied after the specified number of bars. Otherwise, it will be applied at the specified bar.
	/// </remarks>
	[JsonAlias("NextBar")]
	public int NextBar { get; set; } = 1;
	/// <summary>
	/// Gets or sets a value indicating whether the next bar is relative.
	/// </summary>
	[JsonAlias("Relative")]
	public bool IsRelative { get; set; } = true;
	///<inheritdoc/>
	public override EventType Type => EventType.SetPlayStyle;
	///<inheritdoc/>
	public override Tab Tab => Tab.Actions;
}
