namespace RhythmBase.RhythmDoctor.Components.Conditions;

/// <summary>
/// Represents a custom condition with an expression.
/// </summary>
public class CustomCondition : BaseConditional
{
	/// <summary>
	/// Gets or sets the expression for the custom condition.
	/// </summary>
	public string Expression { get; set; } = "";
	///<inheritdoc/>
	public override ConditionType Type => ConditionType.Custom;
}
