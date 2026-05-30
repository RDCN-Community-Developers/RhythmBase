using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace RhythmBase.Global.Components.RichText;

/// <summary>
/// Represents a rich text string with various styling options.
/// </summary>
/// <param name="text">The text content of the rich string.</param>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public struct Phrase<TStyle>(string text) :
#if NET7_0_OR_GREATER
	IEqualityOperators<Phrase<TStyle>, Phrase<TStyle>, bool>,
#endif
		IEquatable<Phrase<TStyle>>
	where TStyle : IRichStringStyle<TStyle>, new()
{
	/// <summary>
	/// Gets the text content of the rich string.
	/// </summary>
	public string Text { get; internal set; } = text;
	/// <summary>
	/// Gets or sets the events associated with the rich string.
	/// </summary>
	public DialogueTone[] Events { get; set; } = [];
	/// <summary>
	/// Gets the length of the text content.
	/// </summary>
	/// <value>The number of characters in the text content.</value>
	public readonly int Length => Text.Length;
#if NETCOREAPP3_0_OR_GREATER
	/// <summary>
	/// Gets the rich string at the specified index.
	/// </summary>
	/// <param name="index">The index of the character.</param>
	/// <returns>A new <see cref="Phrase{TStyle}"/> with the character at the specified index.</returns>
	public Phrase<TStyle> this[Index index] =>
		new()
		{
			Text = Text[index].ToString(),
			Style = Style,
			Events = GetEvents(index.GetOffset(Length), 1)
		};

	/// <summary>
	/// Gets the rich string within the specified range.
	/// </summary>
	/// <param name="range">The range of characters.</param>
	/// <returns>A new <see cref="Phrase{TStyle}"/> with the characters within the specified range.</returns>
	public Phrase<TStyle> this[Range range]
	{
		get
		{
			Phrase<TStyle> style = new()
			{
				Text = Text[range],
				Style = Style,
				Events = GetEvents(range.Start.GetOffset(Length), range.End.GetOffset(Length) - range.Start.GetOffset(Length))
			};
			return style;
		}
	}
#endif
	private readonly DialogueTone[] GetEvents(int start, int length) => [.. Events
		.Where(e => e.Index >= start && e.Index < start + length)
		.Select(e => new DialogueTone(e.Type, e.Index - start))];
	/// <summary>
	/// Gets a new <see cref="Phrase{TStyle}"/> with the same style as the current instance.
	/// </summary>
	public TStyle Style { get; init; } = new();
	/// <summary>
	/// Initializes a new instance of the <see cref="Phrase{TStyle}"/> struct with an empty text.
	/// </summary>
	public Phrase() : this("") { }
	/// <summary>
	/// Implicitly converts a string to an <see cref="Phrase{TStyle}"/>.
	/// </summary>
	/// <param name="text">The text to convert.</param>
	/// <returns>A new <see cref="Phrase{TStyle}"/> instance with the specified text.</returns>
	public static implicit operator Phrase<TStyle>(string text) => new() { Text = text };
	/// <inheritdoc/>
	public static bool operator ==(Phrase<TStyle> left, Phrase<TStyle> right) => left.Text == right.Text && left.Style.Equals(right.Style);
	/// <inheritdoc/>
	public static bool operator !=(Phrase<TStyle> left, Phrase<TStyle> right) => !(left == right);
	/// <inheritdoc/>
	public readonly bool Equals(Phrase<TStyle> other) => this == other;
	/// <inheritdoc/>
	public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is Phrase<TStyle> && base.Equals(obj);
	/// <inheritdoc/>
	public readonly override int GetHashCode() => Text.GetHashCode();
	/// <inheritdoc/>
	public readonly override string ToString() => Text;
	/// <inheritdoc/>
	private readonly string GetDebuggerDisplay() => ToString();
}
