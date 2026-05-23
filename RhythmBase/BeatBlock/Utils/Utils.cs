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
        internal static RDJsonSerializerOptions GetJsonSerializerOptions(string? dir, ILevelReadSettings<IBaseEvent, EventType, BBBeat>? settings)
        {
            settings??=new LevelReadSettings();
            RDJsonSerializerOptions options = new() { JsonSerializerOptions = new(Utils.options), Type = LevelType.BeatBlock };
            return options; 
        }
        internal static RDJsonSerializerOptions GetJsonSerializerOptions(string? dir, ILevelWriteSettings<IBaseEvent, EventType, BBBeat>? settings)
        {
            settings??=new LevelWriteSettings();
            RDJsonSerializerOptions options = new() { JsonSerializerOptions = (Utils.options), Type = LevelType.BeatBlock, WriteAligned = settings.WriteAligned };
            if (settings.WriteIndented)
                options.JsonSerializerOptions.WriteIndented = true;
            else
                options.JsonSerializerOptions.WriteIndented = false;
            if (settings.EnableUnsafeRelaxedJsonEscaping)
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            return options;
        }

    }
}