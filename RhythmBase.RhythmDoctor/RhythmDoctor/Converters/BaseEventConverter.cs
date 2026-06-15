using RhythmBase.Global.Converters;
using RhythmBase.RhythmDoctor.Events;
using System.Text.Json;

namespace RhythmBase.RhythmDoctor.Converters;

internal class BaseEventConverter : MetadataJsonConverter<IBaseEvent>
{
	private class EventUpgrater
	{
		public int MaxVersion { get; init; }
		public required Action<IBaseEvent> UpgrateFunc { get; init; }
		public required EventType Type { get; init; }
	}
	private static class EventUpgraterCollection
	{
		private static readonly List<EventUpgrater> _upgraters = new();
		public static readonly EnumCollection<EventType> TypeHasUpgrater = new();
		public static int MaxVersion;
		public static void Add<T>(int version, Action<IBaseEvent> upgrateAction) where T : IBaseEvent, new()
		{
			var type = EventTypeRegistry.ToEnum<T>();
			MaxVersion = int.Max(MaxVersion, version);
			TypeHasUpgrater.Add(type);
			_upgraters.Add(new EventUpgrater()
			{
				MaxVersion = version,
				Type = type,
				UpgrateFunc = upgrateAction
			});
		}
		public static IEnumerable<EventUpgrater> GetUpgraters(int version, EventType type)
		{
			foreach (EventUpgrater upgrater in _upgraters)
				if (upgrater.Type == type && upgrater.MaxVersion >= version)
					yield return upgrater;
		}
		static EventUpgraterCollection()
		{
			// 在这里注册升级器
			Add<AddOneshotBeat>(58, e =>
			{
				if (e is AddOneshotBeat ate
					&& ate.FreezeBurnMode == OneshotType.Wave
					&& ate.Loop == 0
					&& ate.Skipshot
					&& ate.Interval == 2f)
					ate.Interval = ate.Tick * 2;
			});
			Add<SetClapSounds>(49, static e =>
			{
				if (e is SetClapSounds scs)
					scs.CpuSound = scs.P2Sound;
			});
			Add<PlaySong>(4, static e =>
			{
				if (e is PlaySong ps) ps.Song.Volume += 30;
			});
			Add<PlaySong>(12, static e =>
			{
				if (e is PlaySong ps) ps.Song.Volume += 10;
			});
			Add<PlaySong>(41, static e =>
			{
				if (e is PlaySong ps && IsWaveFile(ps.Song.Filename))
					ps.Song.Volume = (int)((ps.Song.Volume - 40f) * 0.88f);
			});
			Add<SetRowXs>(57, static e =>
			{
				if (e is SetRowXs srx)
				{
					srx.SyncoVolume = 0;
					srx.SyncoPlayModifierSound = false;
					srx.SyncoPlayModifierOffSound = false;
				}
			});
			Add<SetRowXs>(63, static e =>
			{
				if (e is SetRowXs srx)
					srx.SyncoPlayModifierOffSound = srx.SyncoPlayModifierSound;
			});
			Add<ChangePlayersRows>(24, static e =>
			{
				if (e is ChangePlayersRows cpr)
					cpr.FlashOnBeat = false;
			});
			Add<NewWindowDance>(55, static e =>
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
			Add<NewWindowDance>(62, static e =>
			{
				if (e is not NewWindowDance nwd)
					return;
				if (nwd.Preset == WindowDancePreset.Ellipse)
					nwd.Speed *= 100f;
			});
			Add<ShowStatusSign>(50, static e =>
			{
				if (e is ShowStatusSign sss)
					sss.Narrate = false;
			});
			Add<ShowDialogue>(43, static e =>
			{
				if (e is ShowDialogue sd)
					sd.PlayTextSounds = false;
			});
			Add<SetVFXPreset>(67, static e =>
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
			Add<MoveCamera>(24, static e =>
			{
				if (e is MoveCamera mc) mc.Rooms = new Components.Room() { [RoomIndex.RoomTop] = true };
			});
			Add<FloatingText>(50, static e =>
			{
				if (e is FloatingText ft) ft.Narrate = false;
			});
			Add<PaintHands>(65, static e =>
			{
				if (e is not PaintHands ph)
					return;
				ph.Border ??= Border.None;
				ph.IsTint ??= false;
				ph.Opacity ??= 100;
			});
			Add<MoveRow>(24, static e =>
			{
				if (e is MoveRow mr) mr.EnableCustomPosition = true;
			});
			Add<SetTheme>(8, static e =>
			{
				if (e is SetTheme st
					&& st.Preset == Theme.Kaleidoscope)
					st.Preset = Theme.HallOfMirrors;
			});
			Add<TintRows>(53, static e =>
			{
				if (e is TintRows tr) tr.Rooms = new Components.Room() { [RoomIndex.RoomTop] = true };
			});
			Add<TintRows>(65, static e =>
			{
				if (e is not TintRows tr)
					return;
				tr.Border ??= Border.None;
				tr.IsTint ??= false;
				tr.Opacity ??= 100;
				tr.Effect ??= TintRowEffect.None;
			});
		}
	}
	private LevelReadSettings _rs = new();
	private LevelWriteSettings _ws = new();
	public BaseEventConverter WithReadSettings(LevelReadSettings settings)
	{
		_rs = settings;
		return this;
	}
	public BaseEventConverter WithWriteSettings(LevelWriteSettings settings)
	{
		_ws = settings;
		return this;
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
			e = ReadForwardEvent(ref reader) ?? (new ForwardEvent() { ActualType = type ?? "" });
		else
			e = EventConverterMap.GetConverter(typeEnum).WithReadSettings(_rs).ReadProperties(ref reader, options);
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.EndObject);
		reader.Read();
		if (options.UpgradeToLatest && options.Version < EventUpgraterCollection.MaxVersion && EventUpgraterCollection.TypeHasUpgrater.Contains(e.Type))
			foreach (var upgrater in EventUpgraterCollection.GetUpgraters(options.Version, e.Type))
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
			EventConverterMap.GetConverter(value.Type).WithWriteSettings(_ws).WriteProperties(writer, value, options);
		}
	}
	public static Events.IForwardEvent? ReadForwardEvent(ref Utf8JsonReader reader)
	{
		using JsonDocument doc = JsonDocument.ParseValue(ref reader);
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
		if (hasRow) return new ForwardRowEvent(doc);
		else return hasTarget ? new ForwardDecorationEvent(doc) : new ForwardEvent(doc);
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