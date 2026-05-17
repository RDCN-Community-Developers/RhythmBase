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
    /// <summary>
    /// Loads a <see cref="BBLevel"/> from the specified file path.
    /// </summary>
    /// <param name="filepath">The file path to load from.</param>
    /// <param name="settings">Optional read settings.</param>
    /// <returns>The loaded <see cref="BBLevel"/>.</returns>
    public static BBLevel FromFile(string filepath, ILevelReadSettings<IBaseEvent, EventType, BBBeat>? settings = null)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
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
        throw new NotImplementedException();
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
