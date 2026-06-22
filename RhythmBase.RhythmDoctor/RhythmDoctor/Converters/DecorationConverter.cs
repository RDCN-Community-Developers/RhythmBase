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
				value.Filter = result;
			else if (reader.ValueTextEquals("visible"u8) && reader.Read())
				value.Visible = reader.GetBoolean();
			else
			{
				reader = checkpoint;
				var fieldName = reader.GetString() ?? "";
				if(fieldName == "row")
				{
					reader.Read();
					reader.Skip();
					continue;
				}
				reader.Read();
				JsonElement extraData = JsonElement.ParseValue(ref reader);
				value[fieldName] = extraData;
#if DEBUG
				Console.WriteLine($"{options.Version}\t| Decoration\t| {fieldName} => ({value[fieldName].ValueKind}){value[fieldName]}");
#endif
			}
		}
		return value;
	}

	public override void Write(Utf8JsonWriter writer, Decoration value, MetadataJsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WriteString("id"u8, value.Id);
		writer.WriteNumber("row"u8, value.Index);
		writer.WritePropertyName("rooms"u8);
		TypeConverterRegistry.Write(writer, value.Room, options);
		if (!value.Character.IsCustom && value.Character.EnumName is GameCharacter rdc)
			writer.WriteString("character", rdc.ToEnumString());
		else
			writer.WriteString("filename", value.Character.StringName);
		writer.WriteBoolean("preview"u8, value.Preview);
		writer.WriteNumber("depth"u8, value.Depth);
		if (value.Filter is not Filter.NearestNeighbor)
			writer.WriteString("filter"u8, value.Filter.ToEnumString());
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
