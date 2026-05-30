using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace RhythmBase.Global.Components.RichText;

/// <summary>
/// Defines the interface for rich string styles.
/// </summary>
/// <typeparam name="TSelf">The type that implements this interface.</typeparam>
public interface IRichStringStyle<TSelf> :
#if NET7_0_OR_GREATER
	IEqualityOperators<TSelf, TSelf, bool>, 
#endif
	IEquatable<TSelf>
where TSelf : IRichStringStyle<TSelf>
{
	/// <summary>
	/// Determines whether the specified object is equal to the current object.
	/// </summary>
	/// <param name="obj">The object to compare with the current object.</param>
	/// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
#if NETSTANDARD
	bool Equals(object? obj);
#else
	bool Equals([NotNullWhen(true)] object? obj) => obj is TSelf e && Equals(e);
#endif
#if !NETSTANDARD
	/// <summary>
	/// Gets the closing tag for the specified name.
	/// </summary>
	/// <param name="name">The name of the tag.</param>
	/// <returns>The closing tag for the specified name.</returns>
	static string GetCloseTag(string name) => $"</{name}>";
	/// <summary>
	/// Gets the opening tag for the specified name and optional argument.
	/// </summary>
	/// <param name="name">The name of the tag.</param>
	/// <param name="arg">The optional argument for the tag.</param>
	/// <returns>The opening tag for the specified name and optional argument.</returns>
	static string GetOpenTag(string name, string? arg = null) => arg is null ? $"<{name}>" : $"<{name}={arg}>";
#endif
	/// <summary>
	/// Generates an XML tag representing the differences between two <see cref="DialoguePhraseStyle"/> instances.
	/// </summary>
	/// <param name="before">The initial <see cref="DialoguePhraseStyle"/> instance.</param>
	/// <param name="after">The modified <see cref="DialoguePhraseStyle"/> instance.</param>
	/// <returns>A string containing the XML tag that represents the differences between the two instances.</returns>
#if NET7_0_OR_GREATER
	static
#endif
	abstract string GetXmlTag(TSelf before, TSelf after);
	/// <summary>
	/// Resets the property of the rich string style based on the provided name.
	/// </summary>
	/// <param name="name">The name of the property to reset.</param>
	/// <returns>True if the property was successfully reset; otherwise, false.</returns>
	internal bool ResetProperty(ReadOnlySpan<char> name);
	/// <summary>
	/// Sets the property of the rich string style based on the provided name and value.
	/// </summary>
	/// <param name="name">The name of the property to set.</param>
	/// <param name="value">The value to set for the property.</param>
	/// <returns>True if the property was successfully set; otherwise, false.</returns>
	internal bool SetProperty(ReadOnlySpan<char> name, ReadOnlySpan<char> value);
	/// <summary>
	/// Gets a value indicating whether the style has a phrase.
	/// </summary>

#if NET7_0_OR_GREATER
	static
#endif
	abstract bool HasPhrase { get; }
}
