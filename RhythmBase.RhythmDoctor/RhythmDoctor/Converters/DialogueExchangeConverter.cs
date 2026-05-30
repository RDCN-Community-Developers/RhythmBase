using RhythmBase.Global.Components.RichText;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.RhythmDoctor.Converters;

[JsonConverterFor(typeof(DialogueExchange))]
internal class DialogueExchangeConverter : JsonConverter<DialogueExchange>
{
	public override DialogueExchange? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if(reader.TokenType != JsonTokenType.String)
			throw new JsonException($"Expected String token, but got {reader.TokenType}.");
		try
		{
			return DialogueExchange.Deserialize(reader.GetString() ?? "");
		}
		catch (Exception ex)
		{
			throw new JsonException("Failed to parse dialogue exchange.", ex);
		}
	}

	public override void Write(Utf8JsonWriter writer, DialogueExchange value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.Serialize());
	}
}
