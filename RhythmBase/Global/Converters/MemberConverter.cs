using System.Text.Json;

namespace RhythmBase.Global.Converters;


public abstract class MemberConverter<TEvent> where TEvent : IEvent
{
	protected LevelReadSettings? _rs;
	protected LevelWriteSettings? _ws;
	public MemberConverter<TEvent> WithReadSettings(LevelReadSettings settings)
	{
		_rs = settings;
		return this;
	}
	public MemberConverter<TEvent> WithWriteSettings(LevelWriteSettings settings)
	{
		_ws = settings;
		return this;
	}
	public abstract TEvent ReadProperties(ref Utf8JsonReader reader, MetadataJsonSerializerOptions options);
	public abstract void WriteProperties(Utf8JsonWriter writer, TEvent value, MetadataJsonSerializerOptions options);
}
