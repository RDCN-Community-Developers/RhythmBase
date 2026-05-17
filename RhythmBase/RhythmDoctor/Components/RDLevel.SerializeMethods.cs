using RhythmBase.RhythmDoctor.Events;
using RhythmBase.RhythmDoctor.Settings;
using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace RhythmBase.RhythmDoctor.Components;

partial class RDLevel
{
    private static class Deserializer
    {
        public static RDLevel Deserialize(IJsonDataSource dataSource, JsonSerializerOptions options)
        {
            if (dataSource.CanGetMemoryDirectly)
            {
                ReadOnlyMemory<byte> jsonData = dataSource.GetMemory();
                Utf8JsonReader reader = new(jsonData.Span, new() { AllowTrailingCommas = true });
                return ConverterHub.Read<RDLevel>(ref reader, options) ?? [];
            }
            else
            {
                ReadOnlyMemory<byte> jsonData = dataSource.GetMemoryAsync().GetAwaiter().GetResult();
                Utf8JsonReader reader = new(jsonData.Span, new() { AllowTrailingCommas = true });
                return ConverterHub.Read<RDLevel>(ref reader, options) ?? [];
            }
        }
        public static async Task<RDLevel> DeserializeAsync(IJsonDataSource dataSource, JsonSerializerOptions options, CancellationToken token = default)
        {
            if (dataSource.CanGetMemoryDirectly)
            {
                ReadOnlyMemory<byte> jsonData = dataSource.GetMemory();
                Utf8JsonReader reader = new(jsonData.Span, new() { AllowTrailingCommas = true });
                return ConverterHub.Read<RDLevel>(ref reader, options) ?? [];
            }
            else
            {
                ReadOnlyMemory<byte> jsonData = await dataSource.GetMemoryAsync(token);
                Utf8JsonReader reader = new(jsonData.Span, new() { AllowTrailingCommas = true });
                return ConverterHub.Read<RDLevel>(ref reader, options) ?? [];
            }
        }
    }

