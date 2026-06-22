using RhythmBase.Adofai.Events;
using System.Text;
using System.Text.Json;

namespace RhythmBase.Adofai.Converters;

internal class MemberConverterBaseTileEvent<TEvent> : MemberConverter<TEvent> where TEvent : BaseTileEvent, new()
{
	protected override bool Read(ref Utf8JsonReader reader, ref TEvent value, MetadataJsonSerializerOptions options)
	{
		if (base.Read(ref reader, ref value, options))
			return true;
		if (reader.ValueTextEquals("floor"u8) && reader.Read())
			value._floor = reader.GetInt32();
		else
			return false;
		return true;
	}
}
internal class MemberConverterBaseTaggedTileEvent<TEvent> : MemberConverterBaseTileEvent<TEvent> where TEvent : BaseTaggedTileEvent, new()
{
	protected override bool Read(ref Utf8JsonReader reader, ref TEvent value, MetadataJsonSerializerOptions options)
	{
		if (base.Read(ref reader, ref value, options))
			return true;
		if (reader.ValueTextEquals("eventTag"u8) && reader.Read())
			value.EventTag = reader.GetString() ?? "";
		else if (reader.ValueTextEquals("angleOffset"u8) && reader.Read())
			value.AngleOffset = reader.GetSingle();
		else
			return false;
		return true;
	}
	protected override void Write(Utf8JsonWriter writer, ref TEvent value, MetadataJsonSerializerOptions options)
	{
		base.Write(writer, ref value, options);
		writer.WriteString("eventTag"u8, value.EventTag);
		writer.WriteNumber("angleOffset"u8, value.AngleOffset);
	}
}
internal class MemberConverterSetFilterAdvanced : MemberConverterBaseTaggedTileEvent<SetFilterAdvanced>
{
	protected override bool Read(ref Utf8JsonReader reader, ref SetFilterAdvanced value, MetadataJsonSerializerOptions options)
	{
		string filter = "";
		AdvancedFilter filterType = default;
		if (base.Read(ref reader, ref value, options))
			return true;
		if (reader.ValueTextEquals("filter"u8) && reader.Read())
		{
			filter = reader.GetString() ?? "";
			if (string.IsNullOrEmpty(filter) || !EnumConverter.TryParse(filter, out filterType))
				return false;
		}
		else if (reader.ValueTextEquals("enabled"u8) && reader.Read())
			value.Enabled = reader.GetBoolean();
		else if (reader.ValueTextEquals("disableOthers"u8) && reader.Read())
			value.DisableOthers = reader.GetBoolean();
		else if (reader.ValueTextEquals("duration"u8) && reader.Read())
			value.Duration = reader.GetSingle();
		else if (reader.ValueTextEquals("ease"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out Global.Components.Easing.EaseType enumValue0))
			value.Ease = enumValue0;
		else if (reader.ValueTextEquals("targetType"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out TargetType enumValue1))
			value.TargetType = enumValue1;
		else if (reader.ValueTextEquals("plane"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out Plane enumValue2))
			value.Plane = enumValue2;
		else if (reader.ValueTextEquals("targetTag"u8) && reader.Read())
			value.TargetTag = reader.GetString() ?? "";
		else if (reader.ValueTextEquals("filterProperties"u8) && reader.Read())
		{
			ReadOnlySpan<byte> json = [(byte)'{', .. Encoding.UTF8.GetBytes(reader.GetString() ?? ""), (byte)'}'];
			Utf8JsonReader subReader = new(json);
			FilterMemberConverterBase converter = EventConverterMap.GetConverter(filterType);
			value.FilterProperties = converter.ReadProperties(ref subReader, options);
		}
		else
			return false;
		return true;
	}
	protected override void Write(Utf8JsonWriter writer, ref SetFilterAdvanced value, MetadataJsonSerializerOptions options)
	{
		base.Write(writer, ref value, options);
		writer.WriteString("filter"u8, value.FilterProperties.Type.ToEnumString());
		writer.WriteBoolean("enabled"u8, value.Enabled);
		writer.WriteBoolean("disableOthers"u8, value.DisableOthers);
		if (value.Enabled)
			writer.WriteNumber("duration"u8, value.Duration);
		if (value.Enabled)
			writer.WriteString("ease"u8, value.Ease.ToEnumString());
		writer.WriteString("targetType"u8, value.TargetType.ToEnumString());
		writer.WriteString("plane"u8, value.Plane.ToEnumString());
		writer.WriteString("targetTag"u8, value.TargetTag);
		writer.WritePropertyName("filterProperties"u8);
		{
			using MemoryStream ms = new();
			using Utf8JsonWriter tempWriter = new(ms, new() { SkipValidation = true, });
			FilterMemberConverterBase converter = EventConverterMap.GetConverter(value.FilterProperties.Type);
			converter.WriteProperties(tempWriter, value.FilterProperties, options);
			tempWriter.Flush();
			string jsonString = Encoding.UTF8.GetString(ms.ToArray());
			writer.WriteStringValue(jsonString);
		}

	}
}