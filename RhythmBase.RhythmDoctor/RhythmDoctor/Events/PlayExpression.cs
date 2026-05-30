namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event that plays an expression.
/// </summary>
[JsonObjectSerializable]
public record class PlayExpression : BaseRowAction
{
	/// <summary>
	/// Gets or sets the expression to be played.
	/// </summary>
	public string Expression { get; set; } = string.Empty;
	/// <summary>
	/// Gets or sets a value indicating whether to replace the current expression.
	/// </summary>
	public bool Replace { get; set; } = false;
	/// <summary>
	/// Gets or sets the target string used for replacement operations.
	/// </summary>
	[JsonCondition($"$&.{nameof(Replace)}")]
	public string Target { get; set; } = string.Empty;
	///<inheritdoc/>
	public override EventType Type => EventType.PlayExpression;
	///<inheritdoc/>
	public override Tab Tab => Tab.Actions;
	///<inheritdoc/>
	public override string ToString() => base.ToString() + $" {Expression}";
}
