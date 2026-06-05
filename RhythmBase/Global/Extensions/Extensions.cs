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
		[DebuggerHidden]
		[StackTraceHidden]
		public static JsonTokenType ThrowIfNotMatch(ref Utf8JsonReader reader, params ReadOnlyEnumCollection<JsonTokenType> expectedTokenType)
		{
			if (expectedTokenType.Contains(reader.TokenType))
				return reader.TokenType;
			string message = $"Expected token {string.Join(", ", expectedTokenType)} but got {reader.TokenType} {(Encoding.UTF8.GetString(reader.ValueSpan.ToArray()))}, at byte position {reader.TokenStartIndex}.";
			throw new JsonException(message);
		}
	}
	extension<TTick>(ITickRange<TTick>)
			where TTick : struct, ITickTime<TTick>
	{
		/// <summary>
		/// Creates a new beat range with the specified start and end beats.
		/// </summary>
		/// <param name="start">The start beat, or null for no start bound.</param>
		/// <param name="end">The end beat, or null for no end bound.</param>
		/// <returns>A new <see cref="ITickRange{TBeat}"/> representing the specified range.</returns>
		public static ITickRange<TTick> CreateRange(TTick? start, TTick? end)
		{
			//switch (start, end)
			//{
			//    case (TTick s1, TTick e1):
			//        return (ITickRange<TTick>)(object)new Global.Components.Range(s1, e1);
			//    case (TTick s2, null):
			//        return (ITickRange<TTick>)(object)new Global.Components.Range(s2, null);
			//    case (null, TTick e2):
			//        return (ITickRange<TTick>)(object)new RhythmDoctor.Components.Range(null, e2);

			//    case (null, null):
			//        return ITickRange<TTick>.Infinity;
			//    default:
			//        throw new NotSupportedException();
			//}
			throw new NotImplementedException();
		}
		/// <summary>
		/// Gets an infinite beat range.
		/// </summary>
		public static ITickRange<TTick> Infinity => CreateRange<TTick>(null, null);
#if NETSTANDARD
		/// <summary>
		/// Creates a beat from a beat-only value. Not supported in .NET Standard.
		/// </summary>
		/// <param name="beatOnly">The beat-only value.</param>
		/// <returns>This method always throws <see cref="NotSupportedException"/>.</returns>
		/// <exception cref="NotSupportedException">Always thrown in .NET Standard.</exception>
		public static TTick FromBeatOnly(float beatOnly) => throw new NotSupportedException("This method is only supported in .NET 8.0 or later.");
		/// <summary>
		/// Creates a beat from a time span. Not supported in .NET Standard.
		/// </summary>
		/// <param name="timeSpan">The time span.</param>
		/// <returns>This method always throws <see cref="NotSupportedException"/>.</returns>
		/// <exception cref="NotSupportedException">Always thrown in .NET Standard.</exception>
		public static TTick FromTimeSpan(TimeSpan timeSpan) => throw new NotSupportedException("This method is only supported in .NET 8.0 or later.");
#endif
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
