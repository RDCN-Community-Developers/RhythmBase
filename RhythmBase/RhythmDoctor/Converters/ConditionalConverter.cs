using RhythmBase.Global.Extensions;
using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Components.Conditions;
using System.Text.Json;
using System.Text.Json.Serialization;
using static RhythmBase.Global.Extensions.EnumConverter;

namespace RhythmBase.RhythmDoctor.Converters;

[RDJsonConverterFor(typeof(BaseConditional))]
internal class ConditionalConverter : RDJsonConverter<BaseConditional>
{
	public override void Write(Utf8JsonWriter writer, BaseConditional? value, RDJsonSerializerOptions serializer)
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
	public override BaseConditional? Read(ref Utf8JsonReader reader, Type objectType, RDJsonSerializerOptions serializer)
	{
		if (reader.TokenType != JsonTokenType.StartObject)
			throw new JsonException($"Expected StartObject token, but got {reader.TokenType}.");
		ReadOnlySpan<byte> type = default;
		string tag = "";
		string name = "";
		Utf8JsonReader checkpoint = reader;
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndObject)
				break;
			if (reader.TokenType == JsonTokenType.PropertyName)
			{
				ReadOnlySpan<byte> propertyName = reader.ValueSpan;
				if (propertyName.SequenceEqual("type"u8))
				{
					reader.Read();
					type = reader.ValueSpan;
					if (type.IsEmpty)
						return null;
				}
				else if (propertyName.SequenceEqual("name"u8))
				{
					reader.Read();
					name = reader.GetString() ?? "";
				}
				else if (propertyName.SequenceEqual("tag"u8))
				{
					reader.Read();
					tag = reader.GetString() ?? "";
				}
				else
				{
					reader.Skip();
				}
			}
		}
		reader = checkpoint;
		BaseConditional conditional;
		if (type.SequenceEqual("Custom"u8))
			conditional = ReadCustom(ref reader, serializer);
		else if (type.SequenceEqual("Language"u8))
			conditional = ReadLanguage(ref reader, serializer);
		else if (type.SequenceEqual("LastHit"u8))
			conditional = ReadLastHit(ref reader, serializer);
		else if (type.SequenceEqual("PlayerMode"u8))
			conditional = ReadPlayerMode(ref reader, serializer);
		else if (type.SequenceEqual("TimesExecuted"u8))
			conditional = ReadTimesExecuted(ref reader, serializer);
		else if (type.SequenceEqual("narration"u8))
			conditional = ReadNarration(ref reader, serializer);
		else if (type.SequenceEqual("Accessibility"u8))
			conditional = ReadAccessibility(ref reader, serializer);
		else
			return null;
		reader.Read();
		conditional.Name = name;
		conditional.Tag = tag;
		return conditional;
	}
	private static CustomCondition ReadCustom(ref Utf8JsonReader reader, RDJsonSerializerOptions _)
	{
		CustomCondition condition = new();
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndObject)
				break;
			if (reader.TokenType == JsonTokenType.PropertyName)
			{
				ReadOnlySpan<byte> propertyName = reader.ValueSpan;
				reader.Read();
				if (propertyName.SequenceEqual("expression"u8))
					condition.Expression = reader.GetString() ?? string.Empty;
			}
		}
		return condition;
	}
	private static LanguageCondition ReadLanguage(ref Utf8JsonReader reader, RDJsonSerializerOptions _)
	{
		LanguageCondition condition = new();
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndObject)
				break;
			if (reader.TokenType == JsonTokenType.PropertyName)
			{
				ReadOnlySpan<byte> propertyName = reader.ValueSpan;
				reader.Read();
				if (propertyName.SequenceEqual("Language"u8) && TryParse(reader.ValueSpan, out LanguageCondition.Language languages))
					condition.TargetLanguage = languages;
			}
		}
		return condition;
	}
	private static LastHitCondition ReadLastHit(ref Utf8JsonReader reader, RDJsonSerializerOptions _)
	{
		LastHitCondition condition = new();
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndObject)
				break;
			if (reader.TokenType == JsonTokenType.PropertyName)
			{
				ReadOnlySpan<byte> propertyName = reader.ValueSpan;
				reader.Read();
				if (propertyName.SequenceEqual("row"u8))
					condition.Row = reader.GetSByte();
				else if (propertyName.SequenceEqual("result"u8) && TryParse(reader.ValueSpan, out LastHitCondition.HitResult result))
					condition.Result = result;
			}
		}
		return condition;
	}
	private static PlayerModeCondition ReadPlayerMode(ref Utf8JsonReader reader, RDJsonSerializerOptions _)
	{
		PlayerModeCondition condition = new();
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndObject)
				break;
			if (reader.TokenType == JsonTokenType.PropertyName)
			{
				ReadOnlySpan<byte> propertyName = reader.ValueSpan;
				reader.Read();
				if (propertyName.SequenceEqual("twoPlayerMode"u8))
					condition.TwoPlayerMode = reader.GetBoolean();
			}
		}
		return condition;
	}
	private static TimesExecutedCondition ReadTimesExecuted(ref Utf8JsonReader reader, RDJsonSerializerOptions _)
	{
		TimesExecutedCondition condition = new();
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndObject)
				break;
			if (reader.TokenType == JsonTokenType.PropertyName)
			{
				ReadOnlySpan<byte> propertyName = reader.ValueSpan;
				reader.Read();
				if (propertyName.SequenceEqual("maxTimes"u8))
					condition.MaxTimes = reader.GetInt32();
			}
		}
		return condition;
	}
	private static NarrationCondition ReadNarration(ref Utf8JsonReader reader, RDJsonSerializerOptions _)
	{
		NarrationCondition condition = new();
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndObject)
				break;
			if (reader.TokenType == JsonTokenType.PropertyName)
			{
				ReadOnlySpan<byte> propertyName = reader.ValueSpan;
				reader.Read();
				if (propertyName.SequenceEqual("narrationEnabled"u8))
					condition.NarrationEnabled = reader.GetBoolean();
			}
		}
		return condition;
	}
	private static AccessibilityCondition ReadAccessibility(ref Utf8JsonReader reader, RDJsonSerializerOptions _)
	{
		AccessibilityCondition condition = new();
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndObject)
				break;
			if (reader.TokenType == JsonTokenType.PropertyName)
			{
				ReadOnlySpan<byte> propertyName = reader.ValueSpan;
				reader.Read();
				if (propertyName.SequenceEqual("effectType"u8) && TryParse(reader.ValueSpan, out EffectType effectType))
					condition.TargetEffectType = effectType;
			}
		}
		return condition;
	}
}