using RhythmBase.BeatBlock.Components;
using RhythmBase.BeatBlock.Events;
using RhythmBase.BeatBlock.Settings;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace RhythmBase.BeatBlock.Utils
{
    internal static class Utils
    {
        private static readonly JsonSerializerOptions options;
        static Utils()
        {
            options = new()
            {
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                //AllowTrailingCommas = true,
            };
        }
        internal static JsonSerializerOptions GetJsonSerializerOptions(string? dir, ILevelReadSettings<IBaseEvent, EventType, BBBeat>? settings)
        {
            settings??=new LevelReadSettings();
            JsonSerializerOptions options = new(Utils.options);
            return options; 
        }
        internal static JsonSerializerOptions GetJsonSerializerOptions(string? dir, ILevelWriteSettings<IBaseEvent, EventType, BBBeat>? settings)
        {
            settings??=new LevelWriteSettings();
            JsonSerializerOptions options = new(Utils.options);
            if(settings.Indented)
                options.WriteIndented = true;
            else
                options.WriteIndented = false;
            if(settings.EnableUnsafeRelaxedJsonEscaping)
                options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            return options;
        }

    }
}