namespace RhythmBase.RhythmDoctor.Components.Conditions;

/// <summary>
/// Represents a condition that controls whether narration (spoken or textual guidance) is enabled.
/// </summary>
public class NarrationCondition : BaseConditional
{
	///<inheritdoc/>
	public override ConditionType Type => ConditionType.Narration;

	/// <summary>
	/// Gets or sets a value indicating whether narration is enabled when this condition is met.
	/// </summary>
	public bool NarrationEnabled { get; set; }
}
