using System;

namespace RhythmBase.RhythmDoctor.Components.Lang;

/// <summary>
/// The built-in functions exposed to expressions. Each is backed by a delegate that can be
/// replaced when simulating a different environment, and invoked either by the evaluator or
/// dynamically (<c>((dynamic)context).Rand(6)</c>).
/// </summary>
public partial class ExpressionContext
{
	private static readonly Random _random = new Random();

	/// <summary>
	/// Gets or sets the implementation of <c>Rand(n)</c>, returning a value in <c>1..n</c>
	/// (or <c>-1..n</c> for negative input).
	/// </summary>
	public Func<int, int> Rand { get; set; } = DefaultRand;

	/// <summary>
	/// Gets or sets the implementation of <c>IIf(condition, whenTrue, whenFalse)</c>.
	/// </summary>
	public Func<bool, object?, object?, object?> IIf { get; set; } = DefaultIIf;

	private static object? DefaultIIf(bool condition, object? whenTrue, object? whenFalse) => condition ? whenTrue : whenFalse;

	private static int DefaultRand(int value)
	{
		if (value > 0)
			return _random.Next(1, value + 1);
		if (value < 0)
			return -_random.Next(1, -value + 1);
		return 0;
	}
}
