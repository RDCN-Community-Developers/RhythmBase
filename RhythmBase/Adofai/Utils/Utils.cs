using RhythmBase.Adofai.Components;
using RhythmBase.Adofai.Converters;
using RhythmBase.Adofai.Events;
using RhythmBase.Adofai.Settings;
using System.Text.Json;
namespace RhythmBase.Adofai.Utils
{
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
				Converters =
				{
					new LevelConverter(),
					new TileReferenceConverter(),
					new FileReferenceConverter(),
				}
			};
		}
		/// <summary>  
		/// Represents the angle used for mid-spin calculations.  
		/// </summary>  
		public const float MidSpinAngle = 999f;
		/// <summary>
		/// Creates and configures a <see cref="JsonSerializerOptions"/> instance for serializing and deserializing JSON data.
		/// </summary>
		public static JsonSerializerOptions GetJsonSerializerOptions(string? filepath = null, ILevelReadSettings<IBaseEvent, EventType, ADBeat>? settings = null)
		{
			settings ??= new LevelReadSettings();
			JsonSerializerOptions options = new(Utils.options);
			LevelConverter levelConverter = new()
			{
				ReadSettings = settings,
				Filepath = filepath
			};
			options.Converters.Add(levelConverter);
			return options;
		}
		/// <summary>
		/// Creates and configures a <see cref="JsonSerializerOptions"/> instance for serializing and deserializing JSON data.
		/// </summary>
		public static JsonSerializerOptions GetJsonSerializerOptions(string? filepath = null, ILevelWriteSettings<IBaseEvent, EventType, ADBeat>? settings = null)
		{
			settings ??= new LevelWriteSettings();
			JsonSerializerOptions options = new(Utils.options);
			if (settings.Indented)
				options.WriteIndented = true;
			else
				options.WriteIndented = false;
			LevelConverter levelConverter = new()
			{
				WriteSettings = settings,
				Filepath = filepath
			};
			options.Converters.Add(levelConverter);
			return options;
		}
	}
}
