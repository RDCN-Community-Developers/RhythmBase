namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// A source of random numbers for a sandbox environment. Implement it with an iterator to make a
/// deterministic sandbox.
/// </summary>
public interface IRandomSimulator
{
	/// <summary>Returns a random integer in <c>[minInclusive, maxExclusive)</c>.</summary>
	/// <param name="minInclusive">The inclusive lower bound.</param>
	/// <param name="maxExclusive">The exclusive upper bound.</param>
	/// <returns>The random value.</returns>
	int Next(int minInclusive, int maxExclusive);
}

/// <summary>
/// The default <see cref="IRandomSimulator"/> backed by <see cref="Random"/>.
/// </summary>
public sealed class RandomSimulator : IRandomSimulator
{
	private readonly Random _random = new();

	/// <inheritdoc/>
	public int Next(int minInclusive, int maxExclusive) => _random.Next(minInclusive, maxExclusive);
}
