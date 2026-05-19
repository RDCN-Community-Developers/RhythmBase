using RhythmBase.BeatBlock.Converters;
using RhythmBase.BeatBlock.Events;
using RhythmBase.BeatBlock.Settings;
using RhythmBase.Global.Extensions;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using static RhythmBase.BeatBlock.Utils.EventTypeUtils;

namespace RhythmBase.BeatBlock.Components;

partial class BBLevel
{
    private static class Deserializer
    {
        private static readonly BaseEventConverter baseEventConverter = new();
        public static BBLevel DeserializeManifest(IJsonDataSource dataSource, RDJsonSerializerOptions options)
        {
            ReadOnlyMemory<byte> jsonData =
                dataSource.CanGetMemoryDirectly
                ? dataSource.GetMemory()
                : dataSource.GetMemoryAsync().GetAwaiter().GetResult();
            Utf8JsonReader reader = new(jsonData.Span, new() { AllowTrailingCommas = true });
            return ConverterHub.Read<BBLevel>(ref reader, options) ?? [];
        }
        public static async Task<BBLevel> DeserializeLevelAsync(IJsonDataSource dataSource, RDJsonSerializerOptions options, CancellationToken token = default)
        {
            ReadOnlyMemory<byte> jsonData =
                 dataSource.CanGetMemoryDirectly
                ? dataSource.GetMemory()
                : await dataSource.GetMemoryAsync(token);
            Utf8JsonReader reader = new(jsonData.Span, new() { AllowTrailingCommas = true });
            return ConverterHub.Read<BBLevel>(ref reader, options) ?? [];
        }
        public static void DeserializeLevel(IJsonDataSource dataSource, RDJsonSerializerOptions options, BBLevel level, ILevelReadSettings<IBaseEvent, EventType, BBBeat> settings)
        {
            ReadOnlyMemory<byte> jsonData =
                dataSource.CanGetMemoryDirectly
                ? dataSource.GetMemory()
                : dataSource.GetMemoryAsync().GetAwaiter().GetResult();
            Utf8JsonReader reader = new(jsonData.Span, new() { AllowTrailingCommas = true });
            reader.Read();
            JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartObject]);
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;
                ReadOnlySpan<byte> propertyName = reader.ValueSpan;
                reader.Read();
                if (propertyName.SequenceEqual("events"u8))
                    foreach (IBaseEvent e in DeserializeEvents(ref reader, options, settings))
                        level.Add(e);
                else
                {
                    reader.Skip();
                }
            }
        }
        public static void DeserializeEvents(IJsonDataSource dataSource, string variantName, RDJsonSerializerOptions options, BBLevel level, ILevelReadSettings<IBaseEvent, EventType, BBBeat> settings)
        {
            ReadOnlyMemory<byte> jsonData =
                dataSource.CanGetMemoryDirectly
                ? dataSource.GetMemory()
                : dataSource.GetMemoryAsync().GetAwaiter().GetResult();
            Utf8JsonReader reader = new(jsonData.Span, new() { AllowTrailingCommas = true });
            reader.Read();
            JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
            foreach (IBaseEvent e in DeserializeEvents(ref reader, options, settings))
            {
                e.Variant = variantName;
                level.Add(e);
            }
            reader.Read();
        }
        private static List<IBaseEvent> DeserializeEvents(ref Utf8JsonReader reader, RDJsonSerializerOptions options, ILevelReadSettings<IBaseEvent, EventType, BBBeat> settings)
        {
            List<IBaseEvent> events = new();
            JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
            int index = 0;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;
                IBaseEvent? e = null;
#if DEBUG
                try
                {
                    e = baseEventConverter.Read(ref reader, typeof(IBaseEvent), options);
                    index++;
                }
                catch (Exception ex)
                {
                    Debug.Print($"Current index: {index}");
                    throw;
                }
#else
                Utf8JsonReader checkpoint = reader;
                try
                {
                    e = baseEventConverter.Read(ref reader, typeof(IBaseEvent), options);
                }
                catch (JsonException)
                {
                    level.Dispose();
                    throw;
                }
                catch (Exception ex)
                {
                    JsonElement element = JsonElement.ParseValue(ref checkpoint);
                    settings.HandleUnreadableEvent(element, ex.Message);
                    continue;
                }
#endif
                if (e == null)
                    continue;
                events.Add(e);
            }
            return events;
        }
        public static void WriteManifestToStream(Stream stream, BBLevel level, RDJsonSerializerOptions options)
        {
            using Utf8JsonWriter writer = new(stream, new() { Indented = options.JsonSerializerOptions.WriteIndented });
            ConverterHub.Write(writer, level, options);
            writer.Flush();
        }
        public static void WriteLevelToStream(Stream stream, BBLevel level, RDJsonSerializerOptions options)
        {
            using Utf8JsonWriter writer = new(stream, new() { Indented = options.JsonSerializerOptions.WriteIndented });
            writer.WriteStartObject();
            writer.WriteStartArray("events"u8);
            foreach (var e in level.Where(i => i is not IPureEvent))
                baseEventConverter.Write(writer, e, options);
            writer.WriteEndArray();
            writer.WriteEndObject();
            writer.Flush();
        }
        public static void WriteEventsToStream(Stream stream, BBLevel level, string variantName, RDJsonSerializerOptions options)
        {
            using Utf8JsonWriter writer = new(stream, new() { Indented = options.JsonSerializerOptions.WriteIndented });
            writer.WriteStartArray();
            foreach (IBaseEvent e in level.Where(e => e.Variant == variantName))
                baseEventConverter.Write(writer, e, options);
            writer.WriteEndArray();
            writer.Flush();
        }
    }
    #region dir
    /// <inheritdoc/>
    public static BBLevel FromDirectory(string directoryPath, ILevelReadSettings<IBaseEvent, EventType, BBBeat>? settings = null) => FromDirectoryAsync(directoryPath, settings).GetAwaiter().GetResult();
    /// <inheritdoc/>
    public static async Task<BBLevel> FromDirectoryAsync(string directoryPath, ILevelReadSettings<IBaseEvent, EventType, BBBeat>? settings = null, CancellationToken cancellationToken = default)
    {
        settings ??= new LevelReadSettings();
        RDJsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(directoryPath, settings);
        BBLevel? level;
        string manifestFilePath = Path.Combine(directoryPath, "manifest.json");
        using FileStream manifestFs = File.Open(manifestFilePath, FileMode.Open, FileAccess.Read);
        level = Deserializer.DeserializeManifest(new StreamDataSource(manifestFs), options);
        string levelFile = Path.Combine(directoryPath, "level.json");
        using FileStream levelFs = File.Open(levelFile, FileMode.Open, FileAccess.Read);
        Deserializer.DeserializeLevel(new StreamDataSource(levelFs), options, level, settings);
        foreach (Variant variant in level.Variants)
        {
            string chartFile = Path.Combine(directoryPath, $"chart-{variant.Name}.json");
            using FileStream chartFs = File.Open(chartFile, FileMode.Open, FileAccess.Read);
            Deserializer.DeserializeEvents(new StreamDataSource(chartFs), variant.Name, options, level, settings);
        }
        return level;
    }
    /// <inheritdoc/>
    public void SaveToDirectory(string directoryPath, ILevelWriteSettings<IBaseEvent, EventType, BBBeat>? settings = null) => SaveToDirectoryAsync(directoryPath, settings).GetAwaiter().GetResult();
    /// <inheritdoc/>
    public Task SaveToDirectoryAsync(string directoryPath, ILevelWriteSettings<IBaseEvent, EventType, BBBeat>? settings = null, CancellationToken cancellationToken = default)
    {
        settings ??= new LevelWriteSettings();
        RDJsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(directoryPath, settings);
        string manifestFilePath = Path.Combine(directoryPath, "manifest.json");
        using FileStream manifestFs = File.Open(manifestFilePath, FileMode.Create, FileAccess.Write);
        Deserializer.WriteManifestToStream(manifestFs, this, options);
        string levelFile = Path.Combine(directoryPath, "level.json");
        using FileStream levelFs = File.Open(levelFile, FileMode.Create, FileAccess.Write);
        Deserializer.WriteLevelToStream(levelFs, this, options);
        foreach (Variant variant in Variants)
        {
            string chartFile = Path.Combine(directoryPath, $"chart-{variant.Name}.json");
            using FileStream chartFs = File.Open(chartFile, FileMode.Create, FileAccess.Write);
            Deserializer.WriteEventsToStream(chartFs, this, variant.Name, options);
        }
        return Task.CompletedTask;
    }
    #endregion
    #region zip
    /// <inheritdoc/>
    public static BBLevel FromZip(string filepath, ILevelReadSettings<IBaseEvent, EventType, BBBeat>? settings = null)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public static Task<BBLevel> FromZipAsync(string filepath, ILevelReadSettings<IBaseEvent, EventType, BBBeat>? settings = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    /// <inheritdoc/>
    public void SaveToZip(string filepath, ILevelWriteSettings<IBaseEvent, EventType, BBBeat>? settings = null)
    {
        throw new NotImplementedException();
    }
    /// <inheritdoc/>
    public Task SaveToZipAsync(string filepath, ILevelWriteSettings<IBaseEvent, EventType, BBBeat>? settings = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    #endregion
    #region json
    /// <inheritdoc/>
    public static BBLevel FromJsonDocument(JsonDocument jsonDocument, ILevelReadSettings<IBaseEvent, EventType, BBBeat>? settings = null)
    {
        throw new NotImplementedException();
    }
    /// <inheritdoc/>
    public static BBLevel FromJsonString(string json, ILevelReadSettings<IBaseEvent, EventType, BBBeat>? settings = null)
    {
        throw new NotImplementedException();
    }
    /// <inheritdoc/>
    public JsonDocument ToJsonDocument(ILevelWriteSettings<IBaseEvent, EventType, BBBeat>? settings = null)
    {
        throw new NotImplementedException();
    }
    /// <inheritdoc/>
    public string ToJsonString(ILevelWriteSettings<IBaseEvent, EventType, BBBeat>? settings = null)
    {
        throw new NotImplementedException();
    }
    #endregion
}
