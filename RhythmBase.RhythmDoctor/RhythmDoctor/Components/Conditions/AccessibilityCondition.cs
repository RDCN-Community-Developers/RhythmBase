namespace RhythmBase.RhythmDoctor.Components.Conditions;

/// <summary>
/// Represents a condition that determines accessibility based on specific effects.
/// </summary>
public class AccessibilityCondition : BaseConditional
{
	///<inheritdoc/>
	public override ConditionType Type => ConditionType.Accessibility;

	/// <summary>
	/// Gets or sets the effect type whose accessibility should be evaluated.
	/// </summary>
	public EffectType TargetEffectType { get; set; }
}
/// <summary>
/// Defines the types of effects that impact accessibility.
/// </summary>
[JsonEnumSerializable]
public enum EffectType
{
	/// <summary>
	/// Indicates visually intensive or flashing effects.
	/// </summary>
	Flashy,

	/// <summary>
	/// Indicates narration or spoken dialogue.
	/// </summary>
	Narration,
}