using RhythmBase.BeatBlock.Components;
using RhythmBase.BeatBlock.Events;
using RhythmBase.BeatBlock.Settings;
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
        internal static JsonSerializerOptions GetJsonSerializerOptions(string? dir, ILevelReadSettings<IBaseEvent, EventType, BBBeat> settings)
        {
            settings??=new LevelReadSettings();
            JsonSerializerOptions options = new(Utils.options);
            return options; 
        }
    }
}