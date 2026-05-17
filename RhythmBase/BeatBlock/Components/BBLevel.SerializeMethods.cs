using RhythmBase.BeatBlock.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace RhythmBase.BeatBlock.Components;

partial class BBLevel
{
    private static class Deserializer
    {
        public static BBLevel Deserialize(IJsonDataSource dataSource, JsonSerializerOptions options)
        {
            if (dataSource.CanGetMemoryDirectly)
            {
                ReadOnlyMemory<byte> jsonData = dataSource.GetMemory();
                Utf8JsonReader reader = new(jsonData.Span, new() { AllowTrailingCommas = true });
                return ConverterHub.Read<BBLevel>(ref reader, options) ?? [];
            }
            else
            {
                ReadOnlyMemory<byte> jsonData = dataSource.GetMemoryAsync().GetAwaiter().GetResult();
                Utf8JsonReader reader = new(jsonData.Span, new() { AllowTrailingCommas = true });
                return ConverterHub.Read<BBLevel>(ref reader, options) ?? [];
            }
        }
        public static async Task<BBLevel> DeserializeAsync(IJsonDataSource dataSource, JsonSerializerOptions options, CancellationToken token = default)
        {
            if (dataSource.CanGetMemoryDirectly)
            {
                ReadOnlyMemory<byte> jsonData = dataSource.GetMemory();
                Utf8JsonReader reader = new(jsonData.Span, new() { AllowTrailingCommas = true });
                return ConverterHub.Read<BBLevel>(ref reader, options) ?? [];
            }
            else
            {
                ReadOnlyMemory<byte> jsonData = await dataSource.GetMemoryAsync(token);
                Utf8JsonReader reader = new(jsonData.Span, new() { AllowTrailingCommas = true });
                return ConverterHub.Read<BBLevel>(ref reader, options) ?? [];
            }
        }
    }
    public static BBLevel FromFile(string filepath, ILevelReadSettings<IBaseEvent, EventType, BBBeat>? settings = null)
    {
        throw new NotImplementedException();
    }

    public static Task<BBLevel> FromFileAsync(string filepath, ILevelReadSettings<IBaseEvent, EventType, BBBeat>? settings = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public static BBLevel FromJsonDocument(JsonDocument jsonDocument, ILevelReadSettings<IBaseEvent, EventType, BBBeat>? settings = null)
    {
        throw new NotImplementedException();
    }

    public static BBLevel FromJsonString(string json, ILevelReadSettings<IBaseEvent, EventType, BBBeat>? settings = null)
    {
        throw new NotImplementedException();
    }

    public static BBLevel FromStream(Stream stream, ILevelReadSettings<IBaseEvent, EventType, BBBeat>? settings = null)
    {
        throw new NotImplementedException();
    }

    public static Task<BBLevel> FromStreamAsync(Stream stream, ILevelReadSettings<IBaseEvent, EventType, BBBeat>? settings = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }


    public void SaveToFile(string filepath, ILevelWriteSettings<IBaseEvent, EventType, BBBeat>? settings = null)
    {
        throw new NotImplementedException();
    }

    public void SaveToFileAsync(string filepath, ILevelWriteSettings<IBaseEvent, EventType, BBBeat>? settings = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void SaveToStream(Stream stream, ILevelWriteSettings<IBaseEvent, EventType, BBBeat>? settings = null)
    {
        throw new NotImplementedException();
    }

    public void SaveToStreamAsync(Stream stream, ILevelWriteSettings<IBaseEvent, EventType, BBBeat>? settings = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void SaveToZip(string filepath, ILevelWriteSettings<IBaseEvent, EventType, BBBeat>? settings = null)
    {
        throw new NotImplementedException();
    }

    public void SaveToZipAsync(string filepath, ILevelWriteSettings<IBaseEvent, EventType, BBBeat>? settings = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public JsonDocument ToJsonDocument(ILevelWriteSettings<IBaseEvent, EventType, BBBeat>? settings = null)
    {
        throw new NotImplementedException();
    }

    public string ToJsonString(ILevelWriteSettings<IBaseEvent, EventType, BBBeat>? settings = null)
    {
        throw new NotImplementedException();
    }
}
