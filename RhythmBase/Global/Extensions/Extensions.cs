using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace RhythmBase.Global.Extensions;

/// <summary>
/// Provides extension methods and utility methods for RhythmBase types.
/// </summary>
public static class Extensions
{
	extension(JsonException)
	{
		/// <summary>
		/// Throws a <see cref="JsonException"/> if the reader's current token type does not match any of the expected types.
		/// </summary>
		/// <param name="reader">The JSON reader to check.</param>
		/// <param name="expectedTokenType">The acceptable token types.</param>
		/// <returns>The current token type if it matches.</returns>
		[Conditional("DEBUG")]
		[DebuggerHidden]
		[StackTraceHidden]
		public static void ThrowIfNotMatch(ref Utf8JsonReader reader, params ReadOnlySpan<JsonTokenType> expectedTokenType)
		{
			ReadOnlyEnumCollection<JsonTokenType> types = new(expectedTokenType);
			if (types.Contains(reader.TokenType))
				return;
			byte[] result = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan.ToArray();
			string message = $"Expected token {string.Join(", ", types)} but got {reader.TokenType} {Encoding.UTF8.GetString(result)}, at byte position {reader.TokenStartIndex}.";
			throw new JsonException(message);
		}
	}
	/// <summary>
	/// Gets the read-only enum collection of event types for the specified event and target types.
	/// </summary>
	/// <typeparam name="TEvent">The event type. Must implement <see cref="IEvent{TType, TBeat}"/>.</typeparam>
	/// <typeparam name="TTarget">The target event type. Must implement <see cref="IEvent{TType, TBeat}"/>.</typeparam>
	/// <typeparam name="TType">The event type enum. Must be a struct and an enum.</typeparam>
	/// <typeparam name="TBeat">The beat type. Must be a struct and implement <see cref="ITickTime{TBeat}"/>.</typeparam>
	/// <returns>A <see cref="ReadOnlyEnumCollection{TType}"/> containing the event types.</returns>
	public static ReadOnlyEnumCollection<TType> TypesOf<TEvent, TTarget, TType, TBeat>()
			where TEvent : IEvent<TType, TBeat>
			where TTarget : IEvent<TType, TBeat>
			where TType : unmanaged, Enum
			where TBeat : struct, ITickTime<TBeat>
	{
		//if (typeof(TType) == typeof(RhythmDoctor.EventType))
		//    return RhythmDoctor.Utils.EventTypeUtils.ToEnums(typeof(TType)) as ReadOnlyEnumCollection<TType>? ?? [];
		//else if (typeof(TType) == typeof(Adofai.EventType))
		//    return Adofai.Utils.EventTypeUtils.ToEnums(typeof(TType)) as ReadOnlyEnumCollection<TType>? ?? [];
		//else if (typeof(TType) == typeof(BeatBlock.EventType))
		//    //return BeatBlock.Utils.EventTypeUtils.ToEnums(typeof(TType)) as ReadOnlyEnumCollection<TType>? ?? [];
		//    throw new NotImplementedException();
		//else
		throw new NotSupportedException($"Unsupported event type enum: {typeof(TType)}");
	}    /// <inheritdoc/>
	internal static string GetCloseTag(string name) => $"</{name}>";
	/// <inheritdoc/>
	internal static string GetOpenTag(string name, string? arg = null) => arg is null ? $"<{name}>" : $"<{name}={arg}>";
	/// <summary>
	/// Tries to add a tag to the specified string based on the provided name and boolean values.
	/// </summary>
	/// <param name="tag">The string to which the tag will be added.</param>
	/// <param name="name">The name of the tag.</param>
	/// <param name="before">A boolean value indicating whether the tag is before.</param>
	/// <param name="after">A boolean value indicating whether the tag is after.</param>
	internal static void TryAddTag(ref string tag, string name, bool before, bool after)
	{
		if (before != after)
			tag += after
			? GetOpenTag(name)
			: GetCloseTag(name);
	}
	/// <summary>
	/// Tries to add a tag to the specified string based on the provided name and optional string values.
	/// </summary>
	/// <param name="tag">The string to which the tag will be added.</param>
	/// <param name="name">The name of the tag.</param>
	/// <param name="before">An optional string value indicating the tag before.</param>
	/// <param name="after">An optional string value indicating the tag after.</param>
	internal static void TryAddTag(ref string tag, string name, string? before, string? after)
	{
		if (before != after)
			tag += after is null
			? GetCloseTag(name)
			: before is null
			? GetOpenTag(name, after)
			: GetCloseTag(name) + GetOpenTag(name, after);
	}

	extension(float? e)
	{
		/// <summary>
		/// Null or equal.
		/// </summary>
		/// <param name="obj">another item.</param>
		/// <returns>
		/// <list type="table">
		/// <item>When neither item is empty,<br />Returns true only if both are equal</item>
		/// <item>when one of the two is empty,<br />Returns true.</item>
		/// <item>when both are empty,<br />Returns false.</item>
		/// </list>
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool NullableEquals(float? obj) => (e != null && obj != null && e.Value == obj.Value) || (e == null && obj == null);
	}
}
