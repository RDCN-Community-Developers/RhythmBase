using System.Text;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// The conditions of the event.
/// </summary>
public class Condition
{
	private const int ulongSize = sizeof(ulong) * 8;
	private ulong[]? _index_conditions;
	private ulong[]? _char_conditions;
	/// <summary>
	/// The time of effectiveness of the condition.
	/// </summary>
	public float Duration { get; set; }
	/// <summary>
	/// Gets a value indicating whether the collection contains any conditions.
	/// </summary>
	public bool IsEmpty => (_index_conditions is null || _index_conditions.All(c => c == 0)) && (_char_conditions is null || _char_conditions.All(c => c == 0));
	private static bool? GetValue(ulong[]? conditions, int index)
	{
		if (conditions == null) return null;
		int ulongIndex = index * 2 / ulongSize;
		int bitIndex = index * 2 % ulongSize;
		if (ulongIndex >= conditions.Length)
			return null;
		ulong value = (conditions[ulongIndex] >> bitIndex) & 0b11;
		if ((value & 0b10) != 0)
			return (value & 0b01) != 0;
		else
			return null;
	}
	private static void SetValue(ref ulong[]? conditions, int index, bool? value)
	{
		int ulongIndex = index * 2 / ulongSize;
		int bitIndex = index * 2 % ulongSize;
		if (conditions == null)
			conditions = new ulong[ulongIndex + 1];
		else if (ulongIndex >= conditions.Length)
			Array.Resize(ref conditions, ulongIndex + 1);
		ulong mask = 0b11UL << bitIndex;
		ulong newValue = value switch
		{
			true => 0b11UL,
			false => 0b10UL,
			null => 0b00UL
		} << bitIndex;
		conditions[ulongIndex] = (conditions[ulongIndex] & ~mask) | newValue;
	}
	internal void SetIndexValueToNull(int index)
	{
		int ulongIndex = index * 2 / ulongSize;
		int bitIndex = index * 2 % ulongSize;
		if (_index_conditions == null || ulongIndex >= _index_conditions.Length)
			return;
		ulong mask = 0b11UL << bitIndex;
		_index_conditions[ulongIndex] &= ~mask;
	}
	/// <summary>
	/// Creates a deep copy of the current <see cref="Condition"/> instance.
	/// </summary>
	/// <returns>A new <see cref="Condition"/> object that is a deep copy of the current instance.</returns>
	public Condition Clone()
	{
		return new Condition
		{
			Duration = this.Duration,
			_index_conditions = _index_conditions is null ? null : (ulong[]?)this._index_conditions?.Clone(),
			_char_conditions = _char_conditions is null ? null : (ulong[]?)this._char_conditions?.Clone()
		};
	}
	/// <summary>
	/// Clears all conditions and resets the duration to zero.
	/// </summary>
	public void Clear()
	{
		_index_conditions = null;
		_char_conditions = null;
		Duration = 0;
	}
	internal void Remap(int[] remap, int trailingEmptyIndex)
	{
		if (_index_conditions is not null)
		{
			ulong[] newIndexConditions = new ulong[trailingEmptyIndex * 2 / ulongSize + 1];
			for (int i = 0; i < remap.Length; i++)
			{
				if (remap[i] == -1) continue;
				int oldIndex = i;
				bool? value = GetValue(_index_conditions, oldIndex);
				SetValue(ref newIndexConditions, remap[i], value);
			}
			_index_conditions = newIndexConditions;
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
			int index = (int)key;
			return GetValue(_char_conditions, index);
		}
		set
		{
			int index = (int)key;
			SetValue(ref _char_conditions, index, value);
		}
	}
	/// <summary>
	/// Gets or sets the enabled or disabled state associated with the specified condition index.
	/// </summary>
	/// <param name="conditional">The base conditional for which to get or set the state.</param>
	/// <returns>The enabled or disabled state of the condition.</returns>
	public bool? this[BaseConditional conditional]
	{
		get
		{
			int index = conditional.ParentCollection?.DataIndexOf(conditional) ?? -1;
			return GetValue(_index_conditions, index);
		}
		set
		{
			int index = conditional.ParentCollection?.DataIndexOf(conditional) ?? -1;
			SetValue(ref _index_conditions, index, value);
		}
	}
	/// <summary>
	/// Loads a condition from a string.
	/// </summary>
	/// <param name="text">The text to load the condition from.</param>
	/// <returns>A new instance of the <see cref="Condition"/> class.</returns>
	/// <exception cref="FormatException">Thrown when the condition is illegal.</exception>
	public static Condition Deserialize(string text)
	{
		Condition o = new();
		int i = 0;
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
				globalIndex = c;
				++i;
			}
			if (text[i] is '&')
			{
				if (globalIndex == '\0')
					SetValue(ref o._index_conditions, index, enabled);
				else
					SetValue(ref o._char_conditions, globalIndex, enabled);
				++i;
				continue;
			}
			if (text[i] is 'd')
			{
				if (globalIndex == '\0')
					SetValue(ref o._index_conditions, index, enabled);
				else
					SetValue(ref o._char_conditions, globalIndex, enabled);
				break;
			}
			throw new FormatException($"Illegal condition: {text}.");
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
		bool isFirst = true;
		if (_index_conditions is not null)
		{
			for (int u = 0; u < _index_conditions.Length; u++)
			{
				ulong conditionals = _index_conditions[u];
				for (int i = 0; i < ulongSize; i += 2)
				{
					ulong value = (conditionals >> i) & 0b11;
					if (value == 0b00) continue;
					int index = u * (ulongSize / 2) + i / 2;
					bool isEnabled = (value & 0b01) != 0;
					if (!isFirst)
						sb.Append('&');
					isFirst = false;
					if (!isEnabled) sb.Append('~');
					sb.Append(index);
				}
			}
		}
		if (_char_conditions is not null)
		{
			for (int u = 0; u < _char_conditions.Length; u++)
			{
				ulong conditionals = _char_conditions[u];
				for (int i = 0; i < ulongSize; i += 2)
				{
					ulong value = (conditionals >> i) & 0b11;
					if (value == 0b00) continue;
					int index = u * (ulongSize / 2) + i / 2;
					bool isEnabled = (value & 0b01) != 0;
					if (!isFirst)
						sb.Append('&');
					isFirst = false;
					if (!isEnabled) sb.Append('~');
					sb.Append((char)index);
				}
			}
		}
		sb.Append('d');
		sb.Append(Duration);
		return sb.ToString();
	}
	/// <inheritdoc/>
	public override string ToString() => Serialize();
}