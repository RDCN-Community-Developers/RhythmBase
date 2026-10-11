namespace RhythmBase.RhythmDoctor.Components.Lang;

/// <summary>
/// A neutral environment used when no sandbox was configured. It exposes no variables and every
/// simulation function returns a neutral value, so levels that only use the 30 fixed slots still
/// evaluate. Supply your own <see cref="IVariables"/> for anything else.
/// </summary>
internal sealed class EmptyVariables : VariablesBase
{
	internal static readonly EmptyVariables Instance = new EmptyVariables();

	private EmptyVariables()
	{
	}

	/// <inheritdoc/>
	public override bool TryGetVariable(string name, out object? value)
	{
		value = null;
		return false;
	}

	/// <inheritdoc/>
	public override bool TrySetVariable(string name, object? value) => false;

	/// <inheritdoc/>
	public override int Rand(int value) => 0;

	/// <inheritdoc/>
	public override bool atLeastRank(Rank rank) => false;

	/// <inheritdoc/>
	public override bool atLeastNPerfects(int hitsToCheck, int numberOfPerfects) => false;
}
