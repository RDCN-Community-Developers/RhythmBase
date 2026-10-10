namespace RhythmBase.RhythmDoctor.Components.Lang;

/// <summary>
/// The 30 fixed slots shared by every evaluation: <c>i0</c>–<c>i9</c>, <c>f0</c>–<c>f9</c> and
/// <c>b0</c>–<c>b9</c>. They are reachable statically through <see cref="I"/>, <see cref="F"/> and
/// <see cref="B"/>, and dynamically as <c>i0</c>, <c>f9</c>, <c>b0</c> and so on.
/// </summary>
public partial class ExpressionContext
{
	/// <summary>The number of slots per kind (<c>i</c>, <c>f</c> and <c>b</c>).</summary>
	public const int SlotCount = 10;

	/// <summary>
	/// Gets the 10 integer slots <c>i0</c>–<c>i9</c>.
	/// </summary>
	public int[] I { get; } = new int[SlotCount];

	/// <summary>
	/// Gets the 10 float slots <c>f0</c>–<c>f9</c>.
	/// </summary>
	public float[] F { get; } = new float[SlotCount];

	/// <summary>
	/// Gets the 10 boolean slots <c>b0</c>–<c>b9</c>.
	/// </summary>
	public bool[] B { get; } = new bool[SlotCount];

	/// <summary>
	/// Tests whether a name refers to one of the 30 fixed slots.
	/// </summary>
	/// <param name="name">The variable name.</param>
	/// <param name="kind">One of <c>i</c>, <c>f</c> or <c>b</c>; otherwise <c>'\0'</c>.</param>
	/// <param name="index">The slot index; otherwise <c>-1</c>.</param>
	/// <returns><see langword="true"/> when <paramref name="name"/> is a fixed slot.</returns>
	private static bool TryParseFixed(string name, out char kind, out int index)
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
