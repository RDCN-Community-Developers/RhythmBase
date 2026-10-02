using System;

namespace RhythmBase.RhythmDoctor.Components.Lang;

/// <summary>
/// The mutable state shared by expression evaluations: the 30 fixed slots plus the
/// <see cref="IVariables"/> virtual environment.
/// </summary>
/// <remarks>
/// Pass the same <see cref="ExpressionContext"/> to an <see cref="ExpressionEvaluator"/> and a
/// <see cref="ConditionalExpressionEvaluator"/> to share the <c>i</c>/<c>f</c>/<c>b</c> slots
/// between them.
/// </remarks>
public sealed class ExpressionContext
{
	/// <summary>
	/// Initializes a new context over the given virtual environment.
	/// </summary>
	/// <param name="variables">The virtual environment; must not be <see langword="null"/>.</param>
	/// <exception cref="ArgumentNullException">When <paramref name="variables"/> is <see langword="null"/>.</exception>
	public ExpressionContext(IVariables variables)
	{
		Variables = variables ?? throw new ArgumentNullException(nameof(variables));
	}

	/// <summary>
	/// Gets the virtual environment supplying every variable outside the 30 fixed slots.
	/// </summary>
	public IVariables Variables { get; }

	/// <summary>
	/// Gets the 10 integer slots <c>i0</c>–<c>i9</c>.
	/// </summary>
	public int[] I { get; } = new int[10];

	/// <summary>
	/// Gets the 10 float slots <c>f0</c>–<c>f9</c>.
	/// </summary>
	public float[] F { get; } = new float[10];

	/// <summary>
	/// Gets the 10 boolean slots <c>b0</c>–<c>b9</c>.
	/// </summary>
	public bool[] B { get; } = new bool[10];

	/// <summary>
	/// Tests whether a name refers to one of the 30 fixed slots.
	/// </summary>
	/// <param name="name">The variable name.</param>
	/// <param name="kind">One of <c>i</c>, <c>f</c> or <c>b</c>; otherwise <c>'\0'</c>.</param>
	/// <param name="index">The slot index; otherwise <c>-1</c>.</param>
	/// <returns><see langword="true"/> when <paramref name="name"/> is a fixed slot.</returns>
	internal static bool TryParseFixed(string name, out char kind, out int index)
	{
		kind = '\0';
		index = -1;
		if (name.Length != 2)
			return false;
		char k = name[0];
		if (k != 'i' && k != 'f' && k != 'b')
			return false;
		char d = name[1];
		if (d < '0' || d > '9')
			return false;
		kind = k;
		index = d - '0';
		return true;
	}
}
