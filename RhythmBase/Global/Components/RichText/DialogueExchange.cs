using System.Text.Json.Serialization;

namespace RhythmBase.Global.Components.RichText;

/// <summary>
/// Represents a list of dialogue lines.
/// </summary>
[System.Text.Json.Serialization.JsonConverter(typeof(DialogueBlock))]
public class DialogueExchange : List<DialogueBlock>
{
	/// <summary>
	/// Serializes the dialogue list to a string.
	/// </summary>
	/// <returns>A string representation of the dialogue list.</returns>
	public string Serialize() => string.Join(Environment.NewLine, this.Select(i => i.Serialize()));
	/// <summary>
	/// Deserializes a string into a <see cref="DialogueExchange"/> instance.
	/// </summary>
	/// <param name="text">The string to deserialize.</param>
	/// <returns>A new <see cref="DialogueExchange"/> containing the deserialized dialogue lines.</returns>
	public static DialogueExchange Deserialize(string text) => [.. text.Split(['\r','\n'], StringSplitOptions.RemoveEmptyEntries).Select(DialogueBlock.Deserialize)];
	/// <summary>
	/// Deserializes a string into a <see cref="DialogueExchange"/> instance, using a lookup of valid expressions.
	/// </summary>
	/// <param name="text">The string to deserialize.</param>
	/// <param name="expressions">A lookup of valid expressions for each character.</param>
	/// <returns>A new <see cref="DialogueExchange"/> containing the deserialized dialogue lines.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the input string is null.</exception>
	/// <exception cref="FormatException">Thrown when the input string has an invalid format.</exception>
	public static DialogueExchange Deserialize(string text, ILookup<string, string> expressions) => [.. text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(i => DialogueBlock.Deserialize(i, expressions))];
	///<inheritdoc/>
	public override string ToString() => string.Join(Environment.NewLine, this.Select(i => i.ToString()));
}