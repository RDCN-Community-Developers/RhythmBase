using RhythmBase.Adofai.Components;
using RhythmBase.Adofai.Converters;
using RhythmBase.Adofai.Events;
using RhythmBase.Adofai.Settings;
using System.Text.Json;
namespace RhythmBase.Adofai.Utils;

	/// <summary>
	/// Useful utils.
	/// </summary>
	public static class Utils
	{
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
				//	new LevelConverter(),
				//	new TileReferenceConverter(),
				//	new FileReferenceConverter(),
				//}
			};
		}
		/// <summary>  
		/// Represents the angle used for mid-spin calculations.  
		/// </summary>  
		public const float MidSpinAngle = 999f;
		/// <summary>
		/// Creates and configures a <see cref="JsonSerializerOptions"/> instance for serializing and deserializing JSON data.
		/// </summary>
		internal static MetadataJsonSerializerOptions GetJsonSerializerOptions(string? filepath = null, LevelReadSettings? settings = null)
		{
			settings ??= new LevelReadSettings();
			MetadataJsonSerializerOptions options = new() { Type = LevelType.Adofai, JsonSerializerOptions = new(Utils.options) };
			LevelConverter levelConverter = new()
			{
				ReadSettings = settings,
				Filepath = filepath
			};
			//options.JsonSerializerOptions.Converters.Add(levelConverter);
			return options;
		}
		/// <summary>
		/// Creates and configures a <see cref="JsonSerializerOptions"/> instance for serializing and deserializing JSON data.
		/// </summary>
		internal static MetadataJsonSerializerOptions GetJsonSerializerOptions(string? filepath = null, LevelWriteSettings? settings = null)
		{
			settings ??= new LevelWriteSettings();
			MetadataJsonSerializerOptions options = new() { Type = LevelType.Adofai, JsonSerializerOptions = new(Utils.options) };
			if (settings.WriteIndented)
				options.JsonSerializerOptions.WriteIndented = true;
			else
				options.JsonSerializerOptions.WriteIndented = false;
			LevelConverter levelConverter = new()
			{
				WriteSettings = settings,
				Filepath = filepath
			};
			//options.JsonSerializerOptions.Converters.Add(levelConverter);
			return options;
		}
	}
