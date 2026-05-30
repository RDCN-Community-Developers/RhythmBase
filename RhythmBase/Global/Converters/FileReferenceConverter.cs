using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.Global.Converters;

[JsonConverterFor(typeof(FileReference))]
public class FileReferenceConverter : JsonConverter<FileReference>
{
	public override FileReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		string? s = reader.GetString();
		return string.IsNullOrEmpty(s) ? default : new FileReference { Path = s! };
	}

	public override void Write(Utf8JsonWriter writer, FileReference value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.Path);
	}
}
