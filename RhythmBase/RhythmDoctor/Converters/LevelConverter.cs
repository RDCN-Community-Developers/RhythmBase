using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Events;
using RhythmBase.RhythmDoctor.Settings;
using RhythmBase.RhythmDoctor.Utils;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.RhythmDoctor.Converters;

[RDJsonConverterFor(typeof(RDLevel))]
internal sealed class LevelConverter : RDJsonConverter<RDLevel>
{
    private static readonly SettingsConverter settingsConverter = new();
    private static readonly RowConverter rowConverter = new();
    private static readonly DecorationConverter decorationConverter = new();
    private static readonly BaseEventConverter baseEventConverter = new();
    private static readonly BookmarkConverter bookmarkConverter = new();
    private static readonly ConditionalConverter conditionalConverter = new();
    internal ILevelReadSettings<IBaseEvent, EventType, RDBeat> ReadSettings { get; set; } = new LevelReadSettings();
    internal ILevelWriteSettings<IBaseEvent, EventType, RDBeat> WriteSettings { get; set; } = new LevelWriteSettings();
    internal string? DirectoryName { get; set; }
    public override RDLevel? Read(ref Utf8JsonReader reader, Type typeToConvert, RDJsonSerializerOptions options)
    {
        reader.Read();
        baseEventConverter.WithReadSettings(ReadSettings);
        RDLevel level = [];
        ReadSettings.FileReferences.Clear();
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartObject]);
        reader.Read();
        while (true)
        {

            if (reader.TokenType == JsonTokenType.EndObject)
                break;
            JsonException.ThrowIfNotMatch(reader, [JsonTokenType.PropertyName]);
            if (reader.ValueSpan.SequenceEqual("settings"u8))
            {
                reader.Read();
                JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartObject]);
                level.Settings = settingsConverter.Read(ref reader, typeof(Components.Settings), options) ?? new();
                if (ReadSettings.LoadAssets && !string.IsNullOrEmpty(DirectoryName))
                    foreach (FileReference file in level.Settings.GetAllFileReferences())
                        if (!file.IsEmpty && file.IsExist(DirectoryName!))
                            ReadSettings.FileReferences.Add(file);
                if (level.Settings.Version < MinimumSupportedVersionRhythmDoctor)
#if DEBUG
                    Console.WriteLine($"Current version {level.Settings.Version} is too low.");
#else
                    throw new VersionTooLowException(MinimumSupportedVersionRhythmDoctor);
#endif
            }
            else if (reader.ValueSpan.SequenceEqual("rows"u8))
            {
                reader.Read();
                JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                        break;
                    Row? e = rowConverter.Read(ref reader, typeof(Row), options);
                    if (e != null)
                    {
                        level.Rows.Add(e);
                        string assPath = DirectoryName + e.Character.CustomCharacter;
                        if (ReadSettings.LoadAssets && !string.IsNullOrEmpty(DirectoryName))
                            foreach (FileReference file in e.Character.GetAllPossibleFileReferences())
                                if (!file.IsEmpty && file.IsExist(DirectoryName!))
                                    ReadSettings.FileReferences.Add(file);
                                else if (file.IsExist(assPath))
                                    ReadSettings.FileReferences.Add(DirectoryName + Path.DirectorySeparatorChar + file);
                    }
                }
                reader.Read();
            }
            else if (reader.ValueSpan.SequenceEqual("decorations"u8))
            {
                reader.Read();
                JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                        break;
                    Decoration? e = decorationConverter.Read(ref reader, typeof(Decoration), options);
                    if (e != null)
                    {
                        level.Decorations.Add(e);
                        string assPath = DirectoryName + e.Character.CustomCharacter;
                        if (ReadSettings.LoadAssets && !string.IsNullOrEmpty(DirectoryName))
                            foreach (FileReference file in e.Character.GetAllPossibleFileReferences())
                                if (!file.IsEmpty && file.IsExist(DirectoryName!))
                                    ReadSettings.FileReferences.Add(file);
                                else if (file.IsExist(assPath))
                                    ReadSettings.FileReferences.Add(DirectoryName + Path.DirectorySeparatorChar + file);
                    }
                }
                reader.Read();
            }
            else if (reader.ValueSpan.SequenceEqual("events"u8))
            {
                reader.Read();
                JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
                Dictionary<int, FloatingText> floatingTexts = [];
                List<AdvanceText> advanceTexts = [];
                JsonElement[]? data = [];
                List<JsonDocument> maybeIllegalAt = [];
                reader.Read();
                int index = 0;
                while (true)
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
                    catch (Exception)
                    {
                        Debug.Print($"Current index: {index}");
                        throw;
                    }
#else
                    Utf8JsonReader checkPoint = reader;
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
                        JsonElement element = JsonElement.ParseValue(ref checkPoint);
                        ReadSettings.HandleUnreadableEvent(element, ex.Message);
                        continue;
                    }
