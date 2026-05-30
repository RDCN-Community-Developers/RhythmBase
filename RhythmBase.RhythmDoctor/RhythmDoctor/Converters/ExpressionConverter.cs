using RhythmBase.RhythmDoctor.Components;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace RhythmBase.RhythmDoctor.Converters;

[JsonConverterFor(typeof(Expression))]
internal class ExpressionConverter : JsonConverter<Expression>
{
	public override void Write(Utf8JsonWriter writer, Expression value, JsonSerializerOptions serializer)
	{
		if (value.IsNumeric)
			writer.WriteRawValue(value.NumericValue.ToString());
		else if (string.IsNullOrEmpty(value.ExpressionValue))
			writer.WriteNullValue();
		else
			writer.WriteStringValue($"{{{value.ExpressionValue}}}");
	}
	public override Expression Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions serializer)
	{
		if (reader.TokenType == JsonTokenType.Number)
			return new Expression(reader.GetSingle());
		else return reader.TokenType == JsonTokenType.String
			? new Expression(reader.GetString()?.TrimStart('{').TrimEnd('}') ?? string.Empty)
			: default;
	}
}
