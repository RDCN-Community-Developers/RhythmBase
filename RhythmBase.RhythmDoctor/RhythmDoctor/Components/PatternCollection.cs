using System.Collections;
using System.Text;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Represents a collection of pattern elements, providing indexed access and conversion between arrays, strings, and
/// pattern collections.
/// </summary>
/// <remarks>PatternCollection enables flexible initialization and conversion between different representations
/// of patterns, including arrays of Patterns and string-based pattern codes. Implicit conversion operators allow for
/// concise and readable code when working with pattern data. Each pattern is mapped to a specific character for string
/// representations, facilitating serialization and parsing scenarios.</remarks>
public struct PatternCollection : IEnumerable<Pattern>
{
	private Pattern[] _ps;
	/// <summary>
	/// Gets the number of elements contained in the collection.
	/// </summary>
	public readonly int Length => _ps.Length;
	/// <summary>
	/// Gets or sets the pattern at the specified index within the collection.
	/// </summary>
	/// <param name="index">The zero-based index of the pattern to get or set. Must be between 0 and 5, inclusive.</param>
	/// <returns>The pattern located at the specified index.</returns>
	/// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="index"/> is less than 0 or greater than 5.</exception>
	public readonly Pattern this[int index]
	{
		get => index is < 0 or > 5 ? throw new IndexOutOfRangeException("Index must be between 0 and 5.") : _ps[index];
		set
		{
			if (index is < 0 or > 5)
				throw new IndexOutOfRangeException("Index must be between 0 and 5.");
			_ps[index] = value;
		}
	}
	/// <summary>
	/// Initializes a new instance of the PatternCollection class with the specified patterns.
	/// </summary>
	/// <param name="pts">An array of Patterns objects to include in the collection. Cannot be null.</param>
	public PatternCollection(params Pattern[] pts) => _ps = pts;
	/// <summary>
	/// Initializes a new instance of the PatternCollection class by parsing a string of pattern characters.
	/// </summary>
	/// <remarks>Each character in the input string is mapped to a specific pattern. The order of characters
	/// determines the order of patterns in the collection.</remarks>
	/// <param name="pts">A string where each character represents a pattern to include in the collection. Valid characters are '-', 'x',
	/// 'u', 'd', 'b', and 'r'.</param>
	/// <exception cref="NotImplementedException">Thrown if the input string contains a character that does not correspond to a supported pattern.</exception>
	public PatternCollection(string pts)
	{
		_ps = new Pattern[pts.Length];
		for (int i = 0; i < pts.Length; i++)
		{
			_ps[i] = pts[i] switch
			{
				'-' => Pattern.None,
				'x' => Pattern.X,
				'u' => Pattern.Up,
				'd' => Pattern.Down,
				'b' => Pattern.Banana,
				'r' => Pattern.ReturnBanana,
				_ => throw new NotImplementedException(),
			};
		}
	}
	/// <summary>
	/// Default pattern.
	/// </summary>
	public static PatternCollection Default => "------";
	/// <summary>
	/// Converts a PatternCollection instance to an array of Patterns objects.
	/// </summary>
	/// <param name="pc">The PatternCollection instance to convert.</param>
	public static implicit operator Pattern[](PatternCollection pc) => pc._ps;
	/// <summary>
	/// Defines an implicit conversion from an array of Patterns to a PatternCollection.
	/// </summary>
	/// <remarks>This operator enables passing a Patterns array where a PatternCollection is expected, allowing
	/// for more concise and readable code.</remarks>
	/// <param name="ps">An array of Patterns to convert to a PatternCollection. Cannot be null.</param>
	public static implicit operator PatternCollection(Pattern[] ps) => new(ps);
	/// <summary>
	/// Converts a PatternCollection instance to its string representation using pattern-specific characters.
	/// </summary>
	/// <remarks>Each pattern in the collection is represented by a specific character in the resulting string.
	/// The mapping is as follows: '-' for None, 'x' for X, 'u' for Up, 'd' for Down, 'b' for Banana, and 'r' for
	/// Return.</remarks>
	/// <param name="pc">The PatternCollection instance to convert to a string.</param>
	public static implicit operator string(PatternCollection pc)
	{
		StringBuilder sb = new();
		for (int i = 0; i < pc._ps.Length; i++)
		{
			sb.Append(pc._ps[i] switch
			{
				Pattern.None => '-',
				Pattern.X => 'x',
				Pattern.Up => 'u',
				Pattern.Down => 'd',
				Pattern.Banana => 'b',
				Pattern.ReturnBanana => 'r',
				_ => throw new NotImplementedException(),
			});
		}
		return sb.ToString();
	}
	/// <summary>
	/// Defines an implicit conversion from a string to a PatternCollection, mapping each character in the string to a
	/// corresponding pattern.
	/// </summary>
	/// <remarks>If the string contains a character that does not correspond to a known pattern, an exception is
	/// thrown. This operator enables concise initialization of PatternCollection instances from string
	/// representations.</remarks>
	/// <param name="s">The string to convert. Each character represents a pattern: '-' for None, 'x' for X, 'u' for Up, 'd' for Down, 'b'
	/// for Banana, and 'r' for Return.</param>
	public static implicit operator PatternCollection(string s)
	{
		PatternCollection pc = new()
		{
			_ps = new Pattern[s.Length]
		};
		for (int i = 0; i < s.Length; i++)
		{
			pc._ps[i] = s[i] switch
			{
				'-' => Pattern.None,
				'x' => Pattern.X,
				'u' => Pattern.Up,
				'd' => Pattern.Down,
				'b' => Pattern.Banana,
				'r' => Pattern.ReturnBanana,
				_ => throw new NotImplementedException(),
			};
		}
		return pc;
	}
	/// <summary>
	/// Returns an enumerator that iterates through the collection of patterns.
	/// </summary>
	/// <returns>An enumerator that can be used to iterate through the collection of patterns.</returns>
	public readonly IEnumerator<Pattern> GetEnumerator()
	{
		foreach (Pattern p in _ps)
			yield return p;
	}
	readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
