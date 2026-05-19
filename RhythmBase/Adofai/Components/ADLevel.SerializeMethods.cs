using RhythmBase.Adofai.Events;
using RhythmBase.Adofai.Settings;
using RhythmBase.RhythmDoctor.Components;
using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace RhythmBase.Adofai.Components;

partial class ADLevel
{
    internal static class Deserializer
    {
        public static ADLevel Deserialize(IJsonDataSource dataSource, JsonSerializerOptions options)
        {
            if (dataSource.CanGetMemoryDirectly)
            {
                ReadOnlyMemory<byte> jsonData = dataSource.GetMemory();
                Utf8JsonReader reader = new(jsonData.Span, new() { AllowTrailingCommas = true });
                return ConverterHub.Read<ADLevel>(ref reader, options) ?? [];
            }
            else
            {
                ReadOnlyMemory<byte> jsonData = dataSource.GetMemoryAsync().GetAwaiter().GetResult();
                Utf8JsonReader reader = new(jsonData.Span, new() { AllowTrailingCommas = true });
                return ConverterHub.Read<ADLevel>(ref reader, options) ?? [];
            }
        }
        public static async Task<ADLevel> DeserializeAsync(IJsonDataSource dataSource, JsonSerializerOptions options, CancellationToken token = default)
        {
            if (dataSource.CanGetMemoryDirectly)
            {
                ReadOnlyMemory<byte> jsonData = dataSource.GetMemory();
                Utf8JsonReader reader = new(jsonData.Span, new() { AllowTrailingCommas = true });
                return ConverterHub.Read<ADLevel>(ref reader, options) ?? [];
            }
            else
            {
                ReadOnlyMemory<byte> jsonData = await dataSource.GetMemoryAsync(token);
                Utf8JsonReader reader = new(jsonData.Span, new() { AllowTrailingCommas = true });
                return ConverterHub.Read<ADLevel>(ref reader, options) ?? [];
            }
        }
    }
    private static void WriteToStream(Stream stream, ADLevel level, JsonSerializerOptions options)
    {
        Utf8JsonWriter writer = new(stream, new JsonWriterOptions { Indented = options.WriteIndented });
        ConverterHub.Write(writer, level, options);
        writer.Flush();
    }
    /// <inheritdoc/>
    public static ADLevel FromFile(string filepath, ILevelReadSettings<IBaseEvent, EventType, ADBeat>? settings = null)
    {
        settings ??= new LevelReadSettings();
        string extension = Path.GetExtension(filepath);
        ADLevel? level;
        if (extension != ".zip")
        {
            if (extension != ".adofai")
                throw new RhythmBaseException("File not supported.");
            using FileStream stream = File.Open(filepath, FileMode.Open, FileAccess.Read);
            level = FromStream(stream, settings);
            level.Filepath = level.ResolvedPath = Path.GetFullPath(filepath);
            return level;
        }
        switch (settings.ZipFileProcessMethod)
        {
            case ZipFileProcessMethod.AllFiles:
                DirectoryInfo tempDirectory = new(Path.Combine(GlobalSettings.CachePath, "RhythmBaseTemp_" + Path.GetRandomFileName()));
                tempDirectory.Create();
                try
                {
#if NET8_0_OR_GREATER
                    using Stream stream = File.OpenRead(filepath);
                    ZipFile.ExtractToDirectory(stream, tempDirectory.FullName, overwriteFiles: true);
#elif NETSTANDARD2_0_OR_GREATER
                    ZipFile.ExtractToDirectory(filepath, tempDirectory.FullName);
#endif
                    string? adlevelPath = null;
                    foreach (FileInfo file in tempDirectory.GetFiles())
                    {
                        if (file.Extension == ".adofai")
                        {
                            adlevelPath = file.FullName;
                            break;
                        }
                    }
                    if (adlevelPath == null)
                        throw new RhythmBaseException("No Adofai file has been found.");
                    level = FromFile(adlevelPath, settings);
                    level.ResolvedPath =Path.GetFullPath(adlevelPath);
                    level.Filepath =  Path.GetFullPath(filepath);
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
                throw new RhythmBaseException(extension + " is not supported.");
        }
        return level;
    }
    /// <inheritdoc/>
    public static async Task<ADLevel> FromFileAsync(string filepath, ILevelReadSettings<IBaseEvent, EventType, ADBeat>? settings = null, CancellationToken cancellationToken = default)
    {
        settings ??= new LevelReadSettings();
        string extension = Path.GetExtension(filepath);
        ADLevel? level;
        if (extension != ".zip")
        {
            if (extension != ".adofai")
                throw new RhythmBaseException("File not supported.");
            using FileStream stream = File.Open(filepath, FileMode.Open, FileAccess.Read);
            level = FromStream(stream, settings);
            level.Filepath = level.ResolvedPath = Path.GetFullPath(filepath);
            return level;
        }
        switch (settings.ZipFileProcessMethod)
        {
            case ZipFileProcessMethod.AllFiles:
                DirectoryInfo tempDirectory = new(Path.Combine(GlobalSettings.CachePath, "RhythmBaseTemp_" + Path.GetRandomFileName()));
                tempDirectory.Create();
                try
                {
#if NET8_0_OR_GREATER
                    using Stream stream = File.OpenRead(filepath);
                    ZipFile.ExtractToDirectory(stream, tempDirectory.FullName, overwriteFiles: true);
#elif NETSTANDARD2_0_OR_GREATER
                    ZipFile.ExtractToDirectory(filepath, tempDirectory.FullName);
#endif
                    string? adlevelPath = null;
                    foreach (FileInfo file in tempDirectory.GetFiles())
                    {
                        if (file.Extension == ".adofai")
                        {
                            adlevelPath = file.FullName;
                            break;
                        }
                    }
                    if (adlevelPath == null)
                        throw new RhythmBaseException("No Adofai file has been found.");
                    level = FromFile(adlevelPath, settings);
                    level.ResolvedPath = Path.GetFullPath(adlevelPath);
                    level.Filepath = Path.GetFullPath(filepath);
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
                throw new RhythmBaseException(extension + " is not supported.");
        }
        return level;
    }
    /// <inheritdoc/>
    public static ADLevel FromStream(Stream adlevelStream, ILevelReadSettings<IBaseEvent, EventType, ADBeat>? settings = null)
    {
        settings ??= new LevelReadSettings();
        JsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(settings: settings);
        ADLevel? level;
        level = Deserializer.Deserialize(new StreamDataSource(adlevelStream), options);
        return level ?? [];
    }
    /// <inheritdoc/>
    public static async Task<ADLevel> FromStreamAsync(Stream stream, ILevelReadSettings<IBaseEvent, EventType, ADBeat>? settings = null, CancellationToken cancellationToken = default)
    {
        settings ??= new LevelReadSettings();
        JsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(settings: settings);
        ADLevel? level;
        level = await Deserializer.DeserializeAsync(new StreamDataSource(stream), options, cancellationToken);
        return level ?? [];
    }
    /// <inheritdoc/>
    public static ADLevel FromJsonString(string json, ILevelReadSettings<IBaseEvent, EventType, ADBeat>? settings = null)
    {
        settings ??= new LevelReadSettings();
        JsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(settings: settings);
        ADLevel? level;
        level = Deserializer.Deserialize(new ReadOnlyMemoryDataSource(new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(json))), options);
        return level ?? [];
    }
    /// <inheritdoc/>
    public void SaveToStream(Stream stream, ILevelWriteSettings<IBaseEvent, EventType, ADBeat>? settings = null)
    {
        settings ??= new LevelWriteSettings();
        JsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(settings: settings);
        WriteToStream(stream, this, options);
    }
    /// <inheritdoc/>
    public async void SaveToStreamAsync(Stream stream, ILevelWriteSettings<IBaseEvent, EventType, ADBeat>? settings = null, CancellationToken cancellationToken = default)
    {
        settings ??= new LevelWriteSettings();
        JsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(settings: settings);
        await Task.Run(() => WriteToStream(stream, this, options), cancellationToken);
    }
    /// <inheritdoc/>
    public void SaveToFile(string filepath, ILevelWriteSettings<IBaseEvent, EventType, ADBeat>? settings = null)
    {
        settings ??= new LevelWriteSettings();
        JsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(filepath, settings);
        using (FileStream stream = File.Open(filepath, FileMode.OpenOrCreate, FileAccess.Write))
        {
            stream.SetLength(0);
            WriteToStream(stream, this, options);
        }
    }
    /// <inheritdoc/>
    public async void SaveToFileAsync(string filepath, ILevelWriteSettings<IBaseEvent, EventType, ADBeat>? settings = null, CancellationToken cancellationToken = default)
    {
        settings ??= new LevelWriteSettings();
        JsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(filepath, settings);
        using (FileStream stream = File.Open(filepath, FileMode.OpenOrCreate, FileAccess.Write))
        {
            await Task.Run(() => WriteToStream(stream, this, options), cancellationToken);
        }
    }
    /// <inheritdoc/>
    public string ToJsonString(ILevelWriteSettings<IBaseEvent, EventType, ADBeat>? settings = null)
    {
        settings ??= new LevelWriteSettings();
        JsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(settings: settings);
        string json;
        using (MemoryStream stream = new())
        {
            WriteToStream(stream, this, options);
            stream.Seek(0, SeekOrigin.Begin);
            json = Encoding.UTF8.GetString(stream.ToArray());
        }
        return json;
    }
    /// <inheritdoc/>
    public static ADLevel FromJsonDocument(JsonDocument jsonDocument, ILevelReadSettings<IBaseEvent, EventType, ADBeat>? settings = null)
    {
        settings ??= new LevelReadSettings();
        JsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(settings: settings);
        ADLevel? level;
        level = Deserializer.Deserialize(new JsonDocumentDataSource(jsonDocument), options);
        return level ?? [];
    }
    /// <inheritdoc/>
    public JsonDocument ToJsonDocument(ILevelWriteSettings<IBaseEvent, EventType, ADBeat>? settings = null)
    {
        settings ??= new LevelWriteSettings();
        string json;
        MemoryStream stream = new();
        SaveToStream(stream, settings);
        stream.Seek(0, SeekOrigin.Begin);
        json = Encoding.UTF8.GetString(stream.ToArray());
        return JsonDocument.Parse(json);
    }
    /// <inheritdoc/>
    public void SaveToZip(string filepath, ILevelWriteSettings<IBaseEvent, EventType, ADBeat>? settings = null)
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
        using Stream stream = new FileStream(filepath, FileMode.Create, FileAccess.Write);
        ZipArchive archive = new(stream, ZipArchiveMode.Create);
        ZipArchiveEntry entry = archive.CreateEntry("main.adofai");
        using (Stream rdlevelStream = entry.Open())
        {
            SaveToStream(rdlevelStream, settings);
        }
        foreach (var file in settings.FileReferences)
        {
            archive.CreateEntryFromFile(Path.Combine(ResolvedDirectory, file.Path), Path.GetFileName(file.Path));
        }
        archive.Dispose();
        settings.LoadAssets = loadAssets;
    }
    /// <inheritdoc/>
    public void SaveToZipAsync(string filepath, ILevelWriteSettings<IBaseEvent, EventType, ADBeat>? settings = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

}