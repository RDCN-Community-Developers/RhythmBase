using RhythmBase.Global.Serialization;
using RhythmBase.RhythmDoctor.Components;
using System.Text.Json;

namespace RhythmBase.RhythmDoctor.Serialization;

[JsonConverterFor(typeof(Decoration))]
internal class DecorationConverter : MetadataJsonConverter<Decoration>
{
	public override Decoration? Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartObject)
			throw new JsonException($"Expected StartObject token, but got {reader.TokenType}.");
		Decoration value = [];
		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
		{
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
			var checkpoint = reader;
			if (reader.ValueTextEquals("id"u8) && reader.Read())
				value.Id = reader.GetString() ?? "";
			else if (reader.ValueTextEquals("rooms"u8) && reader.Read())
				value.Room = TypeConverterRegistry.Read<SingleRoom>(ref reader, options);
			else if (reader.ValueTextEquals("filename"u8) && reader.Read())
				value.Character = reader.GetString() ?? "";
			else if (reader.ValueTextEquals("character"u8) && reader.Read())
			{
				string character = reader.GetString() ?? "";
				if (EnumConverter.TryParse(character, out GameCharacter rdc))
					value.Character = rdc;
				else
					value.Character = character;
			}
			else if (reader.ValueTextEquals("preview"u8) && reader.Read())
				value.Preview = reader.GetBoolean();
			else if (reader.ValueTextEquals("depth"u8) && reader.Read())
				value.Depth = reader.GetInt32();
			else if (reader.ValueTextEquals("filter"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out Filter result))
#pragma warning disable CS0612
				value.Filter = result;
#pragma warning restore CS0612
			else if (reader.ValueTextEquals("visible"u8) && reader.Read())
				value.Visible = reader.GetBoolean();
			else if (reader.ValueTextEquals("row"u8) && reader.Read())
				reader.Skip();
			else if (reader.ValueTextEquals("type"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out DecorationType type))
				value.Type = type;
			else if (reader.ValueTextEquals("decoName"u8) && reader.Read())
				value.Name = reader.GetString() ?? "";
			else if (reader.ValueTextEquals("font"u8) && reader.Read())
				value.Font = reader.GetString() ?? "";
			else if (reader.ValueTextEquals("sortingLayer"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out LayerType layer))
				value.Layer = layer;
			else
			{
				switch (options.Strictness)
				{
					case JsonStrictness.Strict:
						throw new JsonException($"Unexpected property '{reader.GetString()}' in Decoration object.");
					case JsonStrictness.Corrective:
						reader = checkpoint;
						var fieldName = reader.GetString() ?? "";
						reader.Read();
						JsonElement extraData = JsonElement.ParseValue(ref reader);
						value[fieldName] = extraData;
#if DEBUG
						Console.WriteLine($"{options.Version}\t| Decoration\t| {fieldName} => ({value[fieldName].ValueKind}){value[fieldName]}");
#endif
						break;
					case JsonStrictness.Fallback:
						reader.Skip();
						break;
				}
			}
		}
		return value;
	}

	public override void Write(Utf8JsonWriter writer, Decoration value, MetadataJsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WriteString("type", value.Type.ToEnumString());
		writer.WriteString("id"u8, value.Id);
		writer.WriteNumber("row"u8, value.Index);
		TypeConverterRegistry.Write(writer, "rooms"u8, value.Room, options);
		if (value.Type is DecorationType.Sprite)
		{
			if (!value.Character.IsCustom && value.Character.EnumName is GameCharacter rdc)
				writer.WriteString("character", rdc.ToEnumString());
			else
				writer.WriteString("filename", value.Character.StringName);
			writer.WriteBoolean("preview"u8, value.Preview);
		}
		else
		{
			writer.WriteString("decoName"u8, value.Name);
			writer.WriteString("font"u8, value.Font.Value);
			writer.WriteString("sortingLayer"u8, value.Layer.ToEnumString());
		}
		writer.WriteNumber("depth"u8, value.Depth);
#pragma warning disable CS0612
		if (value.Filter is not Filter.NearestNeighbor)
			writer.WriteString("filter"u8, value.Filter.ToEnumString());
#pragma warning restore CS0612
		if (!value.Visible)
			writer.WriteBoolean("visible"u8, value.Visible);
		foreach (var kvp in value.ExtraData)
		{
			writer.WritePropertyName(kvp.Key);
			kvp.Value.WriteTo(writer);
		}
		writer.WriteEndObject();
	}
}
