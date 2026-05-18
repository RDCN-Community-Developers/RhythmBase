using RhythmBase.RhythmDoctor.Components;
using System.Text;
using System.Text.Json;

namespace RhythmBase.Global.Extensions;

/// <summary>
/// Provides extension methods and utility methods for RhythmBase types.
/// </summary>
public static class Extensions
{
    extension(JsonException)
    {
        internal static JsonTokenType ThrowIfNotMatch(Utf8JsonReader reader, JsonTokenType[] expectedTokenType)
        {
            if (expectedTokenType.Contains(reader.TokenType))
                return reader.TokenType;
            string message = $"Expected token {string.Join(", ", expectedTokenType)} but got {reader.TokenType} {(Encoding.UTF8.GetString(reader.ValueSpan.ToArray()))}, at byte position {reader.TokenStartIndex}.";
            throw new JsonException(message);
        }
    }
    extension<TBeat>(IBeatRange<TBeat>)
        where TBeat : struct, IBeat<TBeat>
    {
        /// <summary>
        /// Creates a new beat range with the specified start and end beats.
        /// </summary>
        /// <param name="start">The start beat, or null for no start bound.</param>
        /// <param name="end">The end beat, or null for no end bound.</param>
        /// <returns>A new <see cref="IBeatRange{TBeat}"/> representing the specified range.</returns>
        public static IBeatRange<TBeat> CreateRange(TBeat? start, TBeat? end)
        {
            switch (start, end)
            {
                case (RDBeat s1, RDBeat e1):
                    return (IBeatRange<TBeat>)(object)new RDRange(s1, e1);
                case (RDBeat s2, null):
                    return (IBeatRange<TBeat>)(object)new RDRange(s2, null);
                case (null, RDBeat e2):
                    return (IBeatRange<TBeat>)(object)new RDRange(null, e2);

                case (null, null):
                    return IBeatRange<TBeat>.Infinity;
                default:
                    throw new NotSupportedException();
            }
        }
        /// <summary>
        /// Gets an infinite beat range.
        /// </summary>
        public static IBeatRange<TBeat> Infinity => CreateRange<TBeat>(null, null);
#if NETSTANDARD
        /// <summary>
        /// Creates a beat from a beat-only value. Not supported in .NET Standard.
        /// </summary>
        /// <param name="beatOnly">The beat-only value.</param>
        /// <returns>This method always throws <see cref="NotSupportedException"/>.</returns>
        /// <exception cref="NotSupportedException">Always thrown in .NET Standard.</exception>
        public static TBeat FromBeatOnly(float beatOnly) => throw new NotSupportedException("This method is only supported in .NET 8.0 or later.");
        /// <summary>
        /// Creates a beat from a time span. Not supported in .NET Standard.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <returns>This method always throws <see cref="NotSupportedException"/>.</returns>
        /// <exception cref="NotSupportedException">Always thrown in .NET Standard.</exception>
        public static TBeat FromTimeSpan(TimeSpan timeSpan) => throw new NotSupportedException("This method is only supported in .NET 8.0 or later.");
#endif
    }
    /// <summary>
    /// Gets the read-only enum collection of event types for the specified event and target types.
    /// </summary>
    /// <typeparam name="TEvent">The event type. Must implement <see cref="IEvent{TType, TBeat}"/>.</typeparam>
    /// <typeparam name="TTarget">The target event type. Must implement <see cref="IEvent{TType, TBeat}"/>.</typeparam>
    /// <typeparam name="TType">The event type enum. Must be a struct and an enum.</typeparam>
    /// <typeparam name="TBeat">The beat type. Must be a struct and implement <see cref="IBeat{TBeat}"/>.</typeparam>
    /// <returns>A <see cref="ReadOnlyEnumCollection{TType}"/> containing the event types.</returns>
    public static ReadOnlyEnumCollection<TType> TypesOf<TEvent, TTarget, TType, TBeat>()
        where TEvent : IEvent<TType, TBeat>
        where TTarget : IEvent<TType, TBeat>
        where TType : struct, Enum
        where TBeat : struct, IBeat<TBeat>
    {
        if (typeof(TType) == typeof(RhythmDoctor.EventType))
            return RhythmDoctor.Utils.EventTypeUtils.ToEnums(typeof(TType)) as ReadOnlyEnumCollection<TType>? ?? [];
        else if (typeof(TType) == typeof(Adofai.EventType))
            return Adofai.Utils.EventTypeUtils.ToEnums(typeof(TType)) as ReadOnlyEnumCollection<TType>? ?? [];
        else if (typeof(TType) == typeof(BeatBlock.EventType))
            //return BeatBlock.Utils.EventTypeUtils.ToEnums(typeof(TType)) as ReadOnlyEnumCollection<TType>? ?? [];
            throw new NotImplementedException();
        else
            throw new NotSupportedException($"Unsupported event type enum: {typeof(TType)}");
    }
}
