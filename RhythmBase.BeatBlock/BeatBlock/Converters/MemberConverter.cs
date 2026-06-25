using RhythmBase.BeatBlock.Components;
using RhythmBase.BeatBlock.Events;
using System.Text.Json;

namespace RhythmBase.BeatBlock.Converters;

internal class BaseEventConverter : BackwardCompatibleMetadataJsonConverter
{
	private float _6_bpm;
	private float _12_playSongTime;
	public void Reset()
	{
		_6_bpm = 0;
		_12_playSongTime = 0;
	}
	public BaseEventConverter()
	{
		Reset();
	}
	protected override void InitializeUpgraters()
	{
		Register<Hold>(2, (e) =>
		{
			e.Angle = e["angle1"].TryGetSingle(out var angle1) ? angle1 : 0;
			e["angle1"] = default;
		});
		Register<MineHold>(3, (e) =>
		{
			e.Angle = e["angle1"].TryGetSingle(out var angle1) ? angle1 : 0;
			e["angle1"] = default;
		});
		Register<Play>(6, (e) =>
		{
			if (e is not Play p) return;
			_6_bpm = p.BeatsPerMinute;
		});
		Register<Paddles>(6, (e) =>
		{
			if (e is not Paddles p) return;
			p.Duration /= 3600 / _6_bpm;
		});
		Register<Decoration>(9, (e) =>
		{
			if (e is not Decoration p) return;
			p.DrawOrder = p.Order;
			p.Order = null;
		});
		Register<Decoration>(11, (e) =>
		{
			if (e is not Decoration p) return;
			p.Order = p.Order is null or 0 ? -999 : p.Order;
		});
		Register<SetColor>(11, (e) =>
		{
			if (e is not SetColor p) return;
			p.Order = p.Order is null or 0 ? -999 : p.Order;
		});
		Register<Ease>(11, (e) =>
		{
			if (e is not Ease p) return;
			p.Order = p.Order is null or 0 ? -999 : p.Order;
		});
		Register<Play>(12, (e) =>
		{
			if (e is not Play p) return;
			_12_playSongTime = p.TickTime.Tick;
		});
		Register<SetBeatsPerMinute>(12, (e) =>
		{
			if (e is not SetBeatsPerMinute s) return;
			if (_12_playSongTime == 0) return;
			e.TickTime += _12_playSongTime;
		});
		//Register<PlaySound>(13, (e) =>
		//{
		//	if(e is not PlaySound p) return;
		//	if(false /*not in built-in sounds*/)
		//		p.Sound += ".ogg";
		//});
		Register<Paddles>(15, (e) =>
		{
			if (e is not Paddles p) return;
			p.ForceStoreInLevel = true;
		});
		Register<Decoration>(16, (e) =>
		{
			if (e is not Decoration d) return;
			d.EffectCanvasRaw = d.EffectCanvas;
		});
	}
	public override bool CanConvert(Type typeToConvert)
	{
		return Type.IsAssignableFrom(typeToConvert);
	}
	public override IBaseEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
	{
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);

		Utf8JsonReader checkpoint = reader;
		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
		{
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
			if (reader.ValueTextEquals("type"u8) && reader.Read())
				break;
			else
				reader.Skip();
		}
		IBaseEvent e;
		// upgrate to the latest version

