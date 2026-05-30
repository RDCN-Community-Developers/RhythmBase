using System.Text;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// The conditions of the event.
/// </summary>
public class Condition
{
	private readonly HashSet<uint> conditions = [];
	/// <summary>
	/// The time of effectiveness of the condition.
	/// </summary>
	public float Duration { get; set; }
	/// <summary>
	/// Gets a value indicating whether the collection contains any conditions.
	/// </summary>
	public bool HasValue => conditions.Count > 0;
	/// <summary>
	/// Gets an array of indices derived from the current set of conditions.
	/// </summary>
	public int[] Indices => [.. conditions.Select(c => (int)(0x3FFFFFFF & c))];
	/// <summary>
	/// Gets or sets the enabled or disabled state associated with the specified condition index.
	/// </summary>
	/// <remarks>Setting the value to <see langword="null"/> removes any explicit enabled or disabled state for
	/// the specified index.</remarks>
	/// <param name="index">The zero-based index of the condition to retrieve or modify.</param>
	/// <returns>A nullable Boolean value indicating the state of the condition at the specified index: <see langword="true"/> if
	/// enabled; <see langword="false"/> if disabled; or <see langword="null"/> if the state is not set.</returns>
	public bool? this[int index]
	{
		get
		{
			uint d = (uint)(index) & 0x3FFFFFFF;
			uint e = d | 0x80000000;
			if (conditions.Contains(e))
				return true;
			else return conditions.Contains(d) ? false : null;
		}
		set
		{
			uint d = (uint)(index) & 0x3FFFFFFF;
			uint e = d | 0x80000000;
			if (value is bool v)
				if (v)
				{
					conditions.Remove(d);
					conditions.Add(e);
				}
				else
				{
					conditions.Remove(e);
					conditions.Add(d);
				}
			else
			{
				conditions.Remove(d);
				conditions.Remove(e);
			}
		}
	}
	/// <summary>
	/// Gets or sets the condition associated with the specified character key.
	/// </summary>
	/// <remarks>Setting the value to <see langword="true"/> marks the condition as active for the specified key;
	/// setting it to <see langword="false"/> marks it as inactive. Assigning <see langword="null"/> removes the condition
	/// for the key. This indexer enables efficient management of conditions using character keys.</remarks>
	/// <param name="key">The character key used to retrieve or set the condition. The key must be a valid character within the defined
	/// range.</param>
	/// <returns>Returns <see langword="true"/> if the condition is active, <see langword="false"/> if it is inactive, or <see
	/// langword="null"/> if the condition is not set for the specified key.</returns>
	public bool? this[char key]
	{
		get
		{
			uint d = (uint)(key) & 0x3FFFFFFF;
			uint e = d | 0x80000000;
			if (conditions.Contains(e))
				return true;
			else return conditions.Contains(d) ? false : null;
		}
		set
		{
			uint d = ((uint)(key) & 0x3FFFFFFF) | 0x40000000;
			uint e = d | 0x80000000;
			if (value is bool v)
				if (v)
				{
					conditions.Remove(d);
					conditions.Add(e);
				}
				else
				{
					conditions.Remove(e);
					conditions.Add(d);
				}
			else
			{
				conditions.Remove(d);
				conditions.Remove(e);
			}
		}
	}
	/// <summary>
	/// Initializes a new instance of the <see cref="Condition"/> class.
	/// </summary>
	public Condition()
	{
	}
	/// <summary>
	/// Loads a condition from a string.
	/// </summary>
	/// <param name="text">The text to load the condition from.</param>
	/// <returns>A new instance of the <see cref="Condition"/> class.</returns>
	/// <exception cref="RhythmBaseException">Thrown when the condition is illegal.</exception>
	public static Condition Deserialize(string text)
	{
		// "p&f&~n&~o&1&~2&3d4.5"
		int i = 0;
		Condition o = new();
		while (i < text.Length && text[i] != 'd')
		{
			bool enabled = true;
			int index = 0;
			char globalIndex = '\0';
			char c = text[i];
			if (text[i] is '~')
			{
				enabled = false;
				++i;
				c = text[i];
			}
			if (char.IsDigit(text[i]))
			{
				while (i < text.Length && char.IsDigit(text[i]))
					index = index * 10 + (text[i++] - '0');
			}
			else
			{
				globalIndex = c switch
				{
					'p' or 'f' or 'n' or 'o' => c,
					_ => throw new RhythmBaseException($"Illegal condition: {text}."),
				};
				++i;
			}
			//while (i < text.Length && char.IsDigit(text[i]))
			//	index = index * 10 + (text[i++] - '0');
			if (text[i] is '&')
			{
				if (globalIndex == '\0')
					o[index] = enabled;
				else
					o[globalIndex] = enabled;
				++i;
				continue;
			}
			if (text[i] is 'd')
			{
				o[index] = enabled;
				break;
			}
			throw new RhythmBaseException($"Illegal condition: {text}.");
		}
		float duration = 0;
		i++;
		while (i < text.Length && char.IsDigit(text[i]))
			duration = duration * 10 + (text[i++] - '0');
		if (i < text.Length && text[i] is '.')
		{
			++i;
			float frac = 0.1f;
			while (i < text.Length && char.IsDigit(text[i]))
			{
				duration += frac * (text[i++] - '0');
				frac *= 0.1f;
			}
		}
		o.Duration = duration;
		return o;
	}
	/// <summary>
	/// Converts conditions to a string.
	/// </summary>
	/// <returns>A string in the format supported by RDLevel.</returns>
	public string Serialize()
	{
		StringBuilder sb = new();
		foreach (uint i in conditions)
		{
			bool isChar = (i & 0x40000000) > 0;
			bool isEnabled = (i & 0x80000000) > 0;
			int index = (int)(i & 0x3FFFFFFF);
			if (sb.Length > 0)
				sb.Append('&');
			if (!isEnabled)
				sb.Append('~');
			if (isChar)
				sb.Append((char)index);
			else
				sb.Append(index);
		}
		sb.Append('d').Append(Duration.ToString("0.########"));
		return sb.ToString();
	}
	/// <summary>
	/// Creates a deep copy of the current <see cref="Condition"/> instance.
	/// </summary>
	/// <returns>A new <see cref="Condition"/> object that is a deep copy of the current instance.</returns>
	public Condition Clone()
	{
		return Deserialize(Serialize());
	}
	/// <summary>
	/// Converts a <see cref="Condition"/> instance to its string representation.
	/// </summary>
	/// <remarks>This operator enables implicit conversion of a <see cref="Condition"/> object to a string,
	/// typically for serialization or display purposes.</remarks>
	/// <param name="c">The <see cref="Condition"/> instance to convert.</param>
	public static implicit operator string(Condition c) => c.Serialize();
	/// <inheritdoc/>
	public override string ToString() => Serialize();
}
