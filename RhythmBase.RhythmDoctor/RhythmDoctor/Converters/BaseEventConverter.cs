using RhythmBase.Global.Converters;
using RhythmBase.RhythmDoctor.Events;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace RhythmBase.RhythmDoctor.Converters;

internal class BaseEventConverter : BackwardCompatibleMetadataJsonConverter
{
	protected override void InitializeUpgraters()
	{
		// 在这里注册升级器
		Register<AddOneshotBeat>(58, e =>
		{
			if (e is AddOneshotBeat ate
				&& ate.FreezeBurnMode == OneshotType.Wave
				&& ate.Loop == 0
				&& ate.Skipshot
				&& ate.Interval == 2f)
				ate.Interval = ate.Tick * 2;
		});
		Register<SetClapSounds>(49, static e =>
		{
			if (e is SetClapSounds scs)
				scs.CpuSound = scs.P2Sound;
		});
		Register<PlaySong>(4, static e =>
		{
			if (e is PlaySong ps) ps.Song.Volume += 30;
		});
		Register<PlaySong>(12, static e =>
		{
			if (e is PlaySong ps) ps.Song.Volume += 10;
		});
		Register<PlaySong>(41, static e =>
		{
			if (e is PlaySong ps && IsWaveFile(ps.Song.Filename))
				ps.Song.Volume = (int)((ps.Song.Volume - 40f) * 0.88f);
		});
		Register<SetRowXs>(57, static e =>
		{
			if (e is SetRowXs srx)
			{
				srx.SyncoVolume = 0;
				srx.SyncoPlayModifierSound = false;
				srx.SyncoPlayModifierOffSound = false;
			}
		});
		Register<SetRowXs>(63, static e =>
		{
			if (e is SetRowXs srx)
				srx.SyncoPlayModifierOffSound = srx.SyncoPlayModifierSound;
		});
		Register<ChangePlayersRows>(24, static e =>
		{
			if (e is ChangePlayersRows cpr)
				cpr.FlashOnBeat = false;
		});
		Register<NewWindowDance>(55, static e =>
		{
			if (e is not NewWindowDance nwd)
				return;
			var usePositionElement = nwd["usePosition"];
			if (usePositionElement.ValueKind == JsonValueKind.String)
			{
				var usePosition = nwd["usePosition"].GetString();
				if (usePosition == "Current")
					nwd.Position = (null, null);
			}
			if (nwd.Angle is float angle1)
			{
				float round1 = angle1 % (float)360f;
				nwd.Angle = (float)90f - round1;
			}
			switch (nwd.Preset)
			{
				case WindowDancePreset.Sway:
					nwd.SubEaseType = nwd.Ease;
					nwd.Frequency = nwd.Duration == 0 ? 0 : 1f / nwd.Duration;
					break;
				case WindowDancePreset.Wrap:
					nwd.Amplitude *= 2f;
					nwd.AmplitudeVector *= 2f;
					break;
				case WindowDancePreset.ShakePer:
					nwd.SubEaseType = nwd.Ease;
					nwd.Frequency = 1f / float.Max(nwd.Period, 1f / 1000f);
					nwd.Period = nwd.Duration;
					nwd.Duration = 0f;
					break;
			}
		});
		Register<NewWindowDance>(62, static e =>
		{
			if (e is not NewWindowDance nwd)
				return;
			if (nwd.Preset == WindowDancePreset.Ellipse)
				nwd.Speed *= 100f;
		});
		Register<ShowStatusSign>(50, static e =>
		{
			if (e is ShowStatusSign sss)
				sss.Narrate = false;
		});
		Register<ShowDialogue>(43, static e =>
		{
			if (e is ShowDialogue sd)
				sd.PlayTextSounds = false;
		});
		Register<SetVFXPreset>(67, static e =>
		{
			if (e is not SetVFXPreset svp)
				return;
			switch (svp.Preset)
			{
				case VfxPreset.Drawing:
					svp.Color = Color.Black;
					svp.SpeedPercentage = 100f;
					break;
				case VfxPreset.RadialBlur:
					svp.Amount = (1f, 1f);
					break;
			}
		});
		Register<MoveCamera>(24, static e =>
		{
			if (e is MoveCamera mc) mc.Rooms = new Components.Room() { [RoomIndex.RoomTop] = true };
		});
		Register<FloatingText>(50, static e =>
		{
			if (e is FloatingText ft) ft.Narrate = false;
		});
		Register<PaintHands>(65, static e =>
		{
			if (e is not PaintHands ph)
				return;
			ph.Border ??= Border.None;
			ph.IsTint ??= false;
			ph.Opacity ??= 100;
		});
		Register<MoveRow>(24, static e =>
		{
			if (e is MoveRow mr) mr.EnableCustomPosition = true;
		});
		Register<SetTheme>(8, static e =>
		{
			if (e is SetTheme st
				&& st.Preset == Theme.Kaleidoscope)
				st.Preset = Theme.HallOfMirrors;
		});
		Register<TintRows>(53, static e =>
		{
			if (e is TintRows tr) tr.Rooms = new Components.Room() { [RoomIndex.RoomTop] = true };
		});
		Register<TintRows>(65, static e =>
		{
			if (e is not TintRows tr)
				return;
			tr.Border ??= Border.None;
			tr.IsTint ??= false;
			tr.Opacity ??= 100;
			tr.Effect ??= TintRowEffect.None;
		});
	}
	public override bool CanConvert(Type typeToConvert)
	{
		return Type.IsAssignableFrom(typeToConvert);
	}
	public override IBaseEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
	{
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
		string? type = null;
		Utf8JsonReader checkpoint = reader;
		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
		{
			if (reader.TokenType == JsonTokenType.PropertyName)
			{
				if (reader.ValueTextEquals("type"u8))
				{
					reader.Read();
					type = reader.GetString();
					break;
				}
				else
				{
					reader.Skip();
				}
			}
		}
		reader = checkpoint; IBaseEvent e;
		if (type is null || !Enum.TryParse(type, true, out EventType typeEnum))
			e = ReadForwardEvent(ref reader) ?? throw new JsonException("Unknown event type and failed to parse.");
		else
			e = EventConverterMap.GetConverter(typeEnum).ReadProperties(ref reader, options);
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.EndObject);
		if (options.UpgradeToLatest && options.Version < MaxVersion && TypeHasUpgrater.Contains(e.Type))
			foreach (var upgrater in GetUpgraters(options.Version, e.Type))
				upgrater.UpgrateFunc(e);
		return e;
	}
	public override void Write(Utf8JsonWriter writer, IBaseEvent value, MetadataJsonSerializerOptions options)
	{
		if (value is Events.IForwardEvent ce)
		{
			WriteForwardEvent(writer, ce);
			return;
		}
		else
		{
			EventConverterMap.GetConverter(value.Type).WriteProperties(writer, value, options);
		}
	}
	public static Events.IForwardEvent? ReadForwardEvent(ref Utf8JsonReader reader)
	{
		JsonDocument doc = JsonDocument.ParseValue(ref reader);
		JsonElement root = doc.RootElement;

		// 判断属性
		bool hasRow = false, hasTarget = false;
		foreach (JsonProperty prop in root.EnumerateObject())
		{
			if (prop.NameEquals("row"))
				hasRow = true;
			else if (prop.NameEquals("target"))
				hasTarget = true;
		}
		return 
			hasRow ? new ForwardRowEvent(doc) :
			hasTarget ? new ForwardDecorationEvent(doc) :
			new ForwardEvent(doc);
	}

	public static void WriteForwardEvent(Utf8JsonWriter writer, Events.IForwardEvent value)
	{
		(int bar, float beat) = value.TickTime;
		writer.WriteStartObject();
		if (!string.IsNullOrEmpty(value.ActualType))
			writer.WriteString("type", value.ActualType);
		writer.WriteNumber("bar", bar);
		writer.WriteNumber("beat", beat);
		if (value is ForwardRowEvent rowEvent)
			writer.WriteNumber("row", rowEvent.Index);
		else if (value is ForwardDecorationEvent decorationEvent)
			writer.WriteString("target", decorationEvent.Target);
		if (!string.IsNullOrEmpty(value.Tag))
			writer.WriteString("tag", value.Tag);
		if (value.RunTag)
			writer.WriteBoolean("runTag", value.RunTag);
		if (!value.Active)
			writer.WriteBoolean("active", value.Active);
		if (value.Condition.HasValue)
			writer.WriteString("if", value.Condition.Serialize());
		if (value.Y != 0)
			writer.WriteNumber("y", value.Y);

		foreach (KeyValuePair<string, JsonElement> kv in ((BaseEvent)value)._extraData)
		{
			writer.WritePropertyName(kv.Key);
			kv.Value.WriteTo(writer);
		}
		writer.WriteEndObject();
	}
}