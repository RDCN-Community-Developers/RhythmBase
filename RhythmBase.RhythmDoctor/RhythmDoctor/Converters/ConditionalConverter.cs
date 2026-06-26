using RhythmBase.Global.Converters;
using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Components.Conditions;
using System.Text;
using System.Text.Json;

namespace RhythmBase.RhythmDoctor.Converters;

[JsonConverterFor(typeof(BaseConditional))]
internal class ConditionalConverter : MetadataJsonConverter<BaseConditional>
{
	public override void Write(Utf8JsonWriter writer, BaseConditional? value, MetadataJsonSerializerOptions serializer)
	{
		if (value is null)
			return;
		writer.WriteStartObject();
		writer.WriteString("type", value.Type.ToEnumString());
		writer.WriteString("name"u8, value.Name);
		writer.WriteString("tag"u8, value.Tag);
		writer.WriteNumber("id"u8, value.Id);
		switch (value.Type)
		{
			case BaseConditional.ConditionType.LastHit:
				writer.WriteNumber("row"u8, ((LastHitCondition)value).Row);
				writer.WriteString("result"u8, ((LastHitCondition)value).Result.ToEnumString());
				break;
			case BaseConditional.ConditionType.Custom:
				writer.WriteString("expression"u8, ((CustomCondition)value).Expression);
				break;
			case BaseConditional.ConditionType.TimesExecuted:
				writer.WriteNumber("maxTimes"u8, ((TimesExecutedCondition)value).MaxTimes);
				break;
			case BaseConditional.ConditionType.Language:
				writer.WriteString("Language"u8, ((LanguageCondition)value).TargetLanguage.ToEnumString());
				break;
			case BaseConditional.ConditionType.PlayerMode:
				writer.WriteBoolean("twoPlayerMode"u8, ((PlayerModeCondition)value).TwoPlayerMode);
				break;
			case BaseConditional.ConditionType.Narration:
				writer.WriteBoolean("narrationEnabled"u8, ((NarrationCondition)value).NarrationEnabled);
				break;
			case BaseConditional.ConditionType.Accessibility:
				writer.WriteString("effectType"u8, ((AccessibilityCondition)value).TargetEffectType.ToEnumString());
				break;
			default:
				break;
		}
		writer.WriteEndObject();
	}
	public override BaseConditional? Read(ref Utf8JsonReader reader, Type objectType, MetadataJsonSerializerOptions serializer)
	{
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
		string? type = null;
		string tag = "";
		string name = "";
		Utf8JsonReader checkpoint = reader;
		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
		{
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
			if (reader.ValueTextEquals("type"u8) && reader.Read())
				type = reader.GetString();
			else if (reader.ValueTextEquals("name"u8) && reader.Read())
				name = reader.GetString() ?? "";
			else if (reader.ValueTextEquals("tag"u8) && reader.Read())
				tag = reader.GetString() ?? "";
			else
				reader.Skip();
		}
		reader = checkpoint;
		BaseConditional conditional;
		if (type == "Custom")
			conditional = ReadCustom(ref reader, serializer);
		else if (type == "Language")
			conditional = ReadLanguage(ref reader, serializer);
		else if (type == "LastHit")
			conditional = ReadLastHit(ref reader, serializer);
		else if (type == "PlayerMode")
			conditional = ReadPlayerMode(ref reader, serializer);
		else if (type == "TimesExecuted")
			conditional = ReadTimesExecuted(ref reader, serializer);
		else if (type == "narration" || type == "Narration")
			conditional = ReadNarration(ref reader, serializer);
		else if (type == "Accessibility")
			conditional = ReadAccessibility(ref reader, serializer);
		else
		{
#if DEBUG
			Console.WriteLine($"Unknown condition type: {type}");
#endif
			reader.Skip();
			return null;
		}
		conditional.Name = name;
		conditional.Tag = tag;
		return conditional;
	}
	private static CustomCondition ReadCustom(ref Utf8JsonReader reader, MetadataJsonSerializerOptions _)
	{
		CustomCondition condition = new();
		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
		{
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
			if (reader.ValueTextEquals("expression"u8) && reader.Read())
				condition.Expression = reader.GetString() ?? string.Empty;
		}
		return condition;
	}
	private static LanguageCondition ReadLanguage(ref Utf8JsonReader reader, MetadataJsonSerializerOptions _)
	{
		LanguageCondition condition = new();
		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
		{
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
			if (reader.ValueTextEquals("Language"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out LanguageCondition.Language languages))
				condition.TargetLanguage = languages;
		}
		return condition;
	}
	private static LastHitCondition ReadLastHit(ref Utf8JsonReader reader, MetadataJsonSerializerOptions _)
	{
		LastHitCondition condition = new();
		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
		{
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
			if (reader.ValueTextEquals("row"u8) && reader.Read())
				condition.Row = reader.GetSByte();
			else if (reader.ValueTextEquals("result"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out LastHitCondition.HitResult result))
				condition.Result = result;
		}
		return condition;
	}
	private static PlayerModeCondition ReadPlayerMode(ref Utf8JsonReader reader, MetadataJsonSerializerOptions _)
	{
		PlayerModeCondition condition = new();
		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
		{
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
			if (reader.ValueTextEquals("twoPlayerMode"u8) && reader.Read())
				condition.TwoPlayerMode = reader.GetBoolean();
		}
		return condition;
	}
	private static TimesExecutedCondition ReadTimesExecuted(ref Utf8JsonReader reader, MetadataJsonSerializerOptions _)
	{
		TimesExecutedCondition condition = new();
		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
		{
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
			if (reader.ValueTextEquals("maxTimes"u8) && reader.Read())
				condition.MaxTimes = reader.GetInt32();
		}
		return condition;
	}
	private static NarrationCondition ReadNarration(ref Utf8JsonReader reader, MetadataJsonSerializerOptions _)
	{
		NarrationCondition condition = new();
		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
		{
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
			if (reader.ValueTextEquals("narrationEnabled"u8) && reader.Read())
				condition.NarrationEnabled = reader.GetBoolean();
		}
		return condition;
	}
	private static AccessibilityCondition ReadAccessibility(ref Utf8JsonReader reader, MetadataJsonSerializerOptions _)
	{
		AccessibilityCondition condition = new();
		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
		{
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
			if (reader.ValueTextEquals("effectType"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out EffectType effectType))
				condition.TargetEffectType = effectType;
		}
		return condition;
	}
}