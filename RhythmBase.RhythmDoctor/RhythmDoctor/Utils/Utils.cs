using RhythmBase.RhythmDoctor.Converters;
using System.Collections.ObjectModel;
using System.Text.Encodings.Web;
using System.Text.Json;
namespace RhythmBase.RhythmDoctor.Utils;

	/// <summary>
	/// Static class providing utility methods.
	/// </summary>
	public static class Utils
	{
			private static readonly BaseEventConverter evc = new();
			private static readonly JsonSerializerOptions options;
			static Utils()
			{
					options = new()
					{
							WriteIndented = true,
							DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
							AllowTrailingCommas = true,
							//Converters =
							//{
							//	new AudioConverter(),
							//	new PaletteColorConverter(),
							//	new PaletteColorWithAlphaConverter(),
							//	new DialogueExchangeConverter(),
							//	new PatternConverter(),
							//	new TabsConverter(),
							//	new RoomConverter(),
							//	new SingleRoomConverter(),
							//	new ExpressionConverter(),
							//	new FileReferenceConverter(),
							//}
					};
			}

			/// <summary>
			/// Gets the <see cref="JsonSerializerOptions"/> configured for serializing or deserializing a level, and sets the file path for the converter.
			/// </summary>
			/// <param name="dirPath">
			/// The file path to associate with the level converter.
			/// </param>
			/// <param name="settings">
			/// The <see cref="LevelReadSettings"/> to use for serialization options. If <c>null</c>, a new instance is used.
			/// </param>
			/// <returns>
			/// A <see cref="JsonSerializerOptions"/> instance configured with converters, indentation settings, and file path.
			/// </returns>
			internal static JsonSerializerOptions GetJsonSerializerOptions(string? dirPath = null, LevelReadSettings? settings = null)
			{
					settings ??= new LevelReadSettings();
					JsonSerializerOptions options = new(Utils.options);
					//LevelConverter levelConverter = new()
					//{
					//	ReadSettings = settings,
					//	DirectoryName = dirPath,
					//};
					//options.Converters.Add(levelConverter);
					return options;
			}
			/// <summary>
			/// Gets the <see cref="JsonSerializerOptions"/> configured for serializing or deserializing a level, and sets the file path for the converter.
			/// </summary>
			/// <param name="dirPath">
			/// The file path to associate with the level converter.
			/// </param>
			/// <param name="settings">
			/// The <see cref="LevelWriteSettings"/> to use for serialization options. If <c>null</c>, a new instance is used.
			/// </param>
			/// <returns>
			/// A <see cref="JsonSerializerOptions"/> instance configured with converters, indentation settings, and file path.
			/// </returns>
			internal static JsonSerializerOptions GetJsonSerializerOptions(string? dirPath = null, LevelWriteSettings? settings = null)
			{
					settings ??= new LevelWriteSettings();
					JsonSerializerOptions options = new(Utils.options);
					if (settings.WriteIndented)
							options.WriteIndented = true;
					else
							options.WriteIndented = false;
					if (settings.EnableUnsafeRelaxedJsonEscaping)
							options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
					//LevelConverter levelConverter = new()
					//{
					//	WriteSettings = settings,
					//	DirectoryName = dirPath,
					//};
					//options.Converters.Add(levelConverter);
					return options;
			}
			/// <summary>
			/// The default beats per minute.
			/// </summary>
			public const float DefaultBPM = 100f;
			/// <summary>
			/// The default crotchets per bar.
			/// </summary>
			public const int DefaultCPB = 8;
			/// <summary>
			/// Gets a read-only collection of default expressions.
			/// </summary>
			public static ReadOnlyCollection<string> DefaultExpressions { get; } = new([
					"neutral",
			"happy",
			"barely",
			"missed",
			"prehit",
			"beep",
		]);
	}
