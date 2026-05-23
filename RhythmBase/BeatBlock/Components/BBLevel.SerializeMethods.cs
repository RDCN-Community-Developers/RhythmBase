using RhythmBase.BeatBlock.Converters;
using RhythmBase.BeatBlock.Events;
using RhythmBase.BeatBlock.Settings;
using RhythmBase.Global.Extensions;
using System.Collections;
using System.Collections.ObjectModel;
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
            return ConverterHub.Read<BBLevel>(ref reader, options) ?? new();
        }
        public static async Task<BBLevel> DeserializeManifestAsync(IJsonDataSource dataSource, RDJsonSerializerOptions options, CancellationToken token = default)
        {
            ReadOnlyMemory<byte> jsonData =
                 dataSource.CanGetMemoryDirectly
                ? dataSource.GetMemory()
                : await dataSource.GetMemoryAsync(token);
            Utf8JsonReader reader = new(jsonData.Span, new() { AllowTrailingCommas = true });
            return ConverterHub.Read<BBLevel>(ref reader, options) ?? new();
        }
        public static void DeserializeLevel(IJsonDataSource dataSource, RDJsonSerializerOptions options, Variant variant, ILevelReadSettings<IBaseEvent, EventType, BBBeat> settings)
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
                        variant.Add(e);
                else
                {
                    reader.Skip();
                }
            }
        }
        public static void DeserializeChart(IJsonDataSource dataSource, RDJsonSerializerOptions options, Variant variant, ILevelReadSettings<IBaseEvent, EventType, BBBeat> settings)
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
                variant.Add(e);
            }
            reader.Read();
        }
        public static void DeserializeTag(IJsonDataSource dataSource, RDJsonSerializerOptions options, TagEventCollection collection, ILevelReadSettings<IBaseEvent, EventType, BBBeat> settings)
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
                collection.Add(e);
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
        public static void WriteVariantLevelToStream(Stream stream, NoIndentScope noIndentScope, Variant level, RDJsonSerializerOptions options)
        {
            using Utf8JsonWriter writer = new(stream, new() { Indented = options.JsonSerializerOptions.WriteIndented });
            writer.WriteStartObject();
            writer.WriteStartArray("events"u8);
            noIndentScope.WriteNoIndentArrayTo(false, writer, level, baseEventConverter.Write);
            writer.WriteEndArray();
            writer.WriteEndObject();
            writer.Flush();
        }
        public static void WriteVariantChartsToStream(Stream stream, NoIndentScope noIndentScope, Variant variant, RDJsonSerializerOptions options)
        {
            using Utf8JsonWriter writer = new(stream, new() { Indented = options.JsonSerializerOptions.WriteIndented });
            writer.WriteStartArray();
            noIndentScope.WriteNoIndentArrayTo(false, writer, variant, baseEventConverter.Write);
            writer.WriteEndArray();
            writer.Flush();
        }
        public static void WriteTagEventsToStream(Stream stream, NoIndentScope noIndentScope, TagEventCollection collection, RDJsonSerializerOptions options)
        {
            using Utf8JsonWriter writer = new(stream, new() { Indented = options.JsonSerializerOptions.WriteIndented });
            writer.WriteStartArray();
            noIndentScope.WriteNoIndentArrayTo(false, writer, collection, baseEventConverter.Write);
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
        string defaultLevelFile = Path.Combine(directoryPath, "level.json");
        if (File.Exists(defaultLevelFile))
        {
            using FileStream levelFs = File.Open(defaultLevelFile, FileMode.Open, FileAccess.Read);
            Deserializer.DeserializeLevel(new StreamDataSource(levelFs), options, level.Variants.Default, settings);
        }
        foreach (Variant variant in level.Variants)
        {
            if (!string.IsNullOrEmpty(variant.LevelFile))
            {
                string levelFile = Path.Combine(directoryPath, variant.LevelFile);
                if (File.Exists(levelFile))
                {
                    using FileStream levelFsVariant = File.Open(levelFile, FileMode.Open, FileAccess.Read);
                    Deserializer.DeserializeLevel(new StreamDataSource(levelFsVariant), options, variant, settings);
                }
            }
            string chartFile = Path.Combine(directoryPath, $"chart-{variant.Name}.json");
            using FileStream chartFs = File.Open(chartFile, FileMode.Open, FileAccess.Read);
            Deserializer.DeserializeChart(new StreamDataSource(chartFs), options, variant, settings);
        }
        if (Directory.Exists(Path.Combine(directoryPath, "tags")))
        {
            string[] tags = Directory.GetFiles(Path.Combine(directoryPath, "tags"), "*.json");
            foreach (string tagFile in tags)
            {
                TagEventCollection collection = [];
                using FileStream tagFs = File.Open(tagFile, FileMode.Open, FileAccess.Read);
                Deserializer.DeserializeTag(new StreamDataSource(tagFs), options, collection, settings);
                string tagName = Path.GetFileNameWithoutExtension(tagFile);
                level.TagEvents[tagName] = collection;
            }
        }
        return level;
    }
    /// <inheritdoc/>
    public void SaveToDirectory(string directoryPath, ILevelWriteSettings<IBaseEvent, EventType, BBBeat>? settings = null) => SaveToDirectoryAsync(directoryPath, settings).GetAwaiter().GetResult();
    /// <inheritdoc/>
    public async Task SaveToDirectoryAsync(string directoryPath, ILevelWriteSettings<IBaseEvent, EventType, BBBeat>? settings = null, CancellationToken cancellationToken = default)
    {
        settings ??= new LevelWriteSettings();
        RDJsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(directoryPath, settings);
        RDJsonSerializerOptions localOptions = new()
        {
            Type = LevelType.BeatBlock,
            JsonSerializerOptions = new JsonSerializerOptions(options.JsonSerializerOptions)
            { WriteIndented = false }
        };
        using NoIndentScope noIndentScope = new(options.JsonSerializerOptions.Encoder, localOptions);
        string manifestFilePath = Path.Combine(directoryPath, "manifest.json");
        using FileStream manifestFs = File.Open(manifestFilePath, FileMode.Create, FileAccess.Write);
        Deserializer.WriteManifestToStream(manifestFs, this, options);
        string defaultLevelFile = Path.Combine(directoryPath, "level.json");
        using FileStream levelFs = File.Open(defaultLevelFile, FileMode.Create, FileAccess.Write);
        if (this.Variants.Any(i => i.IsUsingDefaultLevel))
            Deserializer.WriteVariantLevelToStream(levelFs, noIndentScope,  this.Variants.Default, options);
        foreach (Variant variant in Variants)
        {
            if (!variant.IsUsingDefaultLevel)
            {
                string levelFile = Path.Combine(directoryPath, variant.LevelFile);
                using FileStream levelFsVariant = File.Open(levelFile, FileMode.Create, FileAccess.Write);
                Deserializer.WriteVariantLevelToStream(levelFsVariant,noIndentScope, variant, options);
            }
            string chartFile = Path.Combine(directoryPath, $"chart-{variant.Name}.json");
            using FileStream chartFs = File.Open(chartFile, FileMode.Create, FileAccess.Write);
            Deserializer.WriteVariantChartsToStream(chartFs,noIndentScope, variant, options);
        }
        if (this.TagEvents.Count > 0)
        {
            string tagsDir = Path.Combine(directoryPath, "tags");
            Directory.CreateDirectory(tagsDir);
            foreach (var tag in TagEvents)
            {
                string tagFile = Path.Combine(tagsDir, $"{tag.Key}.json");
                using FileStream tagFs = File.Open(tagFile, FileMode.Create, FileAccess.Write);
                Deserializer.WriteTagEventsToStream(tagFs,noIndentScope, tag.Value, options);
            }
        }
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
