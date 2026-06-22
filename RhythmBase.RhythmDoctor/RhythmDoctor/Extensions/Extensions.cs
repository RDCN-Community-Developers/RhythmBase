using RhythmBase.Global.Components.RichText;
using RhythmBase.Global.Converters;
using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Converters;
using RhythmBase.RhythmDoctor.Events;
using RhythmBase.RhythmDoctor.Utils;
using System.Text.Json;
namespace RhythmBase.RhythmDoctor.Extensions;

/// <summary>
/// Extensions
/// </summary>
public static partial class Extensions
{
#if NETSTANDARD
	internal static T FirstOrDefault<T>(this IEnumerable<T> e, T defaultValue) => e.FirstOrDefault(defaultValue);
	internal static T FirstOrDefault<T>(this IEnumerable<T> e, Func<T, bool> predicate, T defaultValue) => e.FirstOrDefault(predicate, defaultValue);
	internal static T LastOrDefault<T>(this IEnumerable<T> e, T defaultValue) => e.LastOrDefault(defaultValue);
	internal static T LastOrDefault<T>(this IEnumerable<T> e, Func<T, bool> predicate, T defaultValue) => e.LastOrDefault(predicate, defaultValue);
#endif

	extension(UnhandledFieldRegistry)
	{
		/// <summary>
		/// Registers a handler that silently ignores the specified field on the given type.
		/// </summary>
		/// <typeparam name="T">The event type that contains the field.</typeparam>
		/// <param name="fieldName">The name of the field to ignore.</param>
		public static void Keep<T>(string fieldName) where T : IBaseEvent
			=> UnhandledFieldRegistry.Register<T>(fieldName, (ref e, value) => { e["usePosition"] = value; return true; });
	}
	extension(Condition e)
	{
		/// <summary>
		/// Gets or sets a value indicating whether the game is in two-player mode.
		/// </summary>
		public bool? TwoPlayerMode { get => e['p']; set => e['p'] = value; }
		/// <summary>
		/// Gets or sets a value indicating whether the flash effect is enabled.
		/// </summary>
		public bool? EnableFlashEffect { get => e['f']; set => e['f'] = value; }
		/// <summary>
		/// Gets or sets a value indicating whether the narration is enabled.
		/// </summary>
		public bool? EnableNarration { get => e['n']; set => e['n'] = value; }
		/// <summary>
		/// Gets or sets a value indicating whether the event should run only once.
		/// </summary>
		public bool? RunOnlyOnce { get => e['o']; set => e['o'] = value; }
	}
	extension(PaletteColor e)
	{
		/// <summary>
		/// Converts the <see cref="PaletteColor"/> to an in-game color based on the provided level's color palette and the properties of the <see cref="PaletteColor"/>.
		/// </summary>
		/// <param name="level">The level containing the color palette.</param>
		/// <returns>The in-game color.</returns>
		public Color ToColor(Level level) => e.EnablePanel ? level.ColorPalette[e.PaletteIndex] : e.Color.WithAlpha(255);
	}
	extension(PaletteColorWithAlpha e)
	{
		/// <summary>
		/// Converts the <see cref="PaletteColorWithAlpha"/> to an in-game color based on the provided level's color palette and the properties of the <see cref="PaletteColorWithAlpha"/>.
		/// </summary>
		/// <param name="level">The level containing the color palette.</param>
		/// <returns>The in-game color.</returns>
		public Color ToColor(Level level) => e.EnablePanel ? level.ColorPalette[e.PaletteIndex] : e.Color;
	}
	extension(string e)
	{
		/// <summary>
		/// Make strings follow the Upper Camel Case.
		/// </summary>
		/// <returns>The result.</returns>
		public string ToUpperCamelCase()
		{
			char[] S = [.. e];
			S[0] = S[0].ToString().ToUpper()[0];
			return string.Join("", [new string(S)]);
		}
		/// <summary>
		/// Make strings follow the Lower Camel Case.
		/// </summary>
		public string ToLowerCamelCase()
		{
			char[] S = [.. e];
			S[0] = S[0].ToString().ToLower()[0];
			return string.Join("", [new string(S)]);
		}
	}

