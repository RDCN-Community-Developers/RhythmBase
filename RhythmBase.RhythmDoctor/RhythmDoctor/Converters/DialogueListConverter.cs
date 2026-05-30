namespace RhythmBase.RhythmDoctor.Converters;

//internal class DialogueListConverter : JsonConverter<RDDialogueExchange>
//{
//	public override RDDialogueExchange? Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions serializer)
//	{
//		string? s = reader.GetString();
//		if (string.IsNullOrEmpty(s))
//			return null;
//		try
//		{
//			return RDDialogueExchange.Deserialize(s!);
//		}
//		catch (Exception ex)
//		{
//			throw new JsonException($"Failed to deserialize RDDialogueExchange: {ex.Message}", ex);
//		}
//	}
//	public override void Write(Utf8JsonWriter writer, RDDialogueExchange? value, JsonSerializerOptions serializer) => writer.WriteStringValue(value?.Serialize());
//}
