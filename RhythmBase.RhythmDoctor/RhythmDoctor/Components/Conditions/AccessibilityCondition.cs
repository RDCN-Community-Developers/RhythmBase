namespace RhythmBase.RhythmDoctor.Components.Conditions;

/// <summary>
/// Represents a condition that determines accessibility based on specific effects.
/// </summary>
public record class AccessibilityCondition : BaseConditional
{
	///<inheritdoc/>
	public override ConditionType Type => ConditionType.Accessibility;

	/// <summary>
	/// Gets or sets the effect type whose accessibility should be evaluated.
	/// </summary>
	public EffectType TargetEffectType { get; set; }
}