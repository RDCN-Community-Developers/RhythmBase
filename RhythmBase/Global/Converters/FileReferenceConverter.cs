using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.Global.Converters;

/// <summary>
/// Converts a <see cref="FileReference"/> to and from its JSON string representation.
/// </summary>
[JsonConverterFor(typeof(FileReference))]
public class FileReferenceConverter : JsonConverter<FileReference>
{
	/// <inheritdoc/>
	public override FileReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		string? s = reader.GetString();
		return string.IsNullOrEmpty(s) ? default : new FileReference { Path = s! };
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, FileReference value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.Path);
	}
}
