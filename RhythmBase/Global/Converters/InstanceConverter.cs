using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace RhythmBase.Global.Converters;


public abstract class InstanceConverter<TEvent> where TEvent : IEvent
{
	protected LevelReadSettings? _rs;
	protected LevelWriteSettings? _ws;
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
	public abstract TEvent ReadProperties(ref Utf8JsonReader reader, MetadataJsonSerializerOptions options);
	public abstract void WriteProperties(Utf8JsonWriter writer, TEvent value, MetadataJsonSerializerOptions options);
}
