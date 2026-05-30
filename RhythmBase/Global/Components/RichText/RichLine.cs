using System.Diagnostics;
using System.Text;

namespace RhythmBase.Global.Components.RichText;

/// <summary>
/// Represents a list of rich text strings.
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public struct RichLine<TStyle>()
	 : IRichTextLine<TStyle>
	where TStyle : IRichStringStyle<TStyle>, new()
{
	/// <summary>
	/// Gets or sets the list of rich text strings.
	/// </summary>
	private Phrase<TStyle>[] texts = [];
	/// <summary>
	/// The length of the string.
	/// </summary>
	public readonly int Length => texts.Sum(i => i.Length);
#if NETCOREAPP3_0_OR_GREATER
	/// <summary>
	/// Gets or sets the <see cref="RichLine{RDPhraseStyle}"/> at the specified index.
	/// </summary>
	/// <param name="index">The index of the <see cref="RichLine{RDPhraseStyle}"/> to get or set.</param>
	/// <returns>The <see cref="RichLine{RDPhraseStyle}"/> at the specified index.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the index is out of range.</exception>
	public RichLine<TStyle> this[Index index]
	{
		get
		{
			int i = index.GetOffset(Length);
			if (Length <= i)
				throw new ArgumentOutOfRangeException(nameof(index));
			int ti = 0;
			while (texts[ti].Length < i)
			{
				i -= texts[ti].Length;
				ti++;
			}
			RichLine<TStyle> line = new()
			{
				texts = [texts[ti][i]]
			};
			return line;
		}
		set
		{
			int i = index.GetOffset(Length);
			if (Length <= i)
				throw new ArgumentOutOfRangeException(nameof(index));
			texts = Concat([this[..i], value, this[(i + 1)..]]).texts;
		}
	}
	/// <summary>
	/// Gets or sets the <see cref="RichLine{RDPhraseStyle}"/> within the specified range.
	/// </summary>
	/// <param name="range">The range of the <see cref="RichLine{RDPhraseStyle}"/> to get or set.</param>
	/// <returns>The <see cref="RichLine{RDPhraseStyle}"/> within the specified range.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the range is out of bounds.</exception>
	public RichLine<TStyle> this[Range range]
	{
		get
		{
			int start = range.Start.GetOffset(Length);
			int end = range.End.GetOffset(Length);
			if (!(start <= end && end <= Length))
				throw new ArgumentOutOfRangeException(nameof(range));
			int ti = 0, tstart, tend;
			Phrase<TStyle>[] strings = [];
			while (texts[ti].Length <= start)
			{
				start -= texts[ti].Length;
				end -= texts[ti].Length;
				ti++;
			}
			tstart = ti;
			while (texts[ti].Length < end)
			{
				end -= texts[ti].Length;
				ti++;
			}
			tend = ti;
			if (tstart == tend)
				strings = [texts[tstart][start..end]];
			else
			{
				for (int i = tstart + 1; i < tend; i++)
					strings = [.. strings, texts[i]];
				strings = [texts[tstart][start..], .. strings, texts[tend][..end]];
			}
			RichLine<TStyle> line = new()
			{
				texts = strings
			};
			return line;
		}
		set
		{
			int start = range.Start.GetOffset(Length);
			int end = range.End.GetOffset(Length);
			if (!(start < end && end <= Length))
				throw new ArgumentOutOfRangeException(nameof(range));
			texts = Concat([this[..start], value, this[end..]]).texts;
		}
	}
#endif
	/// <summary>
	/// Gets the default <see cref="Phrase{RDPhraseStyle}"/> instance.
	/// </summary>
	public static readonly RichLine<TStyle> Empty = new();
	/// <inheritdoc/>
	public
#if NET7_0_OR_GREATER
		static
#endif
		RichLine<TStyle> Concat(params RichLine<TStyle>[] lines)
	{
		if (lines.Length == 0)
			return Empty;
		int total = lines.Sum(i => i.texts.Length);

		Phrase<TStyle>[] parts = new Phrase<TStyle>[total];
		int idx = 0;

		Phrase<TStyle>[] first = lines[0].texts;
		foreach (var text in first)
		{
			if (text.Length == 0)
				continue;
			parts[idx++] = text;
		}
		for (int il = 1; il < lines.Length; il++)
		{
			int it = 0;
			Phrase<TStyle>[] txts = lines[il].texts;
			if (idx > 0 && txts.Length > 0 && parts[idx - 1].Style.Equals(txts[0].Style))
			{
				Phrase<TStyle> before = parts[idx - 1], after = txts[0];
				Phrase<TStyle> richString = new(before.Text + after.Text)
				{
					Style = before.Style,
					Events = [.. before.Events, .. after.Events.Select(i => new DialogueTone(i.Type, i.Index + before.Length) { Pause = i.Pause })]
				};
				parts[idx - 1] = richString;
				it++;
			}
			for (; it < txts.Length; it++)
			{
				if (txts[it].Length == 0)
					continue;
				parts[idx++] = txts[it];
			}
		}
		Array.Resize(ref parts, idx);
		return new() { texts = parts };
	}
	/// <summary>
	/// Implicitly converts a <see cref="Phrase{RDPhraseStyle}"/> to a <see cref="RichLine{RDPhraseStyle}"/>.
	/// </summary>
	/// <param name="text">The <see cref="Phrase{RDPhraseStyle}"/> to convert.</param>
	/// <returns>A new <see cref="RichLine{RDPhraseStyle}"/> containing the specified <see cref="Phrase{RDPhraseStyle}"/>.</returns>
	public static implicit operator RichLine<TStyle>(Phrase<TStyle> text) => new() { texts = [text] };
	/// <summary>
	/// Implicitly converts a <see cref="Phrase{RDPhraseStyle}"/> to a <see cref="RichLine{RDPhraseStyle}"/>.
	/// </summary>
	/// <param name="texts">The <see cref="Phrase{RDPhraseStyle}"/> to convert.</param>
	/// <returns>A new <see cref="RichLine{RDPhraseStyle}"/> containing the specified <see cref="Phrase{RDPhraseStyle}"/>.</returns>
	public static implicit operator RichLine<TStyle>(Phrase<TStyle>[] texts) => new() { texts = texts };
	/// <summary>
	/// Implicitly converts a <see cref="string"/> to a <see cref="RichLine{RDPhraseStyle}"/>.
	/// </summary>
	/// <param name="text">The <see cref="string"/> to convert.</param>
	/// <returns>A new <see cref="RichLine{RDPhraseStyle}"/> containing the specified <see cref="string"/>.</returns>
	public static implicit operator RichLine<TStyle>(string text) => new() { texts = [new Phrase<TStyle>(text)] };
	/// <summary>
	/// Converts the specified <see cref="RichLine{TStyle}"/> instance to its string representation.
	/// </summary>
	public static explicit operator string(RichLine<TStyle> line) => line.ToString();
	/// <summary>
	/// Deserializes a string into an <see cref="RichLine{RDPhraseStyle}"/>.
	/// </summary>
	/// <param name="text">The string to deserialize.</param>
	/// <returns>A new <see cref="RichLine{RDPhraseStyle}"/> containing the deserialized content.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the input text is null.</exception>
	/// <exception cref="FormatException">Thrown when the input text has an invalid format.</exception>

#if NET7_0_OR_GREATER
	static
#endif
	public RichLine<TStyle> Deserialize(string text)
	{
		if (string.IsNullOrEmpty(text))
			return new() { texts = [] };
		RichLine<TStyle> line = "";
		TStyle style = new();
		int pl = 0;
		int p = text.IndexOf('<');
		if (p == -1)
			return DeserializeStringPart(text, style);
		while (p < text.Length)
		{
			string stringPart;
			int tagend = text.IndexOf('>', p);
			if (tagend == -1)
			{
				stringPart = text[pl..];
				line += DeserializeStringPart(stringPart, style);
				return line;
			}
			else
				stringPart = text[pl..p];
			if (stringPart.Length > 0)
				line += DeserializeStringPart(stringPart, style);
			string tagContent = text[(p + 1)..tagend];
			if (tagContent.StartsWith('/') && style.ResetProperty(tagContent.AsSpan()[1..])) { }
			else
			{
				string[] parts = tagContent.Split('=', 2);
				if (style.SetProperty(parts[0], parts.Length == 2 ? parts[1] : "true")) { }
				else
				{
					//line += DeserializeStringPart(text.Substring(p, tagend + 1 - p), style);
				}
			}
			pl = tagend + 1;
			p = text.IndexOf('<', pl);
			if (p == -1)
				return line + DeserializeStringPart(text[pl..], style);
		}
		return line;
	}
	private static void ParseAttributes(ReadOnlySpan<char> tag, ref TStyle style)
	{
		int pos = 0;
		int eq = tag[pos..].IndexOf('=');
		ReadOnlySpan<char> key, val;
		if (eq == -1)
		{
			key = tag[pos..].Trim();
			val = [];
			_ = tag.Length;
		}
		else
		{
			eq += pos;
			key = tag[pos..eq].Trim();
			int valStart = eq + 1;
			val = tag[valStart..].Trim();
			_ = tag.Length;
		}
		style.SetProperty(key, val);
	}
	private static Phrase<TStyle> DeserializeStringPart(string text, TStyle style)
	{
		if (!
#if NET7_0_OR_GREATER
			TStyle
#else
			style
#endif
			.HasPhrase)
			return new Phrase<TStyle>(text) { Style = style };
		int pstart = 0;
		DialogueTone[] events = [];
		while (pstart < text.Length)
		{
			int pend = text.IndexOf('[', pstart);
			if (pend == -1)
			{
				break;
			}
			int pstart2 = text.IndexOf(']', pend);
			int pend2 = text.IndexOf('[', pend + 1);
			if (pstart2 == -1)
				break;
			if (pend2 != -1 && pend2 < pstart2)
			{
				pstart = pend2 + 1;
				continue;
			}
			string btag = text[(pend + 1)..pstart2];
			if (DialogueTone.Create(btag, pend, out DialogueTone? e) && e is DialogueTone ei)
				events = [.. events, ei];
			text = text[..pend] + text[(pstart2 + 1)..];
		}
		return new Phrase<TStyle>(text) { Style = style, Events = events };
	}
	/// <summary>
	/// Serializes the current <see cref="RichLine{RDPhraseStyle}"/> instance to a string.
	/// </summary>
	/// <returns>A string representation of the current <see cref="RichLine{RDPhraseStyle}"/> instance.</returns>
	/// <remarks>
	/// The serialization process converts the rich text line into a string format, including any styling information.
	/// </remarks>
	public readonly string Serialize()
	{
		StringBuilder sb = new();
		TStyle style = new();
		int ci = 0;
		foreach (Phrase<TStyle> str in texts)
		{
			sb.Append(
#if NET7_0_OR_GREATER
				TStyle
#else
				str.Style
#endif
				.GetXmlTag(style, str.Style));
			string part = str.Text;
			int offset = 0;
			foreach (DialogueTone e in str.Events)
			{
				string serialized = e.Serialize();
				part = part.Insert(e.Index + offset, serialized);
				offset += serialized.Length;
			}
			sb.Append(part);
			ci += str.Length;
			style = str.Style;
		}
		sb.Append(
#if NET7_0_OR_GREATER
			TStyle
#else
							style
#endif
.GetXmlTag(style, new()));
		return sb.ToString();
	}
	/// <summary>
	/// Concatenates two <see cref="RichLine{RDPhraseStyle}"/> instances.
	/// </summary>
	/// <param name="left">The left <see cref="RichLine{RDPhraseStyle}"/>.</param>
	/// <param name="right">The right <see cref="RichLine{RDPhraseStyle}"/>.</param>
	/// <returns>A new <see cref="RichLine{RDPhraseStyle}"/> that is the result of concatenating the two specified instances.</returns>
	public static RichLine<TStyle> operator +(RichLine<TStyle> left, RichLine<TStyle> right) =>
#if NETSTANDARD2_0
		Empty.
#endif
		Concat([.. left.texts, .. right.texts]);
	/// <inheritdoc/>
	public readonly override string ToString() => string.Join("", texts);
	/// <inheritdoc/>
	private readonly string GetDebuggerDisplay() => ToString();
}