    private static void WriteToStream(Stream stream, RDLevel level, JsonSerializerOptions options)
    {
        Utf8JsonWriter writer = new(stream, new JsonWriterOptions { Indented = options.WriteIndented });
        ConverterHub.Write(writer, level, options);
        writer.Flush();
    }
    /// <inheritdoc/>
    public static RDLevel FromFile(string filepath, ILevelReadSettings<IBaseEvent, EventType, RDBeat>? settings = null)
    {
        settings ??= new LevelReadSettings();
        string extension = Path.GetExtension(filepath);
        RDLevel? level;
        if (extension is not ".rdzip" and not ".zip")
        {
            if (extension is not ".rdlevel" and not ".json")
                throw new RhythmBaseException("File not supported.");
            using FileStream stream = File.Open(filepath, FileMode.Open, FileAccess.Read);
            level = FromStream(stream, Path.GetDirectoryName(Path.GetFullPath(filepath)) ?? "", settings);
            level.Filepath = level.ResolvedPath = Path.GetFullPath(filepath);
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
    /// <inheritdoc/>
    public static async Task<RDLevel> FromFileAsync(string filepath, ILevelReadSettings<IBaseEvent, EventType, RDBeat>? settings = null, CancellationToken cancellationToken = default)
    {
        settings ??= new LevelReadSettings();
        string extension = Path.GetExtension(filepath);
        RDLevel? level;
        if (extension is not ".rdzip" and not ".zip")
        {
            if (extension is not ".rdlevel" and not ".json")
                throw new RhythmBaseException("File not supported.");
            using FileStream stream = File.Open(filepath, FileMode.Open, FileAccess.Read);
            level = await FromStreamAsync(stream, settings, cancellationToken);
            level.Filepath = level.ResolvedPath = Path.GetFullPath(filepath);
        }
        switch (settings.ZipFileProcessMethod)
        {
            case ZipFileProcessMethod.AllFiles:
                DirectoryInfo tempDirectory = new(Path.Combine(GlobalSettings.CachePath, "RhythmBaseTemp_Zip_" + Path.GetRandomFileName()));
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
                        if (file.Name == "main.rdlevel")
                        {
                            rdlevelPath = file.FullName;
                            break;
                        }
                    }
                    if (rdlevelPath == null)
                        throw new RhythmBaseException("No RDLevel file has been found.");
                    level = await FromFileAsync(rdlevelPath, settings, cancellationToken);
                    level.ResolvedPath = Path.GetFullPath(filepath);
                    level.Filepath = Path.GetFullPath(rdlevelPath);
                    level.isZip = true;
                    level.isExtracted = true;
                }
                catch (Exception ex2)
                {
                    tempDirectory.Delete(true);
                    throw new RhythmBaseException("Cannot extract the file.", ex2);
                }
                break;
            case ZipFileProcessMethod.LevelFileOnly:
                try
                {
                    using FileStream zipStream = new(filepath, FileMode.Open, FileAccess.Read);
                    using ZipArchive archive = new(zipStream, ZipArchiveMode.Read);
                    ZipArchiveEntry? entry = archive.GetEntry("main.rdlevel") ?? throw new RhythmBaseException("Cannot find the level file.");
                    using Stream stream = entry.Open();
                    level = await FromStreamAsync(stream, settings, cancellationToken);
                    level.ResolvedPath = Path.GetFullPath(filepath);
                    level.isZip = true;
                    level.isExtracted = false;
                }
                catch (Exception ex2)
                {
                    throw new RhythmBaseException("Cannot extract the file.", ex2);
                }
                break;
            default:
                throw new RhythmBaseException(extension + " is not supported.");
        }
        return level;
    }
    private static async Task<RDLevel> DeserializeAsync(ReadOnlyMemory<byte> rdlevelJson, JsonSerializerOptions options, CancellationToken token = default)
    {
        Utf8JsonReader reader = new(rdlevelJson.Span, new() { AllowTrailingCommas = true });
        return ConverterHub.Read<RDLevel>(ref reader, options) ?? [];
    }
    /// <inheritdoc/>
    public static RDLevel FromStream(Stream rdlevelStream, ILevelReadSettings<IBaseEvent, EventType, RDBeat>? settings = null)
    {
        settings ??= new LevelReadSettings();
        JsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(settings: settings);
        RDLevel? level;
        settings.OnBeforeReading();
        level = Deserializer.Deserialize(new StreamDataSource(rdlevelStream), options);
        settings.OnAfterReading();
        return level ?? [];
    }
    private static RDLevel FromStream(Stream rdlevelStream, string dirPath, ILevelReadSettings<IBaseEvent, EventType, RDBeat>? settings = null)
    {
        settings ??= new LevelReadSettings();
        JsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(dirPath, settings);
        RDLevel? level;
        settings.OnBeforeReading();
        level = Deserializer.Deserialize(new StreamDataSource(rdlevelStream), options);
        settings.OnAfterReading();
        return level ?? [];
    }
    /// <inheritdoc/>
    public static async Task<RDLevel> FromStreamAsync(Stream rdlevelStream, ILevelReadSettings<IBaseEvent, EventType, RDBeat>? settings = null, CancellationToken cancellationToken = default)
    {
        settings ??= new LevelReadSettings();
        JsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(settings: settings);
        RDLevel? level;
        settings.OnBeforeReading();
        level = Deserializer.DeserializeAsync(new StreamDataSource(rdlevelStream), options, cancellationToken).GetAwaiter().GetResult();
        settings.OnAfterReading();
        return level ?? [];
    }
    /// <inheritdoc/>
    public static RDLevel FromJsonString(string json, ILevelReadSettings<IBaseEvent, EventType, RDBeat>? settings = null)
    {
        settings ??= new LevelReadSettings();
        JsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(settings: settings);
        RDLevel? level;
        settings.OnBeforeReading();
        level = Deserializer.Deserialize(new ReadOnlyMemoryDataSource(new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(json))), options);
        settings.OnAfterReading();
        return level ?? [];
    }
    /// <inheritdoc/>
    public static RDLevel FromJsonDocument(JsonDocument jsonDocument, ILevelReadSettings<IBaseEvent, EventType, RDBeat>? settings = null)
    {
        settings ??= new LevelReadSettings();
        JsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(settings: settings);
        RDLevel? level;
        settings.OnBeforeReading();
        level = Deserializer.Deserialize(new JsonDocumentDataSource(jsonDocument), options);
        settings.OnAfterReading();
        return level ?? [];
    }
    /// <inheritdoc/>
    public void SaveToStream(Stream stream, ILevelWriteSettings<IBaseEvent, EventType, RDBeat>? settings = null)
    {
        settings ??= new LevelWriteSettings();
        JsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(settings: settings);
        settings.OnBeforeWriting();
        WriteToStream(stream, this, options);
        settings.OnAfterWriting();
    }
    /// <inheritdoc/>
    public async void SaveToStreamAsync(Stream stream, ILevelWriteSettings<IBaseEvent, EventType, RDBeat>? settings = null, CancellationToken cancellationToken = default)
    {
        settings ??= new LevelWriteSettings();
        JsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(settings: settings);
        settings.OnBeforeWriting();
        await Task.Run(() => WriteToStream(stream, this, options), cancellationToken);
        settings.OnAfterWriting();
    }
    /// <inheritdoc/>
    public void SaveToFile(string filepath, ILevelWriteSettings<IBaseEvent, EventType, RDBeat>? settings = null)
    {
        settings ??= new LevelWriteSettings();
        DirectoryInfo directory = new FileInfo(filepath).Directory ?? new("");
        if (!directory.Exists)
            directory.Create();
        settings.OnBeforeWriting();
        using (FileStream stream = File.Open(filepath, FileMode.OpenOrCreate, FileAccess.Write))
        {
            stream.SetLength(0);
            SaveToStream(stream, settings);
        }
        settings.OnAfterWriting();
    }
    /// <inheritdoc/>
    public async void SaveToFileAsync(string filepath, ILevelWriteSettings<IBaseEvent, EventType, RDBeat>? settings = null, CancellationToken cancellationToken = default)
    {
        settings ??= new LevelWriteSettings();
        JsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(Path.GetDirectoryName(filepath) ?? "", settings);
        DirectoryInfo directory = new FileInfo(filepath).Directory ?? new("");
        if (!directory.Exists)
            directory.Create();
        settings.OnBeforeWriting();
        using (FileStream stream = File.Open(filepath, FileMode.OpenOrCreate, FileAccess.Write))
        {
            stream.SetLength(0);
            await Task.Run(() => SaveToStream(stream, settings), cancellationToken);
        }
        settings.OnAfterWriting();
    }
    /// <inheritdoc/>
    public void SaveToZip(string filepath, ILevelWriteSettings<IBaseEvent, EventType, RDBeat>? settings = null)
    {
        if (string.IsNullOrEmpty(this.ResolvedDirectory))
            throw new NotImplementedException();
        settings ??= new LevelWriteSettings();
        settings.FileReferences.Clear();
        bool loadAssets = settings.LoadAssets;
        settings.LoadAssets = true;
        DirectoryInfo directory = new FileInfo(filepath).Directory ?? new("");
        if (!directory.Exists)
            directory.Create();
        settings.OnBeforeWriting();
        using Stream stream = new FileStream(filepath, FileMode.Create, FileAccess.Write);
        ZipArchive archive = new(stream, ZipArchiveMode.Create);
        ZipArchiveEntry entry = archive.CreateEntry("main.rdlevel");
        using (Stream rdlevelStream = entry.Open())
        {
            SaveToStream(rdlevelStream, settings);
        }
        foreach (var file in settings.FileReferences)
        {
            archive.CreateEntryFromFile(Path.Combine(ResolvedDirectory, file.Path), Path.GetFileName(file.Path));
        }
        archive.Dispose();
        settings.OnAfterWriting();
        settings.LoadAssets = loadAssets;
    }
    /// <inheritdoc/>
    public async void SaveToZipAsync(string filepath, ILevelWriteSettings<IBaseEvent, EventType, RDBeat>? settings = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(this.ResolvedDirectory))
            throw new NotImplementedException();
        settings ??= new LevelWriteSettings();
        settings.FileReferences.Clear();
        bool loadAssets = settings.LoadAssets;
        settings.LoadAssets = true;
        JsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(Path.GetDirectoryName(Filepath) ?? "", settings);
        DirectoryInfo directory = new FileInfo(filepath).Directory ?? new("");
        if (!directory.Exists)
            directory.Create();
        settings.OnBeforeWriting();
        using Stream stream = new FileStream(filepath, FileMode.Create, FileAccess.Write);
        ZipArchive archive = new(stream, ZipArchiveMode.Create);
        ZipArchiveEntry entry = archive.CreateEntry("main.rdlevel");
        using (Stream rdlevelStream = entry.Open())
        {
            await Task.Run(() => SaveToStream(rdlevelStream, settings), cancellationToken);
        }
        foreach (var file in settings.FileReferences)
        {
            archive.CreateEntryFromFile(Path.Combine(ResolvedDirectory, file.Path), Path.GetFileName(file.Path));
        }
        archive.Dispose();
        settings.OnAfterWriting();
        settings.LoadAssets = loadAssets;
    }
    /// <summary>
    /// Serializes the current level to a JSON string.
    /// </summary>
    /// <param name="settings">Optional settings for writing the level. If null, default settings are used.</param>
    /// <returns>A JSON string representing the current level.</returns>
    public string ToJsonString(ILevelWriteSettings<IBaseEvent, EventType, RDBeat>? settings = null)
    {
        settings ??= new LevelWriteSettings();
        JsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(settings: settings);
        string json;
        settings.OnBeforeWriting();
        using (MemoryStream stream = new())
        {
            SaveToStream(stream, settings);
            stream.Seek(0, SeekOrigin.Begin);
            json = Encoding.UTF8.GetString(stream.ToArray());
        }
        settings.OnAfterWriting();
        return json;
    }
    /// <summary>
    /// Serializes the current level to a <see cref="JsonDocument"/>.
    /// </summary>
    /// <param name="settings">
    /// Optional settings for writing the level. If <c>null</c>, default settings are used.
    /// </param>
    /// <returns>
    /// A <see cref="JsonDocument"/> representing the current level in JSON format.
    /// </returns>
    public JsonDocument ToJsonDocument(ILevelWriteSettings<IBaseEvent, EventType, RDBeat>? settings = null)
    {
        settings ??= new LevelWriteSettings();
        string json;
        settings.OnBeforeWriting();
        MemoryStream stream = new();
        SaveToStream(stream, settings);
        stream.Seek(0, SeekOrigin.Begin);
        json = Encoding.UTF8.GetString(stream.ToArray());
        settings.OnAfterWriting();
        return JsonDocument.Parse(json);
    }
}