	extension(float beat)
	{
		/// <summary>
		/// Calculate the fraction of <paramref name="splitBase" /> equal to the nearest floating point number.
		/// <example>
		/// <code>
		/// 2.236f.QuantizeBeat(4) == 2.25f
		/// float.Pi.QuantizeBeat(5) == 3.2f
		/// float.E.QuantizeBeat(2) == 2.5f
		/// </code>
		/// </example>
		/// </summary>
		/// <param name="splitBase">Indicate what fraction this is.</param>
		public float QuantizeBeat(uint splitBase) => (float)(Math.Round(beat * splitBase) / splitBase);
	}

	extension(double beat)
	{
		/// <summary>
		/// Calculate the fraction of <paramref name="splitBase" /> equal to the nearest floating point number.
		/// <example>
		/// <code>
		/// 2.236f.QuantizeBeat(4) == 2.25f
		/// float.Pi.QuantizeBeat(5) == 3.2f
		/// float.E.QuantizeBeat(2) == 2.5f
		/// </code>
		/// </example>
		/// </summary>
		/// <param name="splitBase">Indicate what fraction this is.</param>
		public double QuantizeBeat(uint splitBase) => Math.Round(beat * splitBase) / splitBase;
	}

	extension(TickTime beat)
	{
		/// <summary>
		/// Calculate the fraction of <paramref name="splitBase" /> equal to the nearest floating point number.
		/// <example>
		/// <code>
		/// 2.236f.QuantizeBeat(4) == 2.25f
		/// float.Pi.QuantizeBeat(5) == 3.2f
		/// float.E.QuantizeBeat(2) == 2.5f
		/// </code>
		/// </example>
		/// </summary>
		/// <param name="splitBase">Indicate what fraction this is.</param>
		public TickTime QuantizeBeat(uint splitBase) => beat._calculator is BeatCalculator bc ? new(bc, beat.Tick.QuantizeBeat(splitBase)) : new(beat.Tick.QuantizeBeat(splitBase));
	}

	extension(Bookmark.BookmarkColors e)
	{
		/// <summary>
		/// Converting enumeration constants to in-game colors。
		/// </summary>
		/// <returns>The in-game color.</returns>
		public Color ToColor() => e switch
		{
			Bookmark.BookmarkColors.Blue => Color.FromRgba(11, 125, 206),
			Bookmark.BookmarkColors.Red => Color.FromRgba(219, 41, 41),
			Bookmark.BookmarkColors.Yellow => Color.FromRgba(212, 212, 51),
			Bookmark.BookmarkColors.Green => Color.FromRgba(54, 215, 54),
			_ => throw new NotSupportedException(),
		};
	}

	extension<TEvent>(OrderedEventCollection<TEvent, EventType, TickTime> e) where TEvent : IBaseEvent
	{
		/// <summary>
		/// Add a range of events.
		/// </summary>
		/// <param name="items">The events to add.</param>
		public void AddRange(IEnumerable<TEvent> items)
		{
			foreach (TEvent item in items)
				e.Add(item);
		}

		/// <summary>
		/// Remove a range of events.
		/// </summary>
		/// <param name="items">A range of events.</param>
		/// <returns>The number of events successfully removed.</returns>
		public int RemoveRange(IEnumerable<TEvent> items)
		{
			int count = 0;
			foreach (TEvent item in items)
				count += e.Remove(item) ? 1 : 0;
			return count;
		}
		/// <summary>
		/// Get all events with the specified tag.
		/// </summary>
		/// <param name="name">Tag name.</param>
		/// <param name="strict">Indicates whether the label is strictly matched.
		/// If <see langword="true" />, determine If it contains the specified tag.
		/// If <see langword="false" />, determine If it Is equal to the specified tag.</param>
		/// <returns>An <see cref="T:System.Linq.IGrouping`2" />, categorized by tag name.</returns>
		public IEnumerable<IGrouping<string, TEvent>> GetTaggedEvents(string name, bool strict)
		{
			if (string.IsNullOrEmpty(name))
				return [];
			return strict
					? ((IEnumerable<TEvent>)e).Where(i => i.Tag == name).GroupBy(i => i.Tag)
					: ((IEnumerable<TEvent>)e).Where(i => i.Tag?.Contains(name) ?? false).GroupBy(i => i.Tag);
		}
	}

