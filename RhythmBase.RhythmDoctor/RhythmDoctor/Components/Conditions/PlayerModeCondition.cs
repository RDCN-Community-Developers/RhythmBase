namespace RhythmBase.RhythmDoctor.Components.Conditions;

/// <summary>
/// Represents a condition based on the player mode.
/// </summary>
public record class PlayerModeCondition : BaseConditional
{
	/// <summary>
	/// Gets or sets a value indicating whether two-player mode is enabled.
	/// </summary>
	public bool TwoPlayerMode { get; set; }
	///<inheritdoc/>
	public override ConditionType Type => ConditionType.PlayerMode;
}
