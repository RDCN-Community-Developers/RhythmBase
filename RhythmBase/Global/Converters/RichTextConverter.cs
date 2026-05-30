using RhythmBase.Global.Components.RichText;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.Global.Converters;

public class RichTextConverter<TRichTextStyle> : JsonConverter<RichLine<TRichTextStyle>> where TRichTextStyle : IRichStringStyle<TRichTextStyle>, new()
{
	public override RichLine<TRichTextStyle> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
#if NETSTANDARD2_0
		return RichLine<TRichTextStyle>.Empty.Deserialize(reader.GetString() ?? string.Empty);
#else
		return RichLine<TRichTextStyle>.Deserialize(reader.GetString() ?? string.Empty);
#endif
	}

	public override void Write(Utf8JsonWriter writer, RichLine<TRichTextStyle> value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.Serialize());
	}
}
