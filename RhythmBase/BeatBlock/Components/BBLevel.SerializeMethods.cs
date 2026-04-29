using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace RhythmBase.BeatBlock.Components;

partial class BBLevel
{
    public static BBLevel FromFile(string filepath, LevelReadSettings? settings = null)
    {
        throw new NotImplementedException();
    }

    public static Task<BBLevel> FromFileAsync(string filepath, LevelReadSettings? settings = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public static BBLevel FromJsonDocument(JsonDocument jsonDocument, LevelReadSettings? settings = null)
    {
        throw new NotImplementedException();
    }

    public static BBLevel FromJsonString(string json, LevelReadSettings? settings = null)
    {
        throw new NotImplementedException();
    }

    public static BBLevel FromStream(Stream stream, LevelReadSettings? settings = null)
    {
        throw new NotImplementedException();
    }

    public static Task<BBLevel> FromStreamAsync(Stream stream, LevelReadSettings? settings = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }


    public void SaveToFile(string filepath, LevelWriteSettings? settings = null)
    {
        throw new NotImplementedException();
    }

    public void SaveToFileAsync(string filepath, LevelWriteSettings? settings = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void SaveToStream(Stream stream, LevelWriteSettings? settings = null)
    {
        throw new NotImplementedException();
    }

    public void SaveToStreamAsync(Stream stream, LevelWriteSettings? settings = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void SaveToZip(string filepath, LevelWriteSettings? settings = null)
    {
        throw new NotImplementedException();
    }

    public void SaveToZipAsync(string filepath, LevelWriteSettings? settings = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public JsonDocument ToJsonDocument(LevelWriteSettings? settings = null)
    {
        throw new NotImplementedException();
    }

    public string ToJsonString(LevelWriteSettings? settings = null)
    {
        throw new NotImplementedException();
    }
}