	extension<TCollection, TEvent>(LevelElementCollection<TCollection, TEvent> e)
			where TCollection : OrderedEventCollection<TEvent, EventType, TickTime>, new()
			where TEvent : IBaseEvent
	{
		/// <summary>
		/// Adds a range of items to the specified <see cref="LevelElementCollection{TCollection, TEvent}"/>.
		/// </summary>
		/// <param name="items">The range of items to add to the collection.</param>
		public void AddRange(IEnumerable<TCollection> items)
		{
			foreach (TCollection item in items)
				e.Add(item);
		}
	}

	extension(Row e)
	{
		/// <summary>
		/// Get all classic beat events and their variants.
		/// </summary>
		private IEnumerable<BaseBeat> ClassicBeats() => e.OfEvent<BaseBeat>().Where(i => i.Type == EventType.AddClassicBeat | i.Type == EventType.AddFreeTimeBeat | i.Type == EventType.PulseFreeTimeBeat);
		/// <summary>
		/// Get all oneshot beat events.
		/// </summary>
		private IEnumerable<BaseBeat> OneshotBeats() => e.OfEvent<BaseBeat>().Where(i => i.Type == EventType.AddOneshotBeat);

		/// <summary>
		/// Gets the room index of the row at the specified beat.
		/// </summary>
		/// <param name="beat">The beat to query.</param>
		/// <returns>
		/// The room index at the specified beat, or the row's default room if no <see cref="ReorderRow"/> event is found.
		/// </returns>
		public RoomIndex RoomOf(TickTime beat) => e.InRange(new(), beat).OfEvent<ReorderRow>().LastOrDefault()?.NewRoom ?? e.Room.Room;
		/// <summary>
		/// Get the row beat status
		/// </summary>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public SortedDictionary<float, int[]> GetRowBeatStatus()
		{
			SortedDictionary<float, int[]> L = [];
			RowType rowType = e.RowType;
			switch (rowType)
			{
				case RowType.Classic:
					int[] value = new int[7];
					L.Add(0f, value);
					foreach (IBaseEvent beat in e)
						switch (beat.Type)
						{
							case EventType.AddClassicBeat:
								AddClassicBeat trueBeat = (AddClassicBeat)beat;
								int i = 0;
								do
								{
									int[] statusArray = L[beat.TickTime.Tick] ?? new int[7];
									int[] array = statusArray;
									int num = i;
									ref int ptr = ref array[num];
									array[num] = ptr + 1;
									L[beat.TickTime.Tick] = statusArray;
									i++;
								}
								while (i <= 6);
								break;
							default:
								throw new NotImplementedException();
						}
					break;
				case RowType.Oneshot:
					throw new NotImplementedException();
				default:
					throw new InvalidOperationException("How");
			}
			return L;
		}
		/// <summary>
		/// Get all beats of the row.
		/// </summary>
		public IEnumerable<BaseBeat> Beats()
		{
			RowType rowType = e.RowType;
			IEnumerable<BaseBeat> Beats;
			if (rowType != RowType.Classic)
			{
				if (rowType != RowType.Oneshot)
					throw new InvalidOperationException("How?");
				Beats = e.OneshotBeats();
			}
			else
				Beats = e.ClassicBeats();
			return Beats;
		}
	}

