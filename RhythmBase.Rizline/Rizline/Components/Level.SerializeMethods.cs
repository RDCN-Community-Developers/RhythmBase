using RhythmBase.Rizline.Events;
using System.Text.Json;

namespace RhythmBase.Rizline.Components;

partial class Level
{
    public static LevelType LevelType => LevelType.RhythmDoctor;
    private static class FileConverter
    {
        public static void DeserializeChart(IJsonDataSource dataSource, MetadataJsonSerializerOptions options, Level level, LevelReadSettings settings)
        {
            ReadOnlyMemory<byte> jsonData =
                dataSource.CanGetMemoryDirectly
                ? dataSource.GetMemory()
                : dataSource.GetMemoryAsync().GetAwaiter().GetResult();
            Utf8JsonReader reader = new Utf8JsonReader(jsonData.Span, new() { AllowTrailingCommas = true });
            reader.Read();
            JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartObject]);
            Chart chart = new Chart();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject) break;
                JsonException.ThrowIfNotMatch(reader, [JsonTokenType.PropertyName]);
                ReadOnlySpan<byte> propertyName = reader.ValueSpan;
                reader.Read();
                if (propertyName.SequenceEqual("fileVersion"u8))
                    chart.FileVersion = reader.GetInt32();
                else if (propertyName.SequenceEqual("songsName"u8))
                    chart.SongsName = reader.GetString() ?? "";
                else if (propertyName.SequenceEqual("chartDelayMs"u8))
                    chart.Delay = TimeSpan.FromMilliseconds(reader.GetDouble());
                else if (propertyName.SequenceEqual("offset"u8))
                    chart.Offset = TimeSpan.FromMilliseconds(reader.GetDouble());
                else if (propertyName.SequenceEqual("themes"u8))
                {
                    JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
                    reader.Read();
                    chart.Themes.MainTheme = ConverterHub.Read<Theme>(ref reader, options);
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                        chart.Themes.RiztimeThemes.Add(ConverterHub.Read<Theme>(ref reader, options));
                    reader.Read();
                }
                else if (propertyName.SequenceEqual("challengeTimes"u8))
                {
                    JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                        chart.ChallengeTimes.Add(ConverterHub.Read<ChallengeTime>(ref reader, options));
                }
                else if (propertyName.SequenceEqual("bPM"u8))
                    chart.Bpm = reader.GetSingle();
                //else if (propertyName.SequenceEqual("bpmShifts"u8))
                //    chart.BpmShifts = JsonSerializer.Deserialize<List<KeyPoint>>(ref reader, options);
                //else if (propertyName.SequenceEqual("lines"u8))
                //    chart.Lines = JsonSerializer.Deserialize<List<Line>>(ref reader, options);
                //else if (propertyName.SequenceEqual("canvasMoves"u8))
                //    chart.CanvasMoves = JsonSerializer.Deserialize<List<CanvasMove>>(ref reader, options);
                else
                    reader.Skip();
            }
        }
    }
    #region zip
    /// <inheritdoc/>
    public static Level FromZip(string filepath, LevelReadSettings? settings = null)
        => FromZipAsync(filepath, settings).GetAwaiter().GetResult();
    /// <inheritdoc/>
    public static Task<Level> FromZipAsync(string filepath, LevelReadSettings? settings = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    /// <inheritdoc/>
    public void SaveToZip(string filepath, LevelWriteSettings? settings = null)
        => SaveToZipAsync(filepath, settings).GetAwaiter().GetResult();
    /// <inheritdoc/>
    public Task SaveToZipAsync(string filepath, LevelWriteSettings? settings = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    #endregion
    #region json
    /// <inheritdoc/>
    public static Level FromJsonDocument(JsonDocument jsonDocument, LevelReadSettings? settings = null)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public static Level FromJsonString(string json, LevelReadSettings? settings = null)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public JsonDocument ToJsonDocument(LevelWriteSettings? settings = null)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public string ToJsonString(LevelWriteSettings? settings = null)
    {
        throw new NotImplementedException();
    }
    #endregion
    #region dir
    /// <inheritdoc/>
    public static Level FromDirectory(string directoryPath, LevelReadSettings? settings = null)
        => FromDirectoryAsync(directoryPath, settings).GetAwaiter().GetResult();
    /// <inheritdoc/>
    public static async Task<Level> FromDirectoryAsync(string directoryPath, LevelReadSettings? settings = null, CancellationToken cancellationToken = default)
    {
        settings ??= new LevelReadSettings();
        using FileStream metadataFs = new(Path.Combine(directoryPath, "metadata.json"), FileMode.Open, FileAccess.Read);
        MetadataJsonSerializerOptions options = JsonSerializerOptionsUtils.GetJsonSerializerOptionsForRead(LevelType, settings);
        Level level = await FileMainEntryConverter.DeserializeMainEntryAsync<Level>(new StreamDataSource(metadataFs), options, cancellationToken);
        string[] jsonFiles = Directory.GetFiles(directoryPath, "*.json", SearchOption.AllDirectories);
        foreach (string jsonFile in jsonFiles)
        {
            if (jsonFile == "metadata.json") continue;
            using FileStream jsonFs = new(jsonFile, FileMode.Open, FileAccess.Read);

        }
        return level;
    }
    /// <inheritdoc/>
    public void SaveToDirectory(string directoryPath, LevelWriteSettings? settings = null)
        => SaveToDirectoryAsync(directoryPath, settings).GetAwaiter().GetResult();
    /// <inheritdoc/>
    public Task SaveToDirectoryAsync(string directoryPath, LevelWriteSettings? settings = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    #endregion
}