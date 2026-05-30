namespace RhythmBase.RhythmDoctor.Components.Conditions;

/// <summary>
/// Represents a condition based on the number of times it can be executed.
/// </summary>
public class TimesExecutedCondition : BaseConditional
{
	/// <summary>
	/// Gets or sets the maximum number of executions allowed.
	/// </summary>
	public int MaxTimes { get; set; }
	///<inheritdoc/>
	public override ConditionType Type => ConditionType.TimesExecuted;
}
