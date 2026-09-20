namespace RhythmBase.RhythmDoctor.Components.Conditions;

/// <summary>
/// Represents a condition based on the last hit in a rhythm game.
/// </summary>
public record class LastHitCondition : BaseConditional
{
	///<inheritdoc/>
	public override ConditionType Type => ConditionType.LastHit;
	/// <summary>
	/// Gets or sets the row where the last hit occurred.
	/// </summary>
	public sbyte Row { get; set; }
	/// <summary>
	/// Gets or sets the result that determines under what condition the event will be executed.
	/// </summary>
	public HitResult Result { get; set; }
}