#endif
                    if (e == null)
                        continue;
                    level.Add(e);
                    if (e is FloatingText ft)
                    {
                        JsonElement v1 = ft["id"];
                        int v2 = v1.GetInt32();
                        floatingTexts[v2] = ft;
                        ft._extraData.Remove("id");
                    }
                    else if (e is AdvanceText at)
                        advanceTexts.Add(at);
                    if (e is IFileEvent fe)
                    {
                        if (ReadSettings.LoadAssets && !string.IsNullOrEmpty(DirectoryName))
                            foreach (FileReference file in fe.Files)
                                if (!file.IsEmpty && file.IsExist(DirectoryName!))
                                    ReadSettings.FileReferences.Add(file);
                    }
                }
                reader.Read();
                foreach (AdvanceText? at in advanceTexts)
                {
                    int targetId = at["id"].GetInt32();
                    if (floatingTexts.TryGetValue(targetId, out FloatingText? ft))
                    {
                        at.Parent = ft;
                        ft.Children.Add(at);
                        at._extraData.Remove("id");
                    }
                    else
                    {
                        ReadSettings.HandleUnreadableEvent(JsonElement.Parse(at.ToJsonString()), $"AdvanceText references non-existent FloatingText id {targetId}.");
                    }
                }
            }
            else if (reader.ValueSpan.SequenceEqual("bookmarks"u8))
            {
                reader.Read();
                JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
                reader.Read();
                while (true)
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                        break;
                    Bookmark e = bookmarkConverter.Read(ref reader, typeof(Bookmark), options);
                    level.Bookmarks.Add(e);
                    reader.Read();
                }
                reader.Read();
            }
            else if (reader.ValueSpan.SequenceEqual("colorPalette"u8))
            {
                reader.Read();
                JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
                int colorIndex = 0;
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                    {
                        break;
                    }
                    string? e = reader.GetString();
                    RDColor color = e is null ? default : RDColor.FromRgba(e);
                    level.ColorPalette[colorIndex] = color;
                    colorIndex++;
                }
                reader.Read();
            }
            else if (reader.ValueSpan.SequenceEqual("conditionals"u8))
            {
                reader.Read();
                JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
                reader.Read();
                while (true)
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                        break;
                    BaseConditional? e = conditionalConverter.Read(ref reader, typeof(BaseConditional), options);
                    if (e != null)
                        level.Conditionals.Add(e);
                }
                reader.Read();
            }
            else
            {
                reader.Skip();
            }
        }
        return level;
    }
    public override void Write(Utf8JsonWriter writer, RDLevel value, RDJsonSerializerOptions options)
    {
        baseEventConverter.WithWriteSettings(WriteSettings);
        using MemoryStream stream = new();
        WriteSettings.FileReferences.Clear();
        RDJsonSerializerOptions localOptions = new()
        {
            Type = RDLevel.LevelType,
            JsonSerializerOptions = new(options.JsonSerializerOptions)
            { WriteIndented = false, }
        };
        byte[] bytes = GetIndentByte(writer, options.JsonSerializerOptions.IndentCharacter, 2);
        ReadOnlySpan<byte> sl;
        writer.WriteStartObject();
        writer.WritePropertyName("settings");
        settingsConverter.Write(writer, value.Settings, options);
        if (WriteSettings.LoadAssets && !string.IsNullOrEmpty(DirectoryName))
            foreach (FileReference fr in value.Settings.GetAllFileReferences())
                if (!fr.IsEmpty && fr.IsExist(DirectoryName!))
                    WriteSettings.FileReferences.Add(fr);

        writer.WritePropertyName("rows");
        writer.WriteStartArray();
        using Utf8JsonWriter noIndentWriter = new(stream, new JsonWriterOptions { Indented = false, Encoder = options.JsonSerializerOptions.Encoder });
        using NoIndentScope noIndentScope = new(options.JsonSerializerOptions.Encoder, localOptions);
        noIndentScope.WriteNoIndentTo(options.WriteAligned, writer, value.Rows, (writer, row, options) =>
        {
            rowConverter.Write(writer, row, options);
            string assPath = Path.Combine(DirectoryName ?? "", row.Character.CustomCharacter ?? "");
            if (WriteSettings.LoadAssets && !string.IsNullOrEmpty(DirectoryName))
                foreach (FileReference file in row.Character.GetAllPossibleFileReferences())
                    if (!file.IsEmpty && file.IsExist(DirectoryName!))
                        WriteSettings.FileReferences.Add(file);
                    else if (file.IsExist(assPath))
                        WriteSettings.FileReferences.Add(DirectoryName + Path.DirectorySeparatorChar + file);
        });
        writer.WriteEndArray();
        writer.WritePropertyName("decorations");
        writer.WriteStartArray();
        noIndentScope.WriteNoIndentTo(options.WriteAligned, writer, value.Decorations, (writer, decoration, options) =>
        {
            decorationConverter.Write(writer, decoration, options);
            string assPath = Path.Combine(DirectoryName ?? "", decoration.Character.CustomCharacter ?? "");
            if (WriteSettings.LoadAssets && !string.IsNullOrEmpty(DirectoryName))
                foreach (FileReference file in decoration.Character.GetAllPossibleFileReferences())
                    if (!file.IsEmpty && file.IsExist(DirectoryName!))
                        WriteSettings.FileReferences.Add(file);
                    else if (file.IsExist(assPath))
                        WriteSettings.FileReferences.Add(DirectoryName + Path.DirectorySeparatorChar + file);
        });
        writer.WriteEndArray();
        writer.WritePropertyName("events");
        writer.WriteStartArray();
        noIndentScope.WriteNoIndentTo(false, writer, value, (writer, e, options) =>
        {
            baseEventConverter.Write(writer, e, options);
            if (WriteSettings.LoadAssets && e is IFileEvent fe && !string.IsNullOrEmpty(DirectoryName))
                foreach (FileReference file in fe.Files)
                    if (!file.IsEmpty && file.IsExist(DirectoryName!))
                        WriteSettings.FileReferences.Add(file);
        });
        writer.WriteEndArray();
        writer.WritePropertyName("bookmarks");
        writer.WriteStartArray();
        foreach (Bookmark bookmark in value.Bookmarks)
            bookmarkConverter.Write(writer, bookmark, localOptions);
        writer.WriteEndArray();
        writer.WritePropertyName("colorPalette");
        writer.WriteStartArray();
        foreach (RDColor color in value.ColorPalette)
            writer.WriteStringValue(color.ToString("RRGGBBAA"));
        writer.WriteEndArray();
        writer.WritePropertyName("conditionals");
        writer.WriteStartArray();
        noIndentScope.WriteNoIndentTo(options.WriteAligned, writer, value.Conditionals, conditionalConverter.Write);
        writer.WriteEndArray();
        writer.WriteEndObject();
    }

    private static byte[] GetIndentByte(Utf8JsonWriter writer, char indentChar, int indentSize) => Encoding.UTF8.GetBytes(Environment.NewLine + new string(indentChar, writer.CurrentDepth * indentSize));
}