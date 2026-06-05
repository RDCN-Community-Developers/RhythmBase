//using System.Text.Json;

//namespace RhythmBase.Global.Converters;

//public class FileMainEntryConverter
//{
//	public static T DeserializeMainEntry<T>(IJsonDataSource dataSource, RhythmBase.Global.Converters.MetadataJsonSerializerOptions options)
//			where T : new()
//	{
//		ReadOnlyMemory<byte> jsonData =
//				dataSource.CanGetMemoryDirectly
//				? dataSource.GetMemory()
//				: dataSource.GetMemoryAsync().GetAwaiter().GetResult();
//		Utf8JsonReader reader = new(jsonData.Span,
//				new() { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip });
//		return RhythmBase.Global.Converters.TypeConverterRegistry.Read<T>(ref reader, options) ?? new();
//	}
//	public static async Task<T> DeserializeMainEntryAsync<T>(IJsonDataSource dataSource, RhythmBase.Global.Converters.MetadataJsonSerializerOptions options, CancellationToken cancellationToken = default)
//			where T : new()
//	{
//		Utf8JsonReader reader = new((await dataSource.GetMemoryAsync(cancellationToken)).Span,
//				new() { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip });
//		return TypeConverterRegistry.Read<T>(ref reader, options) ?? new();
//	}
//	public static void SerializeMainEntry<T>(T mainEntry, Stream stream, RhythmBase.Global.Converters.MetadataJsonSerializerOptions options)
//	{
//		using Utf8JsonWriter writer = new(stream, new()
//		{
//			Indented = options.JsonSerializerOptions.WriteIndented,
//			Encoder = options.JsonSerializerOptions.Encoder,
//			IndentCharacter = options.JsonSerializerOptions.IndentCharacter,
//			IndentSize = options.JsonSerializerOptions.IndentSize,
//		});
//		RhythmBase.Global.Converters.TypeConverterRegistry.Write(writer, mainEntry, options);
//		writer.Flush();
//	}
//}
