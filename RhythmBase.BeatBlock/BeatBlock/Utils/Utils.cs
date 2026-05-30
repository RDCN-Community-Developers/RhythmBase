//namespace RhythmBase.BeatBlock.Utils
//{
//    internal static class Utils
//    {
//        private static readonly JsonSerializerOptions options;
//        static Utils()
//        {
//            options = new()
//            {
//                WriteIndented = true,
//            };
//        }
//        internal static MetadataJsonSerializerOptions GetJsonSerializerOptions(string? dir, ILevelReadSettings<IBaseEvent, EventType, BBBeat>? settings)
//        {
//            settings??=new LevelReadSettings();
//            MetadataJsonSerializerOptions options = new() { JsonSerializerOptions = new(Utils.options), Type = LevelType.BeatBlock };
//            return options; 
//        }
//        internal static MetadataJsonSerializerOptions GetJsonSerializerOptions(string? dir, ILevelWriteSettings<IBaseEvent, EventType, BBBeat>? settings)
//        {
//            settings??=new LevelWriteSettings();
//            MetadataJsonSerializerOptions options = new() { JsonSerializerOptions = (Utils.options), Type = LevelType.BeatBlock, WriteAligned = settings.WriteAligned };
//            options.JsonSerializerOptions.WriteIndented = settings.WriteIndented;
//            if (settings.EnableUnsafeRelaxedJsonEscaping)
//                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
//            return options;
//        }

//    }
//}