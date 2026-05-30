using RhythmBase.Rizline.Components;
using RhythmBase.Rizline.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace RhythmBase.Rizline.Converters;

internal class MetadataEventConverter : MetadataJsonConverter<IBaseEvent>
{
	public override IBaseEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
	{
		JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartObject]);
		int type = -1;
		Utf8JsonReader checkpoint = reader;
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndObject)
				break;
			if (reader.TokenType == JsonTokenType.PropertyName)
			{
				if (reader.ValueTextEquals("type"u8))
				{
					reader.Read();
					type = reader.GetInt32();
					break;
				}
				else
				{
					reader.Skip();
				}
			}
		}
		reader = checkpoint; IBaseEvent e;
		e = null;
		return e;
	}

	public override void Write(Utf8JsonWriter writer, IBaseEvent value, MetadataJsonSerializerOptions options)
	{
		throw new NotImplementedException();
	}
}

internal abstract class InstanceConverter<TEvent> : RhythmBase.Global.Converters.InstanceConverter<IBaseEvent> where TEvent : IBaseEvent
{
    private LevelReadSettings _rs = new LevelReadSettings();
    private LevelWriteSettings _ws = new LevelWriteSettings();
    public InstanceConverter<TEvent> WithReadSettings(LevelReadSettings settings)
    {
        _rs = settings;
        return this;
    }
    public InstanceConverter<TEvent> WithWriteSettings(LevelWriteSettings settings)
    {
        _ws = settings;
        return this;
    }
	public override IBaseEvent ReadProperties(ref Utf8JsonReader reader, MetadataJsonSerializerOptions options)
	{
		throw new NotImplementedException();
	}
	public override void WriteProperties(Utf8JsonWriter writer, IBaseEvent value, MetadataJsonSerializerOptions options)
	{
		throw new NotImplementedException();
	}
}