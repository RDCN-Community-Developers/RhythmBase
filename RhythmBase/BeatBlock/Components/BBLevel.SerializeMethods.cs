using RhythmBase.BeatBlock.Converters;
using RhythmBase.BeatBlock.Events;
using RhythmBase.BeatBlock.Settings;
using RhythmBase.Global.Extensions;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;
using static RhythmBase.BeatBlock.Utils.EventTypeUtils;

namespace RhythmBase.BeatBlock.Components;

partial class BBLevel
{
    private static class Deserializer
    {
        private static readonly BaseEventConverter baseEventConverter = new();
        public static BBLevel DeserializeLevel(IJsonDataSource dataSource, JsonSerializerOptions options)
        {
            ReadOnlyMemory<byte> jsonData =
                dataSource.CanGetMemoryDirectly
                ? dataSource.GetMemory()
                : dataSource.GetMemoryAsync().GetAwaiter().GetResult();
            Utf8JsonReader reader = new(jsonData.Span, new() { AllowTrailingCommas = true });
            return ConverterHub.Read<BBLevel>(ref reader, options) ?? [];
        }
        public static async Task<BBLevel> DeserializeLevelAsync(IJsonDataSource dataSource, JsonSerializerOptions options, CancellationToken token = default)
        {
            ReadOnlyMemory<byte> jsonData =
                 dataSource.CanGetMemoryDirectly
                ? dataSource.GetMemory()
                : await dataSource.GetMemoryAsync(token);
            Utf8JsonReader reader = new(jsonData.Span, new() { AllowTrailingCommas = true });
            return ConverterHub.Read<BBLevel>(ref reader, options) ?? [];
        }
        public static void DeserializeLevel(IJsonDataSource dataSource, JsonSerializerOptions options, BBLevel level, ILevelReadSettings<IBaseEvent, EventType, BBBeat> settings)
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
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    if (propertyName.SequenceEqual("events"u8))
                        foreach (IBaseEvent e in DeserializeEvents(ref reader, options, settings))
                            level.Add(e);
                    else
                    {
                        reader.Skip();
                    }
                }
            }
        }
        public static void DeserializeEvents(IJsonDataSource dataSource, JsonSerializerOptions options, BBLevel level, ILevelReadSettings<IBaseEvent, EventType, BBBeat> settings)
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
                level.Add(e);
            }
            reader.Read();
        }
        private static List<IBaseEvent> DeserializeEvents(ref Utf8JsonReader reader, JsonSerializerOptions options, ILevelReadSettings<IBaseEvent, EventType, BBBeat> settings)
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
    }
    private static void WriteToStream(Stream stream, BBLevel level, JsonSerializerOptions options)
    {
        using Utf8JsonWriter writer = new(stream, new() { Indented = options.WriteIndented });
        ConverterHub.Write(writer, level, options);
        writer.Flush();
    }
    /// <summary>
    /// Loads a <see cref="BBLevel"/> from the specified file path.
    /// </summary>
    /// <param name="filepath">The file path to load from.</param>
    /// <param name="settings">Optional read settings.</param>
    /// <returns>The loaded <see cref="BBLevel"/>.</returns>
    public static BBLevel FromFile(string filepath, ILevelReadSettings<IBaseEvent, EventType, BBBeat>? settings = null)
    {
        settings ??= new LevelReadSettings();
        string extension = Path.GetExtension(filepath);
        BBLevel? level;
        if (extension is not ".zip")
        {
            if (extension is not ".json")
                throw new NotSupportedException($"Unsupported file extension: {extension}");
            string dir = Path.GetDirectoryName(filepath) ?? "";
            string manifestFilePath = Path.Combine(dir, "manifest.json");
            using FileStream manifestFs = File.Open(manifestFilePath, FileMode.Open, FileAccess.Read);
            level = FromStream(manifestFs, dir, settings);
            string levelFile = Path.Combine(dir, "level.json");
            string[] chartFiles = [.. level.Variants.Select(v => Path.Combine(dir, $"chart-{v.Name}.json"))];
            JsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(dir, settings);
            using FileStream levelFs = File.Open(levelFile, FileMode.Open, FileAccess.Read);
            Deserializer.DeserializeLevel(new StreamDataSource(levelFs), options, level, settings);
            foreach (string chartFile in chartFiles)
            {
                using FileStream chartFs = File.Open(chartFile, FileMode.Open, FileAccess.Read);
                Deserializer.DeserializeEvents(new StreamDataSource(chartFs), options, level, settings);
            }
            return level;
        }
        switch (settings.ZipFileProcessMethod)
        {
            case ZipFileProcessMethod.AllFiles:
                DirectoryInfo tempDirectory = GlobalSettings.GetTempDirectory();
                tempDirectory.Create();
                try
                {
#if NET8_0_OR_GREATER
                    using Stream stream = File.OpenRead(filepath);
                    ZipFile.ExtractToDirectory(stream, tempDirectory.FullName, overwriteFiles: true);
#elif NETSTANDARD2_0_OR_GREATER
                    ZipFile.ExtractToDirectory(filepath, tempDirectory.FullName);
#endif
                    string? rdlevelPath = null;
                    foreach (FileInfo? file in tempDirectory.GetFiles())
                    {
                        if (file.Extension == ".rdlevel")
                        {
                            rdlevelPath = file.FullName;
                            break;
                        }
                    }
                    if (rdlevelPath == null)
                        throw new RhythmBaseException("No RDLevel file has been found.");
                    level = FromFile(rdlevelPath, settings);
                    level.ResolvedPath = Path.GetFullPath(rdlevelPath);
                    level.Filepath = Path.GetFullPath(filepath);
                    level.isZip = true;
                    level.isExtracted = true;
                }
                catch (DirectoryNotFoundException)
                {
                    throw;
                }
                catch (FileNotFoundException)
                {
                    throw;
                }
#if !DEBUG
                catch (Exception ex2)
                {
                    tempDirectory.Delete(true);
                    throw new RhythmBaseException("Cannot extract the file.", ex2);
                }
#endif
                break;
            case ZipFileProcessMethod.LevelFileOnly:
                try
                {
                    using FileStream zipStream = new(filepath, FileMode.Open, FileAccess.Read);
                    using ZipArchive archive = new(zipStream, ZipArchiveMode.Read);
                    ZipArchiveEntry? entry = archive.GetEntry("main.rdlevel") ?? throw new RhythmBaseException("Cannot find the level file.");
                    using Stream stream = entry.Open();
                    level = FromStream(stream, settings);
                    level.Filepath = Path.GetFullPath(filepath);
                    level.isZip = true;
                    level.isExtracted = false;
                }
                catch (Exception ex2)
                {
                    throw new RhythmBaseException("Cannot extract the file.", ex2);
                }
                break;
            default:
                throw new RhythmBaseException($"{settings.ZipFileProcessMethod} is not supported.");
        }
        return level;
    }
    /// <summary>
    /// Asynchronously loads a <see cref="BBLevel"/> from the specified file path.
    /// </summary>
    /// <param name="filepath">The file path to load from.</param>
    /// <param name="settings">Optional read settings.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, containing the loaded <see cref="BBLevel"/>.</returns>
    public static Task<BBLevel> FromFileAsync(string filepath, ILevelReadSettings<IBaseEvent, EventType, BBBeat>? settings = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Loads a <see cref="BBLevel"/> from a <see cref="JsonDocument"/>.
    /// </summary>
    /// <param name="jsonDocument">The JSON document to load from.</param>
    /// <param name="settings">Optional read settings.</param>
    /// <returns>The loaded <see cref="BBLevel"/>.</returns>
    public static BBLevel FromJsonDocument(JsonDocument jsonDocument, ILevelReadSettings<IBaseEvent, EventType, BBBeat>? settings = null)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Loads a <see cref="BBLevel"/> from a JSON string.
    /// </summary>
    /// <param name="json">The JSON string to load from.</param>
    /// <param name="settings">Optional read settings.</param>
    /// <returns>The loaded <see cref="BBLevel"/>.</returns>
    public static BBLevel FromJsonString(string json, ILevelReadSettings<IBaseEvent, EventType, BBBeat>? settings = null)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Loads a <see cref="BBLevel"/> from a <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream">The stream to load from.</param>
    /// <param name="settings">Optional read settings.</param>
    /// <returns>The loaded <see cref="BBLevel"/>.</returns>
    public static BBLevel FromStream(Stream stream, ILevelReadSettings<IBaseEvent, EventType, BBBeat>? settings = null)
    {
        throw new NotImplementedException();
    }
    private static BBLevel FromStream(Stream stream, string? dir, ILevelReadSettings<IBaseEvent, EventType, BBBeat>? settings = null)
    {
        settings ??= new LevelReadSettings();
        JsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(dir, settings);
        BBLevel? level;
        settings.OnBeforeReading();
        level = Deserializer.DeserializeLevel(new StreamDataSource(stream), options);
        settings.OnAfterReading();
        return level ?? [];
    }
    /// <summary>
    /// Asynchronously loads a <see cref="BBLevel"/> from a <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream">The stream to load from.</param>
    /// <param name="settings">Optional read settings.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, containing the loaded <see cref="BBLevel"/>.</returns>
    public static Task<BBLevel> FromStreamAsync(Stream stream, ILevelReadSettings<IBaseEvent, EventType, BBBeat>? settings = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Saves the level to the specified file path.
    /// </summary>
    /// <param name="filepath">The file path to save to.</param>
    /// <param name="settings">Optional write settings.</param>
    public void SaveToFile(string filepath, ILevelWriteSettings<IBaseEvent, EventType, BBBeat>? settings = null)
    {
        settings ??= new LevelWriteSettings();
        DirectoryInfo directory = new FileInfo(filepath).Directory ?? new("");
        if (!directory.Exists)
            directory.Create();
        settings.OnBeforeWriting();
        using (FileStream fs = File.Open(filepath, FileMode.OpenOrCreate, FileAccess.Write))
        {
            fs.SetLength(0);
            SaveToStream(fs, settings);
        }
        settings.OnAfterWriting();
    }
    /// <summary>
    /// Asynchronously saves the level to the specified file path.
    /// </summary>
    /// <param name="filepath">The file path to save to.</param>
    /// <param name="settings">Optional write settings.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public void SaveToFileAsync(string filepath, ILevelWriteSettings<IBaseEvent, EventType, BBBeat>? settings = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Saves the level to the specified stream.
    /// </summary>
    /// <param name="stream">The stream to save to.</param>
    /// <param name="settings">Optional write settings.</param>
    public void SaveToStream(Stream stream, ILevelWriteSettings<IBaseEvent, EventType, BBBeat>? settings = null)
    {
        settings ??= new LevelWriteSettings();
        JsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(dir: null, settings: settings);
        settings.OnBeforeWriting();
        WriteToStream(stream, this, options);
        settings.OnAfterWriting();
    }
    /// <summary>
    /// Asynchronously saves the level to the specified stream.
    /// </summary>
    /// <param name="stream">The stream to save to.</param>
    /// <param name="settings">Optional write settings.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public void SaveToStreamAsync(Stream stream, ILevelWriteSettings<IBaseEvent, EventType, BBBeat>? settings = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Saves the level to a zip file at the specified path.
    /// </summary>
    /// <param name="filepath">The file path of the zip file to save to.</param>
    /// <param name="settings">Optional write settings.</param>
    public void SaveToZip(string filepath, ILevelWriteSettings<IBaseEvent, EventType, BBBeat>? settings = null)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Asynchronously saves the level to a zip file at the specified path.
    /// </summary>
    /// <param name="filepath">The file path of the zip file to save to.</param>
    /// <param name="settings">Optional write settings.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public void SaveToZipAsync(string filepath, ILevelWriteSettings<IBaseEvent, EventType, BBBeat>? settings = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Converts the level to a <see cref="JsonDocument"/>.
    /// </summary>
    /// <param name="settings">Optional write settings.</param>
    /// <returns>A <see cref="JsonDocument"/> representing the level.</returns>
    public JsonDocument ToJsonDocument(ILevelWriteSettings<IBaseEvent, EventType, BBBeat>? settings = null)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Converts the level to a JSON string.
    /// </summary>
    /// <param name="settings">Optional write settings.</param>
    /// <returns>A JSON string representing the level.</returns>
    public string ToJsonString(ILevelWriteSettings<IBaseEvent, EventType, BBBeat>? settings = null)
    {
        throw new NotImplementedException();
    }
}