	extension(Level e)
	{
		/// <summary>
		/// Get an instance of the beat associated with the level.
		/// </summary>
		/// <param name="beatOnly">Total number of 1-based beats.</param>
		public TickTime BeatOf(float beatOnly) => e.Calculator.BeatOf(beatOnly);
		/// <summary>
		/// Get an instance of the beat associated with the level.
		/// </summary>
		/// <param name="bar">The 1-based bar.</param>
		/// <param name="beat">The 1-based beat of the bar.</param>
		public TickTime BeatOf(int bar, float beat) => e.Calculator.BeatOf(bar, beat);
		/// <summary>
		/// Get an instance of the beat associated with the level.
		/// </summary>
		/// <param name="timeSpan">Total time span of the beat.</param>
		public TickTime BeatOf(TimeSpan timeSpan) => e.Calculator.BeatOf(timeSpan);
	}

	extension(Decoration e)
	{
		/// <summary>
		/// Gets the depth of the decoration at the specified beat.
		/// </summary>
		/// <param name="beat">The beat to query.</param>
		/// <returns>
		/// The depth value at the specified beat, or the decoration's default depth if no <see cref="ReorderSprite"/> event is found.
		/// </returns>
		public int DepthOf(TickTime beat) => e.InRange(new(), beat).OfEvent<ReorderSprite>().LastOrDefault()?.Depth ?? e.Depth;

		/// <summary>
		/// Gets the room index of the decoration at the specified beat.
		/// </summary>
		/// <param name="beat">The beat to query.</param>
		/// <returns>
		/// The room index at the specified beat, or the decoration's default room if no <see cref="ReorderSprite"/> event is found.
		/// </returns>
		public RoomIndex RoomOf(TickTime beat) => e.InRange(new(), beat).OfEvent<ReorderSprite>().LastOrDefault()?.NewRoom ?? e.Room.Room;
	}

	extension<TEvent>(TEvent e) where TEvent : class, IBaseEvent
	{
		/// <summary>
		/// Returns all previous events of the same type, including events of the same beat but executed before itself.
		/// </summary>
		public IEnumerable<TEvent> Before()
		{
			ReadOnlyEnumCollection<EventType> types = EventTypeRegistry.ToEnums<TEvent>();
			IEnumerator<KeyValuePair<TickTime, TypedEventCollection<EventType, TickTime>>> enumerator = e.TickTime.BaseLevel?.EventsBeatOrder.GetEnumerator() ?? throw new InvalidRDBeatException();
			while (enumerator.MoveNext() && enumerator.Current.Key < e.TickTime)
			{
				if (enumerator.Current.Value.ContainsTypes(types))
					foreach (IBaseEvent item in enumerator.Current.Value)
					{
						if (item as TEvent is TEvent o)
							yield return o;
					}
			}
			if (enumerator.Current.Key != e.TickTime)
				yield break;
			if (enumerator.Current.Value.ContainsTypes(types))
				foreach (IBaseEvent item in enumerator.Current.Value)
				{
					if (item == e)
						yield break;
					if (item as TEvent is TEvent o)
						yield return o;
				}
		}

