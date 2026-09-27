using RhythmBase.Global.Components.Vector;
using RhythmBase.Global.Serialization;
using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Events;
using RhythmBase.RhythmDoctor.Serialization;
using System.Text.Json;

namespace RhythmBase.RhythmDoctor.Serialization;

internal partial class RDMemberConverter
{
	internal abstract class BaseRowAction<TEvent> : MemberConverter<TEvent> where TEvent : BaseRowAction, new()
	{
		protected override bool Read(ref Utf8JsonReader reader, ref TEvent value, MetadataJsonSerializerOptions options)
		{
			if (base.Read(ref reader, ref value, options))
				return true;
			if (reader.ValueTextEquals("row"u8) && reader.Read())
				value.Row = reader.GetInt32();
			else
				return false;
			return true;
		}
		protected override void Write(Utf8JsonWriter writer, ref TEvent value, MetadataJsonSerializerOptions options)
		{
			base.Write(writer, ref value, options);
			writer.WriteNumber("row"u8, value.Parent?.Index ?? value.Row);
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
				value.Target = reader.GetString() ?? "";
			else
				return false;
			return true;
		}
		protected override void Write(Utf8JsonWriter writer, ref TEvent value, MetadataJsonSerializerOptions options)
		{
			base.Write(writer, ref value, options);
			if (value is not Comment cmt || cmt.CustomTab == Tab.Decorations)
				writer.WriteString("target"u8, value.Parent?.Id ?? value.Target);
			else
				writer.WriteNumber("y"u8, value.Y);
		}
	}
	internal abstract class BaseBeatsPerMinute<TEvent> : MemberConverter<TEvent> where TEvent : BaseBeatsPerMinute, new()
	{
		protected override bool Read(ref Utf8JsonReader reader, ref TEvent value, MetadataJsonSerializerOptions options)
		{
			if (base.Read(ref reader, ref value, options)) return true;
			if (value is PlaySong v1 && reader.ValueTextEquals("bpm"u8) && reader.Read())
				value.BeatsPerMinute = reader.GetSingle();
			else if (value is SetBeatsPerMinute v2 && reader.ValueTextEquals("beatsPerMinute"u8) && reader.Read())
				value.BeatsPerMinute = reader.GetSingle();
			else return false;
			return true;
		}
		protected override void Write(Utf8JsonWriter writer, ref TEvent value, MetadataJsonSerializerOptions options)
		{
			base.Write(writer, ref value, options);
			if (value is PlaySong v1)
				writer.WriteNumber("bpm"u8, value.BeatsPerMinute);
			else if (value is SetBeatsPerMinute v2)
				writer.WriteNumber("beatsPerMinute"u8, value.BeatsPerMinute);
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
	internal class SetVFXPreset : MemberConverter<Events.SetVfxPreset>
	{
		protected override bool Read(ref Utf8JsonReader reader, ref Events.SetVfxPreset value, MetadataJsonSerializerOptions options)
		{
			if (base.Read(ref reader, ref value, options))
				return true;
			if (reader.ValueTextEquals("rooms"u8) && reader.Read())
				value.Rooms = TypeConverterRegistry.Read<Room>(ref reader, options);
			else if (reader.ValueTextEquals("preset"u8) && reader.Read())
			{
				if (reader.TokenType is JsonTokenType.String && EnumConverter.TryParse(ref reader, out VfxPreset enumValue0))
					value.Preset = enumValue0;
				else if (reader.TokenType is JsonTokenType.Number && reader.TryGetInt32(out int intValue0))
					value.Preset = (VfxPreset)intValue0;
				else
					value.Preset = default;
				if (false && value.Preset is VfxPreset.HeatDistortion) // Temporarily disabled: migrated to RhythmDoctorUpgrader.
					value.Position = (100, 100);
			}
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
			else if (reader.ValueTextEquals("xySpeed"u8) && reader.Read())
				value.XYSpeed = TypeConverterRegistry.Read<Point>(ref reader, options);
			else if (reader.ValueTextEquals("position"u8) && reader.Read())
				value.Position = TypeConverterRegistry.Read<Point>(ref reader, options);
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
		protected override void Write(Utf8JsonWriter writer, ref Events.SetVfxPreset value, MetadataJsonSerializerOptions options)
		{
			base.Write(writer, ref value, options);
			{ TypeConverterRegistry.Write(writer, "rooms"u8, value.Rooms, options); }
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
			{ TypeConverterRegistry.Write(writer, "amount"u8, valueNotNull3, options); }
			if (value.Enable && VfxAttributes[value.Preset].HasFlag(VfxAttribute.EnableSpeed) && value.SpeedPercentage is float valueNotNull4)
				writer.WriteNumber("speedPerc"u8, valueNotNull4);
			if (value.Enable && VfxAttributes[value.Preset].HasFlag(VfxAttribute.EnableEase))
				writer.WriteString("ease"u8, value.Ease.ToEnumString());
			if (value.Enable && VfxAttributes[value.Preset].HasFlag(VfxAttribute.EnableEase))
				writer.WriteNumber("duration"u8, value.Duration);
			if (value.Enable && VfxAttributes[value.Preset].HasFlag(VfxAttribute.EnablePosition) && value.Position is Point valueNotNull5)
				TypeConverterRegistry.Write(writer, "position"u8, valueNotNull5, options);
			if (value.Enable && VfxAttributes[value.Preset].HasFlag(VfxAttribute.EnableXY) && value.Amount is Point valueNotNull6)
				TypeConverterRegistry.Write(writer, "xySpeed"u8, valueNotNull6, options);
		}
	}
	internal class GoToLevel : MemberConverter<Events.GoToLevel>
	{
		protected override bool Read(ref Utf8JsonReader reader, ref Events.GoToLevel value, MetadataJsonSerializerOptions options)
		{
			if (base.Read(ref reader, ref value, options))
				return true;
			if (reader.ValueTextEquals("action"u8) && reader.Read())
			{ if (EnumConverter.TryParse(ref reader, out GoToLevelAction enumValue0)) value.Action = enumValue0; else return false; }
			else if (reader.ValueTextEquals("rdlevel"u8) && reader.Read())
				value.Chart = TypeConverterRegistry.Read<FileReference>(ref reader, options);
			else if (reader.ValueTextEquals("dontUpdateRestart"u8) && reader.Read())
				value.DontUpdateRestart = reader.GetBoolean();
			else if (reader.ValueTextEquals("fadeOut"u8) && reader.Read())
				value.FadeOut = reader.GetBoolean();
			else if (reader.ValueTextEquals("keepMistakes"u8) && reader.Read())
				value.KeepMistakes = reader.GetBoolean();
			else if (reader.ValueTextEquals("skippable"u8) && reader.Read())
				value.Skippable = reader.GetBoolean();
			else if (reader.ValueTextEquals("startImmediately"u8) && reader.Read())
				value.StartImmediately = reader.GetBoolean();
			else return false;
			return true;
		}
		protected override void Write(Utf8JsonWriter writer, ref Events.GoToLevel value, MetadataJsonSerializerOptions options)
		{
			base.Write(writer, ref value, options);
			writer.WriteString("action"u8, value.Action.ToEnumUtf8String());
			if (value.Action is not GoToLevelAction.LoadNext)
				writer.WritePropertyName("rdlevel"u8); TypeConverterRegistry.Write(writer, value.Chart, options);
			if (!value.Chart.IsEmpty)
			{
				writer.WriteBoolean("dontUpdateRestart"u8, value.DontUpdateRestart);
				writer.WriteBoolean("fadeOut"u8, value.FadeOut);
				writer.WriteBoolean("keepMistakes"u8, value.KeepMistakes);
				writer.WriteBoolean("startImmediately"u8, value.StartImmediately);
			}
			if (value.Action is not GoToLevelAction.SetNext)
				writer.WriteBoolean("skippable"u8, value.Skippable);
		}
	}
	internal class SetGameSound : MemberConverter<Events.SetGameSound>
	{
		protected override bool Read(ref Utf8JsonReader reader, ref Events.SetGameSound value, MetadataJsonSerializerOptions options)
		{
			if (base.Read(ref reader, ref value, options))
				return true;
			if (reader.ValueTextEquals("soundType"u8) && reader.Read())
			{
				if (EnumConverter.TryParse(ref reader, out SoundType type))
					value.SoundType = type;
				else
					return false;
				return true;
			}
			if (reader.ValueTextEquals("sounds"u8) && reader.Read())
			{
				// The array is positional: entries align with the group members of soundType.
				SoundType type = value.SoundType;
				if (!Constants.SoundGroupTypeMap.ContainsKey(type))
				{
					// soundType may appear after sounds; resolve it with a split reader.
					SoundType? scanned = ScanSoundType(ref reader);
					if (scanned is SoundType scannedType)
						type = scannedType;
				}
				value.Sounds = BuildCollection(type, ReadAudioSlots(ref reader));
				return true;
			}
			return false;
		}
		protected override void Write(Utf8JsonWriter writer, ref Events.SetGameSound value, MetadataJsonSerializerOptions options)
		{
			base.Write(writer, ref value, options);
			writer.WriteString("soundType"u8, value.SoundType.ToEnumUtf8String());
			writer.WritePropertyName("sounds"u8);
			writer.WriteStartArray();
			foreach (KeyValuePair<SoundType, Audio?> slot in value.Sounds)
			{
				writer.WriteStartObject();
				Audio? audio = slot.Value;
				if (audio is null)
					writer.WriteBoolean("used"u8, false);
				else
				{
					writer.WriteString("filename"u8, audio.Filename);
					if (audio.Volume != 100)
						writer.WriteNumber("volume"u8, audio.Volume);
					if (audio.Pitch != 100)
						writer.WriteNumber("pitch"u8, audio.Pitch);
					if (audio.Pan != 0)
						writer.WriteNumber("pan"u8, audio.Pan);
					if (audio.Offset != TimeSpan.Zero)
						writer.WriteNumber("offset"u8, audio.Offset.TotalMilliseconds);
				}
				writer.WriteEndObject();
			}
			writer.WriteEndArray();
		}

		private static SoundCollection BuildCollection(SoundType soundType, List<Audio?> slots)
		{
			if (slots.Count > 1 && Constants.SoundGroupTypeMap.TryGetValue(soundType, out SoundType[]? members))
			{
				SoundCollection group = new(members);
				for (int i = 0; i < members.Length && i < slots.Count; i++)
					group._values[i] = slots[i];
				return group;
			}
			return new SoundCollection.SingleAudioSoundCollection(SoundType.ClapSoundP1Classic)
			{
				Audio = slots.Count > 0 ? slots[0] : null,
			};
		}

		private static List<Audio?> ReadAudioSlots(ref Utf8JsonReader reader)
		{
			List<Audio?> slots = [];
			while (reader.Read() && reader.TokenType is not JsonTokenType.EndArray)
			{
				if (reader.TokenType is not JsonTokenType.StartObject)
				{
					reader.Skip();
					slots.Add(null);
					continue;
				}

				Audio audio = new();
				bool used = true;
				while (reader.Read() && reader.TokenType is not JsonTokenType.EndObject)
				{
					if (reader.TokenType is not JsonTokenType.PropertyName)
						continue;
					if (reader.ValueTextEquals("used"u8) && reader.Read())
						used = reader.GetBoolean();
					else if (reader.ValueTextEquals("filename"u8) && reader.Read())
						audio.Filename = reader.GetString() ?? "";
					else if (reader.ValueTextEquals("volume"u8) && reader.Read())
						audio.Volume = reader.GetInt32();
					else if (reader.ValueTextEquals("pitch"u8) && reader.Read())
						audio.Pitch = reader.GetInt32();
					else if (reader.ValueTextEquals("pan"u8) && reader.Read())
						audio.Pan = reader.GetInt32();
					else if (reader.ValueTextEquals("offset"u8) && reader.Read())
						audio.Offset = TimeSpan.FromMilliseconds(reader.GetSingle());
					else
					{
						reader.Read();
						reader.Skip();
					}
				}
				slots.Add(used ? audio : null);
			}
			return slots;
		}

		private static SoundType? ScanSoundType(ref Utf8JsonReader reader)
		{
			Utf8JsonReader scan = reader;
			scan.Skip();
			while (scan.Read())
			{
				if (scan.TokenType is JsonTokenType.EndObject)
					break;
				if (scan.TokenType is not JsonTokenType.PropertyName)
					break;
				if (scan.ValueTextEquals("soundType"u8))
				{
					scan.Read();
					return scan.TokenType is JsonTokenType.String && EnumConverter.TryParse(ref scan, out SoundType type)
						? type
						: null;
				}
				scan.Read();
				scan.Skip();
			}
			return null;
		}
	}
}