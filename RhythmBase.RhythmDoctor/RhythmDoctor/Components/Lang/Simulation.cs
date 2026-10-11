using System;

namespace RhythmBase.RhythmDoctor.Components.Lang;

/// <summary>
/// The rank-cap rule of the sandbox environment, backing the <c>atLeastRank</c> built-in.
/// </summary>
public interface IRankSimulation
{
	/// <summary>
	/// Gets or sets the simulated rank cap.
	/// </summary>
	ExpressionContext.RDRank RankCap { get; set; }

	/// <summary>
	/// Evaluates whether the simulated rank is at least <paramref name="rank"/>.
	/// </summary>
	/// <param name="rank">The rank literal, for example <see cref="ExpressionContext.RDRank.SPlus"/>.</param>
	/// <returns><see langword="true"/> when the simulated rank reaches <paramref name="rank"/>.</returns>
	bool AtLeast(ExpressionContext.RDRank rank);
}

/// <summary>
/// The success-rate rule of the sandbox environment, backing the <c>atLeastNPerfects</c> built-in.
/// </summary>
public interface IPerfectsSimulation
{
	/// <summary>
	/// Gets or sets the simulated perfect-hit ratio above which the check succeeds.
	/// </summary>
	float SuccessRate { get; set; }

	/// <summary>
	/// Evaluates whether the perfect ratio exceeds <see cref="SuccessRate"/>.
	/// </summary>
	/// <param name="hitsToCheck">The number of hits in the window.</param>
	/// <param name="numberOfPerfects">The number of perfect hits in the window.</param>
	/// <returns><see langword="true"/> when the perfect ratio exceeds the simulated rate.</returns>
	bool AtLeastNPerfects(int hitsToCheck, int numberOfPerfects);
}

/// <summary>
/// The default sandbox simulation: the current rank-cap and success-rate implementation. It
/// implements both <see cref="IRankSimulation"/> and <see cref="IPerfectsSimulation"/>.
/// </summary>
public sealed class DefaultSimulation : IRankSimulation, IPerfectsSimulation
{
	/// <inheritdoc/>
	public ExpressionContext.RDRank RankCap { get; set; } = ExpressionContext.RDRank.S;

	/// <inheritdoc/>
	public float SuccessRate { get; set; } = 0.9f;

	/// <inheritdoc/>
	public bool AtLeast(ExpressionContext.RDRank rank) => RankCap <= rank;

	/// <inheritdoc/>
	public bool AtLeastNPerfects(int hitsToCheck, int numberOfPerfects)
		=> numberOfPerfects / (float)hitsToCheck > SuccessRate;
}
