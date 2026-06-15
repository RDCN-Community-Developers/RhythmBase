using RhythmBase.Global.Converters;
using RhythmBase.RhythmDoctor.Components;
using System.Text.Json;

namespace RhythmBase.RhythmDoctor.Converters;

[JsonConverterFor(typeof(Decoration))]
internal class DecorationConverter : MetadataJsonConverter<Decoration>
{
	public override Decoration? Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartObject)
			throw new JsonException($"Expected StartObject token, but got {reader.TokenType}.");
		Decoration decoration = [];
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndObject)
				break;
			if (reader.TokenType == JsonTokenType.PropertyName)
			{
				if (reader.ValueTextEquals("id"u8))
				{
					reader.Read();
					decoration.Id = reader.GetString() ?? "";
				}
				else if (reader.ValueTextEquals("rooms"u8))
				{
					reader.Read();
					decoration.Room = TypeConverterRegistry.Read<SingleRoom>(ref reader, options);
				}
				else if (reader.ValueTextEquals("filename"u8))
				{
					reader.Read();
					decoration.Character = reader.GetString() ?? "";
				}
				else if (reader.ValueTextEquals("character"u8))
				{
					reader.Read();
					string character = reader.GetString() ?? "";
					if (EnumConverter.TryParse(character, out Characters rdc))
						decoration.Character = rdc;
					else
						decoration.Character = character;
				}
				else if (reader.ValueTextEquals("preview"u8))
				{
					reader.Read();
					decoration.Preview = reader.GetBoolean();
				}
				else if (reader.ValueTextEquals("depth"u8))
				{
					reader.Read();
					decoration.Depth = reader.GetInt32();
				}
				else if (reader.ValueTextEquals("filter"u8))
				{
					reader.Read();
					if (EnumConverter.TryParse(ref reader, out Filter result))
						decoration.Filter = result;
				}
				else if (reader.ValueTextEquals("visible"u8))
				{
					reader.Read();
					decoration.Visible = reader.GetBoolean();
				}
			}
		}
		return decoration;
	}

	public override void Write(Utf8JsonWriter writer, Decoration value, MetadataJsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WriteString("id"u8, value.Id);
		writer.WriteNumber("row"u8, value.Index);
		writer.WritePropertyName("rooms"u8);
		TypeConverterRegistry.Write(writer, value.Room, options);
		if (!value.Character.IsCustom && value.Character.Character is Characters rdc)
			writer.WriteString("character", rdc.ToEnumString());
		else
			writer.WriteString("filename", value.Character.CustomCharacter);
		writer.WriteBoolean("preview"u8, value.Preview);
		writer.WriteNumber("depth"u8, value.Depth);
		if (value.Filter is not Filter.NearestNeighbor)
			writer.WriteString("filter"u8, value.Filter.ToEnumString());
		if (!value.Visible)
			writer.WriteBoolean("visible"u8, value.Visible);
		writer.WriteEndObject();
	}
}
