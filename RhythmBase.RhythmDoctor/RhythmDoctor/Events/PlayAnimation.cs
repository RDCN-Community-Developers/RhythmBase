namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an action to play an animation.
/// </summary>
[JsonObjectSerializable]
public record class PlayAnimation : BaseDecorationAction
{
	///<inheritdoc/>
	public override EventType Type => EventType.PlayAnimation;
	///<inheritdoc/>
	public override Tab Tab => Tab.Decorations;
	/// <summary>
	/// Gets or sets the expression for the animation.
	/// </summary>
	public string Expression { get; set; } = string.Empty;
	///<inheritdoc/>
	public override string ToString() => base.ToString() + $" Expression:{Expression}";
}
