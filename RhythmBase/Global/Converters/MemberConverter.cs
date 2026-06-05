using System.Text.Json;

namespace RhythmBase.Global.Converters;

/// <summary>
/// Base class for converters that read and write individual event (or other type) properties within a JSON object.
/// </summary>
/// <remarks>Unlike <see cref="MetadataJsonConverter{T}"/> which handles entire JSON objects, this converter
/// operates on the properties inside the object — the opening and closing braces are managed by the caller.</remarks>
/// <typeparam name="TEvent">The event or member type being converted.</typeparam>
public abstract class MemberConverter<TEvent> where TEvent : IEvent
{
	/// <summary>
	/// The read settings passed from the parent converter.
	/// </summary>
	protected LevelReadSettings? _rs;
	/// <summary>
	/// The write settings passed from the parent converter.
	/// </summary>
	protected LevelWriteSettings? _ws;
	/// <summary>
	/// Associates the specified read settings with this converter.
	/// </summary>
	/// <param name="settings">The level read settings.</param>
	/// <returns>This converter instance for fluent chaining.</returns>
	public MemberConverter<TEvent> WithReadSettings(LevelReadSettings settings)
	{
		_rs = settings;
		return this;
	}
	/// <summary>
	/// Associates the specified write settings with this converter.
	/// </summary>
	/// <param name="settings">The level write settings.</param>
	/// <returns>This converter instance for fluent chaining.</returns>
	public MemberConverter<TEvent> WithWriteSettings(LevelWriteSettings settings)
	{
		_ws = settings;
		return this;
	}
	/// <summary>
	/// Reads the properties of an event from the current JSON reader position.
	/// </summary>
	/// <param name="reader">The JSON reader positioned inside the event object.</param>
	/// <param name="options">The metadata-aware serializer options.</param>
	/// <returns>The deserialized event instance.</returns>
	public abstract TEvent ReadProperties(ref Utf8JsonReader reader, MetadataJsonSerializerOptions options);
	/// <summary>
	/// Writes the properties of an event to the specified JSON writer.
	/// </summary>
	/// <param name="writer">The JSON writer to write to.</param>
	/// <param name="value">The event instance to serialize.</param>
	/// <param name="options">The metadata-aware serializer options.</param>
	public abstract void WriteProperties(Utf8JsonWriter writer, TEvent value, MetadataJsonSerializerOptions options);
}
