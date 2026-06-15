using RhythmBase.Global.Components.Vector;
using RhythmBase.Global.Converters;
using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Events;
using System.Text.Json;

namespace RhythmBase.RhythmDoctor.Converters;

internal partial class RDMemberConverter
{
		internal abstract class BaseRowAction<TEvent> : MemberConverter<TEvent> where TEvent : BaseRowAction, new()
		{
		protected override bool Read(ref Utf8JsonReader reader, ref TEvent value, MetadataJsonSerializerOptions options)
		{
				if (base.Read(ref reader, ref value, options))
						return true;
				if (reader.ValueTextEquals("row"u8) && reader.Read())
						value._row = reader.GetInt32();
				else
						return false;
				return true;
		}
				protected override void Write(Utf8JsonWriter writer, ref TEvent value, MetadataJsonSerializerOptions options)
				{
						base.Write(writer, ref value, options);
						writer.WriteNumber("row"u8, value.Parent?.Index ?? value._row);
				}
		}
		internal abstract class BaseBeat<TEvent> : BaseRowAction<TEvent> where TEvent : BaseBeat, new()
		{
		}
		internal abstract class BaseDecorationAction<TEvent> : MemberConverter<TEvent> where TEvent : BaseDecorationAction, new()
		{
				protected override bool Read(ref Utf8JsonReader reader, ref TEvent value, MetadataJsonSerializerOptions options)
				{
						if (base.Read(ref reader, ref value, options))
								return true;
						if (reader.ValueTextEquals("target"u8) && reader.Read())
								value._decoId = reader.GetString() ?? "";
						else
								return false;
						return true;
				}
				protected override void Write(Utf8JsonWriter writer, ref TEvent value, MetadataJsonSerializerOptions options)
				{
						base.Write(writer, ref value, options);
						if (value is not Comment cmt || cmt.CustomTab == Tab.Decorations)
								writer.WriteString("target"u8, value.Parent?.Id ?? value._decoId);
				}
		}
		internal abstract class BaseBeatsPerMinute<TEvent> : MemberConverter<TEvent> where TEvent : BaseBeatsPerMinute, new()
		{
				protected override bool Read(ref Utf8JsonReader reader, ref TEvent value, MetadataJsonSerializerOptions options)
				{
						return base.Read(ref reader, ref value, options);
				}
				protected override void Write(Utf8JsonWriter writer, ref TEvent value, MetadataJsonSerializerOptions options)
				{
						base.Write(writer, ref value, options);						
				}
		}
		internal abstract class BaseWindowEvent<TEvent> : MemberConverter<TEvent> where TEvent : BaseWindowEvent, new()
		{
				protected override bool Read(ref Utf8JsonReader reader, ref TEvent value, MetadataJsonSerializerOptions options)
				{
						return base.Read(ref reader, ref value, options);
				}
				protected override void Write(Utf8JsonWriter writer, ref TEvent value, MetadataJsonSerializerOptions options)
				{
						base.Write(writer, ref value, options);
				}
		}
		internal class SetVFXPreset : MemberConverter<Events.SetVFXPreset>
		{
				protected override bool Read(ref Utf8JsonReader reader, ref Events.SetVFXPreset value, MetadataJsonSerializerOptions options)
				{
						if (base.Read(ref reader, ref value, options))
								return true;
						if (reader.ValueTextEquals("rooms"u8) && reader.Read())
								value.Rooms = TypeConverterRegistry.Read<Room>(ref reader, options);
						else if (reader.ValueTextEquals("preset"u8) && reader.Read())
								if (reader.TokenType is JsonTokenType.String && EnumConverter.TryParse(ref reader, out VfxPreset enumValue0))
										value.Preset = enumValue0;
								else if (reader.TokenType is JsonTokenType.Number && reader.TryGetInt32(out int intValue0))
										value.Preset = (VfxPreset)intValue0;
								else
										value.Preset = default;
						else if (reader.ValueTextEquals("enable"u8) && reader.Read())
								if (reader.TokenType is JsonTokenType.True or JsonTokenType.False)
										value.Enable = reader.GetBoolean();
								else if (reader.TokenType is JsonTokenType.String)
										value.Enable = "Enabled" == reader.GetString();
								else
										value.Enable = false;
						else if (reader.ValueTextEquals("threshold"u8) && reader.Read())
								value.Threshold = reader.GetSingle();
						else if (reader.ValueTextEquals("intensity"u8) && reader.Read())
								value.Intensity = reader.GetSingle();
						else if (reader.ValueTextEquals("color"u8) && reader.Read())
								value.Color = TypeConverterRegistry.Read<PaletteColor>(ref reader, options);
						else if (reader.ValueTextEquals("floatX"u8) && reader.Read())
						{
								if (reader.TokenType is not JsonTokenType.Null)
								{
										var p = value.Amount ?? new();
										p.X = reader.GetSingle();
										value.Amount = p;
								}
						}
						else if (reader.ValueTextEquals("floatY"u8) && reader.Read())
						{
								if (reader.TokenType is not JsonTokenType.Null)
								{
										var p = value.Amount ?? new();
										p.Y = reader.GetSingle();
										value.Amount = p;
								}
						}
						else if (reader.ValueTextEquals("amount"u8) && reader.Read())
								value.Amount = TypeConverterRegistry.Read<Point>(ref reader, options);
						else if (reader.ValueTextEquals("speedPerc"u8) && reader.Read())
								value.SpeedPercentage = reader.GetSingle();
						else if (reader.ValueTextEquals("ease"u8) && reader.Read())
								if (reader.TokenType is JsonTokenType.String && EnumConverter.TryParse(ref reader, out Global.Components.Easing.EaseType enumValue1))
										value.Ease = enumValue1;
								else if (reader.TokenType is JsonTokenType.Number && reader.TryGetInt32(out int intValue1))
										value.Ease = (Global.Components.Easing.EaseType)intValue1;
								else
										value.Ease = default;
						else if (reader.ValueTextEquals("duration"u8) && reader.Read())
								value.Duration = reader.GetSingle();
						else return false;
						return true;
				}
				protected override void Write(Utf8JsonWriter writer, ref Events.SetVFXPreset value, MetadataJsonSerializerOptions options)
				{
						base.Write(writer, ref value, options);
						{ writer.WritePropertyName("rooms"u8); TypeConverterRegistry.Write(writer, value.Rooms, options); }
						writer.WriteString("preset"u8, value.Preset.ToEnumString());
						if (value.Preset is not VfxPreset.DisableAll)
								writer.WriteBoolean("enable"u8, value.Enable);
						if (value.Enable && value.Preset is VfxPreset.Bloom && value.Threshold is float valueNotNull0)
								writer.WriteNumber("threshold"u8, valueNotNull0);
						if (value.Enable && VfxAttributes[value.Preset].HasFlag(VfxAttribute.EnableIntensity) && value.Intensity is float valueNotNull1)
								writer.WriteNumber("intensity"u8, valueNotNull1);
						if (value.Enable && VfxAttributes[value.Preset].HasFlag(VfxAttribute.EnableColor) && value.Color is PaletteColor valueNotNull2)
						{ writer.WriteString("color"u8, valueNotNull2.Serialize()); }
						if (value.Enable && VfxAttributes[value.Preset].HasFlag(VfxAttribute.EnableAbsoluteXY) && value.Amount is Point valueNotNull3)
						{ writer.WritePropertyName("amount"u8); TypeConverterRegistry.Write(writer, valueNotNull3, options); }
						if (value.Enable && VfxAttributes[value.Preset].HasFlag(VfxAttribute.EnableSpeed) && value.SpeedPercentage is float valueNotNull4)
								writer.WriteNumber("speedPerc"u8, valueNotNull4);
						if (value.Enable && VfxAttributes[value.Preset].HasFlag(VfxAttribute.EnableEase))
								writer.WriteString("ease"u8, value.Ease.ToEnumString());
						if (value.Enable && VfxAttributes[value.Preset].HasFlag(VfxAttribute.EnableEase))
								writer.WriteNumber("duration"u8, value.Duration);
				}
		}
}