		/// <summary>
		/// Returns all events of the same type that follow, including events of the same beat but executed after itself.
		/// </summary>
		public IEnumerable<TEvent> After()
		{
			ReadOnlyEnumCollection<EventType> types = EventTypeRegistry.ToEnums<TEvent>();
			IEnumerator<KeyValuePair<TickTime, TypedEventCollection<EventType, TickTime>>> enumerator = e.TickTime.BaseLevel?.EventsBeatOrder.GetEnumerator() ?? throw new InvalidRDBeatException();
			bool moved;
			while ((moved = enumerator.MoveNext()) && enumerator.Current.Key < e.TickTime) { }
			if (!moved)
				yield break;
			bool flag = false;
			if (enumerator.Current.Key == e.TickTime && enumerator.Current.Value.ContainsTypes(types))
			{
				foreach (IBaseEvent item in enumerator.Current.Value)
				{
					if (flag && item as TEvent is TEvent o)
						yield return o;
					if (item == e)
						flag = true;
				}
			}
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Value.ContainsTypes(types))
					foreach (IBaseEvent item in enumerator.Current.Value)
					{
						if (item as TEvent is TEvent o)
							yield return o;
					}
			}
		}

		/// <summary>
		/// Returns the previous event of the same type, including events of the same beat but executed before itself.
		/// </summary>
		public TEvent Front() => e.FrontOrDefault() ?? throw new InvalidOperationException("");
		/// <summary>
		/// Returns the previous event of the same type, including events of the same beat but executed before itself. Returns <see langword="null" /> if it does not exist.
		/// </summary>
		public TEvent? FrontOrDefault()
		{
			ReadOnlyEnumCollection<EventType> types = EventTypeRegistry.ToEnums<TEvent>();
			IEnumerator<KeyValuePair<TickTime, TypedEventCollection<EventType, TickTime>>> enumerator = e.TickTime.BaseLevel?.EventsBeatOrder.GetEnumerator() ?? throw new InvalidRDBeatException();
			TEvent? front = null;
			while (enumerator.MoveNext() && enumerator.Current.Key < e.TickTime)
			{
				if (enumerator.Current.Value.ContainsTypes(types))
					foreach (IBaseEvent item in enumerator.Current.Value)
					{
						if (item as TEvent is TEvent o)
							front = o;
					}
			}
			if (enumerator.Current.Key != e.TickTime || !enumerator.Current.Value.ContainsTypes(types))
				return front;
			foreach (IBaseEvent item in enumerator.Current.Value)
			{
				if (item == e)
					return front;
				if (item as TEvent is TEvent o)
					front = o;
			}
			throw new InvalidOperationException();
		}

		/// <summary>
		/// Returns the next event of the same type, including events of the same beat but executed after itself.
		/// </summary>
		public TEvent Next() => e.NextOrDefault() ?? throw new InvalidOperationException("");
		/// <summary>
		/// Returns the next event of the same type, including events of the same beat but executed after itself. Returns <see langword="null" /> if it does not exist.
		/// </summary>
		public TEvent? NextOrDefault()
		{
			ReadOnlyEnumCollection<EventType> types = EventTypeRegistry.ToEnums<TEvent>();
			IEnumerator<KeyValuePair<TickTime, TypedEventCollection<EventType, TickTime>>> enumerator = e.TickTime.BaseLevel?.EventsBeatOrder.GetEnumerator() ?? throw new InvalidRDBeatException();
			bool moved;
			while ((moved = enumerator.MoveNext()) && enumerator.Current.Key < e.TickTime) { }
			if (!moved)
				return null;
			bool flag = false;
			if (enumerator.Current.Key == e.TickTime && enumerator.Current.Value.ContainsTypes(types))
			{
				foreach (IBaseEvent item in enumerator.Current.Value)
				{
					if (flag && item is TEvent o)
						return o;
					if (item == e)
						flag = true;
				}
			}
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.Value.ContainsTypes(types))
					continue;
				foreach (IBaseEvent item in enumerator.Current.Value)
				{
					if (item is TEvent o)
						return o;
				}
			}
			return null;
		}
	}

	extension(IBaseEvent e)
	{
		/// <summary>
		/// Returns all previous events of the specified type, including events of the same beat but executed before itself.
		/// </summary>
		public IEnumerable<TEvent> Before<TEvent>() where TEvent : class, IBaseEvent
		{
			ReadOnlyEnumCollection<EventType> types = EventTypeRegistry.ToEnums<TEvent>();
			IEnumerator<KeyValuePair<TickTime, TypedEventCollection<EventType, TickTime>>> enumerator = e.TickTime.BaseLevel?.EventsBeatOrder.GetEnumerator() ?? throw new InvalidRDBeatException();
			while (enumerator.MoveNext() && enumerator.Current.Key < e.TickTime)
			{
				if (enumerator.Current.Value.ContainsTypes(types))
					foreach (IBaseEvent item in enumerator.Current.Value)
					{
						if (item as TEvent is TEvent o)
							yield return o;
					}
			}
			if (enumerator.Current.Key != e.TickTime || !enumerator.Current.Value.ContainsTypes(types))
				yield break;
			foreach (IBaseEvent item in enumerator.Current.Value)
			{
				if (item == e)
					yield break;
				if (item as TEvent is TEvent o)
					yield return o;
			}
		}

		/// <summary>
		/// Returns all events of the specified type that follow, including events of the same beat but executed after itself.
		/// </summary>
		public IEnumerable<TEvent> After<TEvent>() where TEvent : class, IBaseEvent
		{
			ReadOnlyEnumCollection<EventType> types = EventTypeRegistry.ToEnums<TEvent>();
			IEnumerator<KeyValuePair<TickTime, TypedEventCollection<EventType, TickTime>>> enumerator = e.TickTime.BaseLevel?.EventsBeatOrder.GetEnumerator() ?? throw new InvalidRDBeatException();
			bool moved;
			while ((moved = enumerator.MoveNext()) && enumerator.Current.Key < e.TickTime) { }
			if (!moved)
				yield break;
			bool flag = false;
			if (enumerator.Current.Key == e.TickTime && enumerator.Current.Value.ContainsTypes(types))
			{
				foreach (IBaseEvent item in enumerator.Current.Value)
				{
					if (flag && item as TEvent is TEvent o)
						yield return o;
					if (item == e)
						flag = true;
				}
			}
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Value.ContainsTypes(types))
					foreach (IBaseEvent item in enumerator.Current.Value)
					{
						if (item as TEvent is TEvent o)
							yield return o;
					}
			}
		}

		/// <summary>
		/// Returns the previous event of the specified type, including events of the same beat but executed before itself.
		/// </summary>
		public TEvent Front<TEvent>() where TEvent : class, IBaseEvent => e.FrontOrDefault<TEvent>() ?? throw new InvalidOperationException("");

		/// <summary>
		/// Returns the previous event of the specified type, including events of the same beat but executed before itself. Returns <see langword="null" /> if it does not exist.
		/// </summary>
		public TEvent? FrontOrDefault<TEvent>() where TEvent : class, IBaseEvent
		{
			ReadOnlyEnumCollection<EventType> types = EventTypeRegistry.ToEnums<TEvent>();
			IEnumerator<KeyValuePair<TickTime, TypedEventCollection<EventType, TickTime>>> enumerator = e.TickTime.BaseLevel?.EventsBeatOrder.GetEnumerator() ?? throw new InvalidRDBeatException();
			TEvent? front = null;
			while (enumerator.MoveNext() && enumerator.Current.Key < e.TickTime)
			{
				if (enumerator.Current.Value.ContainsTypes(types))
					foreach (IBaseEvent item in enumerator.Current.Value)
					{
						if (item is TEvent o)
							front = o;
					}
			}
			if (enumerator.Current.Key != e.TickTime || !enumerator.Current.Value.ContainsTypes(types))
				return front;
			foreach (IBaseEvent item in enumerator.Current.Value)
			{
				if (item == e)
					return front;
				if (item is TEvent o)
					front = o;
			}
			throw new InvalidOperationException();
		}

		/// <summary>
		/// Returns the next event of the specified type, including events of the same beat but executed after itself.
		/// </summary>
		public TEvent Next<TEvent>() where TEvent : class, IBaseEvent => e.NextOrDefault<TEvent>() ?? throw new InvalidOperationException("");

		/// <summary>
		/// Returns the next event of the specified type, including events of the same beat but executed after itself. Returns <see langword="null" /> if it does not exist.
		/// </summary>
		public TEvent? NextOrDefault<TEvent>() where TEvent : class, IBaseEvent
		{
			ReadOnlyEnumCollection<EventType> types = EventTypeRegistry.ToEnums<TEvent>();
			IEnumerator<KeyValuePair<TickTime, TypedEventCollection<EventType, TickTime>>> enumerator = e.TickTime.BaseLevel?.EventsBeatOrder.GetEnumerator() ?? throw new InvalidRDBeatException();
			bool moved;
			while ((moved = enumerator.MoveNext()) && enumerator.Current.Key < e.TickTime) { }
			if (!moved)
				return null;
			bool flag = false;
			if (enumerator.Current.Key == e.TickTime && enumerator.Current.Value.ContainsTypes(types))
			{
				foreach (IBaseEvent item in enumerator.Current.Value)
				{
					if (flag && item is TEvent o)
						return o;
					if (item == e)
						flag = true;
				}
			}
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Value.ContainsTypes(types))
					foreach (IBaseEvent item in enumerator.Current.Value)
					{
						if (item is TEvent o)
							return o;
					}
			}
			return null;
		}
	}

	/// <summary>
	/// Compares this <see cref="BaseEvent"/> instance with another <see cref="BaseEvent"/> instance.
	/// </summary>
	/// <param name="e">The current <see cref="BaseEvent"/> instance.</param>
	/// <param name="obj">The <see cref="BaseEvent"/> instance to compare with.</param>
	/// <returns>
	/// <para>0 if both events are the same instance.</para>
	/// <para>-1 if <paramref name="e"/> should be ordered before <paramref name="obj"/>.</para>
	/// <para>1 if <paramref name="e"/> should be ordered after <paramref name="obj"/>.</para>
	/// </returns>
	/// <exception cref="InvalidOperationException">
	/// Thrown if either event has an empty beat, or if the event order cannot be determined.
	/// </exception>
	public static int CompareTo(this BaseEvent e, BaseEvent obj)
	{
		if (e.TickTime.IsEmpty || obj.TickTime.IsEmpty)
			throw new InvalidOperationException("Cannot compare events with empty beats.");
		if (ReferenceEquals(e, obj))
			return 0;
		if (e.TickTime == obj.TickTime)
		{
			TypedEventCollection<EventType, TickTime> list = (e._beat.BaseLevel?.EventsBeatOrder[e.TickTime]) ?? throw new InvalidOperationException("How?");
			return list.CompareTo(e, obj) ? -1 : 1;
		}
		return e.TickTime.CompareTo(obj.TickTime);
	}
	/// <summary>
	/// Converts the specified RDRoomIndex enumeration value to its corresponding byte representation.
	/// </summary>
	/// <param name="e">The RDRoomIndex enumeration value to convert.</param>
	/// <returns>A byte representing the value of the specified RDRoomIndex enumeration.</returns>
	public static byte ToIndex(this RoomIndex e) => new SingleRoom(e).Value;
#if NETSTANDARD
	extension<TStyle>(RichLine<TStyle>) where TStyle : IRichStringStyle<TStyle>, new()
	{
		/// <summary>
		/// Deserializes a string into an <see cref="RichLine{RDPhraseStyle}"/>.
		/// </summary>
		/// <param name="text">The string to deserialize.</param>
		/// <returns>A new <see cref="RichLine{RDPhraseStyle}"/> containing the deserialized content.</returns>
		/// <exception cref="ArgumentNullException">Thrown when the input text is null.</exception>
		/// <exception cref="FormatException">Thrown when the input text has an invalid format.</exception>
		public static RichLine<TStyle> Deserialize(string text)
		{
			return RichLine<TStyle>.Empty.Deserialize(text);
		}
	}
#endif
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
	public enum Wavetype
	{
		BoomAndRush,
		Spring,
		Spike,
		SpikeHuge,
		Ball,
		Single
	}
	public enum ShockWaveType
	{
		size,
		distortion,
		duration
	}
	public enum Particle
	{
		HitExplosion,
		leveleventexplosion
	}
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
