using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.Global.Converters;

/// <summary>
/// Base class for JSON converters that require access to <see cref="MetadataJsonSerializerOptions"/> during serialization.
/// </summary>
/// <typeparam name="T">The type being converted.</typeparam>
public abstract class MetadataJsonConverter<T> : JsonConverter<T>
{
	/// <summary>
	/// Gets or sets the metadata-aware serializer options associated with this converter.
	/// </summary>
	public MetadataJsonSerializerOptions? JsonSerializerOptions { get; internal set; }
	/// <summary>
	/// Associates the specified <see cref="JsonSerializerOptions"/> with this converter, creating a default
	/// <see cref="MetadataJsonSerializerOptions"/> wrapper if none exists.
	/// </summary>
	/// <param name="options">The standard JSON serializer options to wrap.</param>
	/// <returns>This converter instance for fluent chaining.</returns>
	public MetadataJsonConverter<T> WithOptions(JsonSerializerOptions options)
	{
		this.JsonSerializerOptions ??= new MetadataJsonSerializerOptions { JsonSerializerOptions = options };
		return this;
	}
	/// <summary>
	/// Associates the specified <see cref="MetadataJsonSerializerOptions"/> with this converter.
	/// </summary>
	/// <param name="options">The metadata-aware serializer options.</param>
	/// <returns>This converter instance for fluent chaining.</returns>
	public MetadataJsonConverter<T> WithOptions(MetadataJsonSerializerOptions options)
	{
		this.JsonSerializerOptions = options;
		return this;
	}
	/// <summary>
	/// Reads and converts JSON to an object, using metadata-aware serializer options.
	/// </summary>
	/// <param name="reader">The reader to read JSON from.</param>
	/// <param name="typeToConvert">The type to convert.</param>
	/// <param name="options">The metadata-aware serializer options.</param>
	/// <returns>The converted value.</returns>
	public abstract T? Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options);
	/// <summary>
	/// Writes a JSON representation of the object, using metadata-aware serializer options.
	/// </summary>
	/// <param name="writer">The writer to write JSON to.</param>
	/// <param name="value">The value to convert.</param>
	/// <param name="options">The metadata-aware serializer options.</param>
	public abstract void Write(Utf8JsonWriter writer, T value, MetadataJsonSerializerOptions options);
	/// <inheritdoc/>
	public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return Read(ref reader, typeToConvert, this.JsonSerializerOptions ?? new MetadataJsonSerializerOptions { JsonSerializerOptions = options });
	}
	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
	{
		Write(writer, value, this.JsonSerializerOptions ?? new MetadataJsonSerializerOptions { JsonSerializerOptions = options });
	}
}
