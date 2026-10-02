using System.Dynamic;

namespace RhythmBase.RhythmDoctor.Components.Lang;

/// <summary>
/// The sandbox environment used while evaluating expressions. It supplies both the variables
/// and the simulation functions a level needs.
/// </summary>
/// <remarks>
/// <para>
/// The evaluator owns the 30 fixed slots (<c>i0</c>–<c>i9</c>, <c>f0</c>–<c>f9</c> and
/// <c>b0</c>–<c>b9</c>) itself. Every other identifier is exposed by this environment as a
/// dynamic member.
/// </para>
/// <para>
/// Because the environment is an <see cref="IDynamicMetaObjectProvider"/>, reading and writing
/// variables uses the dynamic member protocol: an implementation returns <see langword="false"/>
/// from <see cref="DynamicObject.TryGetMember"/> for an unknown name, and <see langword="false"/>
/// from <see cref="DynamicObject.TrySetMember"/> for a read-only or unknown name.
/// </para>
/// <para>
/// The simulation functions are declared but intentionally not implemented here: an upper layer
/// decides how to model them. For example, <see cref="Rand"/> can be backed by an iterator to
/// make a deterministic sandbox.
/// </para>
/// <para>
/// Implementations are expected to derive from <see cref="VariablesBase"/> (which derives from
/// <see cref="DynamicObject"/> and implements the dynamic plumbing).
/// </para>
/// </remarks>
public interface IVariables : IDynamicMetaObjectProvider
{
	/// <summary>
	/// Returns a random integer.
	/// </summary>
	/// <param name="value">The inclusive upper bound. A negative value mirrors the range to the negative side.</param>
	/// <returns>A value in <c>[1, value]</c>, or in <c>[-|value|, -1]</c> when <paramref name="value"/> is negative.</returns>
	int Rand(int value);

	/// <summary>
	/// Returns whether the simulated current rank is at least <paramref name="rank"/>.
	/// </summary>
	/// <param name="rank">The rank to compare against.</param>
	/// <returns><see langword="true"/> when the current rank is at least <paramref name="rank"/>.</returns>
	bool atLeastRank(Rank rank);

	/// <summary>
	/// Returns whether the last hits contain enough perfects.
	/// </summary>
	/// <param name="hitsToCheck">The number of recent hits to inspect.</param>
	/// <param name="numberOfPerfects">The number of perfect hits required.</param>
	/// <returns><see langword="true"/> when the condition holds.</returns>
	bool atLeastNPerfects(int hitsToCheck, int numberOfPerfects);
}