		if (EnumConverter.TryParse(ref reader, out EventType typeEnum))
			e = EventConverterMap.GetConverter(typeEnum).ReadProperties(ref checkpoint, options);
		else
		{
			if (options.Version <= 1 && reader.ValueTextEquals("beat"u8))
				e = EventConverterMap.GetConverter(EventType.Block).ReadProperties(ref checkpoint, options);
			else if (options.Version <= 10 && reader.ValueTextEquals("width"u8))
				e = EventConverterMap.GetConverter(EventType.Paddles).ReadProperties(ref checkpoint, options);
			else
				e = ReadForwardEvent(ref checkpoint) ?? new ForwardEvent() { ActualType = reader.GetString() ?? "" };
		}
		JsonException.ThrowIfNotMatch(ref checkpoint, JsonTokenType.EndObject);
		reader = checkpoint;
		return e;
	}
	public static Events.IForwardEvent? ReadForwardEvent(ref Utf8JsonReader reader)
	{
		JsonDocument doc = JsonDocument.ParseValue(ref reader);
		//JsonElement root = doc.RootElement;

		return new ForwardEvent(doc);
	}

	public override void Write(Utf8JsonWriter writer, IBaseEvent value, MetadataJsonSerializerOptions options)
	{
		if (value is Events.IForwardEvent ce)
			WriteForwardEvent(writer, ce);
		else
			EventConverterMap.GetConverter(value.Type).WriteProperties(writer, value, options);
	}

	private static void WriteForwardEvent(Utf8JsonWriter writer, Events.IForwardEvent value)
	{
		writer.WriteStartObject();
		if (!string.IsNullOrEmpty(value.ActualType))
			writer.WriteString("type"u8, value.ActualType);
		foreach (KeyValuePair<string, JsonElement> kv in ((BaseEvent)(IBaseEvent)value)._extraData)
		{
			writer.WritePropertyName(kv.Key);
			kv.Value.WriteTo(writer);
		}
		writer.WriteEndObject();
	}

}
internal abstract class EventInstanceConverterBase : Global.Converters.MemberConverter<IBaseEvent> { }
internal abstract class MemberConverter<TEvent> : EventInstanceConverterBase where TEvent : IBaseEvent, new()
{
	public sealed override IBaseEvent ReadProperties(ref Utf8JsonReader reader, MetadataJsonSerializerOptions options)
	{
		TEvent value = new();
		float time = 0;
		float angle = 0;
		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
		{
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
			var checkpoint = reader;
			if (reader.ValueTextEquals("time"u8) && reader.Read())
				time = reader.GetSingle();
			else if (reader.ValueTextEquals("angle"u8) && reader.Read())
				angle = reader.GetSingle();
			else if (reader.ValueTextEquals("type"u8) && reader.Read())
			{ }
			else if (!Read(ref reader, ref value, options))
			{
				reader = checkpoint;
				string fieldName = reader.GetString()!;
				reader.Read();
				JsonElement fieldValue = JsonElement.ParseValue(ref reader);

				if (UnhandledFieldRegistry.TryHandle(ref value, fieldName, fieldValue, (int)value.Type))
					continue;
				if (options.TryHandleUser(ref value, fieldName, fieldValue, (int)value.Type))
					continue;

				value[fieldName] = fieldValue;
			}
		}
		value.TickTime = new TickTime(time);
		value.Angle = angle;
		return value;
	}
	public sealed override void WriteProperties(Utf8JsonWriter writer, IBaseEvent value, MetadataJsonSerializerOptions options)
	{
		TEvent v = (TEvent)value;
		writer.WriteStartObject();
		Write(writer, ref v, options);
		foreach (KeyValuePair<string, JsonElement> kv in ((BaseEvent)(IBaseEvent)v)._extraData)
		{
			writer.WritePropertyName(kv.Key);
			writer.WriteRawValue(kv.Value.GetRawText());
		}
		writer.WriteEndObject();
	}
	protected virtual bool Read(ref Utf8JsonReader reader, ref TEvent value, MetadataJsonSerializerOptions options)
	{
		bool result = true;
		if (reader.ValueTextEquals("angle"u8) && reader.Read())
			value.Angle = reader.GetSingle();
		else if (reader.ValueTextEquals("time"u8) && reader.Read())
			value.TickTime = new TickTime(reader.GetSingle());
		else if (reader.ValueTextEquals("variant"u8) && reader.Read())
			value.Variant = reader.GetString();
		else if (reader.ValueTextEquals("order"u8) && reader.Read())
			value.Order = reader.GetInt32();
		else
			result = false;
		return result;
	}
	protected virtual void Write(Utf8JsonWriter writer, ref TEvent value, MetadataJsonSerializerOptions options)
	{
		writer.WriteNumber("angle"u8, value.Angle);
		writer.WriteNumber("time"u8, value.TickTime.Tick);
		if (value.Variant is not null)
			writer.WriteString("variant"u8, value.Variant);
		if (value.Order is int valueNotNull)
			writer.WriteNumber("order"u8, valueNotNull);
	}
}