namespace RhythmBase.RhythmDoctor.Converters;

//[JsonConverterFor(typeof(Tab))]
//internal class TabsConverter : JsonConverter<Tab>
//{
//	public override Tab Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//	{
//		if (reader.TokenType != JsonTokenType.String)
//			throw new JsonException($"Expected String token, but got {reader.TokenType}.");
//		Tab tabs = reader.ValueSpan switch
//		{
//			var v when v.SequenceEqual("Song"u8) => Tab.Sounds,
//			var v when v.SequenceEqual("Sprites"u8) => Tab.Decorations,
//			var v when v.SequenceEqual("Actions"u8) => Tab.Actions,
//			var v when v.SequenceEqual("Rooms"u8) => Tab.Rooms,
//			var v when v.SequenceEqual("Windows"u8) => Tab.Windows,
//			_ => throw new ArgumentException($"Invalid tabs value: '{reader.GetString()}'.")
//		};
//		return tabs;
//	}

//	public override void Write(Utf8JsonWriter writer, Tab value, JsonSerializerOptions options)
//	{
//		writer.WriteStringValue(value switch
//		{
//			Tab.Sounds => "Song",
//			Tab.Decorations => "Sprites",
//			Tab.Actions => "Actions",
//			Tab.Rooms => "Rooms",
//			Tab.Windows => "Windows",
//			_ => throw new ArgumentException($"Invalid tabs value: '{value}'.")
//		});
//	}
//}
