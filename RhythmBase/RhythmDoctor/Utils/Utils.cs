using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Converters;
using RhythmBase.RhythmDoctor.Events;
using RhythmBase.RhythmDoctor.Settings;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
namespace RhythmBase.RhythmDoctor.Utils
{
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
		internal static JsonSerializerOptions GetJsonSerializerOptions(string? dirPath = null, ILevelReadSettings<IBaseEvent, EventType, RDBeat>? settings = null)
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
		internal static JsonSerializerOptions GetJsonSerializerOptions(string? dirPath = null, ILevelWriteSettings<IBaseEvent, EventType, RDBeat>? settings = null)
		{
			settings ??= new LevelWriteSettings();
			JsonSerializerOptions options = new(Utils.options);
			if (settings.WriteIndented)
				options.WriteIndented = true;
			else
				options.WriteIndented = false;
			if(settings.EnableUnsafeRelaxedJsonEscaping)
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
		/// <summary>
		/// Converts the specified <see cref="IBaseEvent"/> instance to its JSON string representation.
		/// </summary>
		/// <remarks>The JSON output can be customized using the provided <paramref name="options"/>.  If no options
		/// are specified, the default settings are applied.</remarks>
		/// <param name="ev">The <see cref="IBaseEvent"/> instance to serialize. Cannot be <see langword="null"/>.</param>
		/// <param name="options">Optional <see cref="JsonSerializerOptions"/> to configure the serialization process.  If <see langword="null"/>,
		/// default serialization options are used.</param>
		/// <returns>A JSON string representation of the <see cref="IBaseEvent"/> instance.</returns>
		public static string ToJsonString(this IBaseEvent ev, JsonSerializerOptions? options = null)
		{
			options ??= new JsonSerializerOptions(Utils.options);
			using MemoryStream stream = new();
			using Utf8JsonWriter writer = new(stream, new JsonWriterOptions { Indented = options.WriteIndented, });
			evc.Write(writer, ev, options);
			writer.Flush();
			return Encoding.UTF8.GetString(stream.ToArray());
		}
		/// <summary>
		/// Deserializes a JSON string into an instance of a type implementing <see cref="IBaseEvent"/>.
		/// </summary>
		/// <param name="json">The JSON string to deserialize. This parameter cannot be <see langword="null"/> or empty.</param>
		/// <param name="options">Optional <see cref="JsonSerializerOptions"/> to customize the deserialization process.  If not provided, the
		/// default options will be used.</param>
		/// <returns>An instance of a type implementing <see cref="IBaseEvent"/> if deserialization is successful;  otherwise, <see
		/// langword="null"/>.</returns>
		public static IBaseEvent? FromJsonString(string json, JsonSerializerOptions? options = null)
		{
			options ??= new JsonSerializerOptions(Utils.options);
			Utf8JsonReader reader = new(Encoding.UTF8.GetBytes(json));
			reader.Read();
			return evc.Read(ref reader, typeof(IBaseEvent), options);
		}
	}
}
