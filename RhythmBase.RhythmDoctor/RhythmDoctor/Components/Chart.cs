using RhythmBase.RhythmDoctor.Serialization;
using RhythmBase.RhythmDoctor.Events;
using RhythmBase.RhythmDoctor.Utils;
using RhythmBase.RhythmDoctor.Extensions;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Represents a single Rhythm Doctor chart (one <c>.rdlevel</c> playable file).
/// Charts are pure data objects; all file I/O and serialization is managed by <see cref="Level"/>.
/// </summary>
public partial class Chart :
	OrderedEventCollection<IBaseEvent>,
	IEventEnumerable<IBaseEvent>,
	IChart<Chart, TickTime>,
	IJsonChart<Chart, TickTime>,
	ILevel
{
	internal bool isZip = false;
	internal bool isExtracted = false;
	private Color[] colorPalette = new Color[PaletteColorCount];

	/// <summary>
	/// Gets or sets the name of the chart (e.g. the file name within a level).
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Occurs when a new event is added to the level.
	/// </summary>
	/// <remarks>Subscribe to this event to receive notifications whenever an event is added. The event handler
	/// receives the current level and event arguments containing additional information about the event.</remarks>
	public event RDEventHandler? EventAdded;

	/// <summary>
	/// Occurs when an event is removed from the level, providing the associated level and event arguments to subscribers.
	/// </summary>
	/// <remarks>Subscribe to this event to perform custom actions when an event is removed from the level. The
	/// event handler receives the level instance and event arguments that provide context about the removal.</remarks>
	public event RDEventHandler? EventRemoved;

	internal List<FloatingText> _floatingTexts = [];

	/// <summary>
	/// Variables.
	/// </summary>
	public readonly Variables Variables;

	/// <inheritdoc/>
	public override int Count => base.Count;

	/// <summary>
	/// The calculator that comes with the level.
	/// </summary>
	public BeatCalculator Calculator { get; }

	/// <summary>
	/// Chart Settings.
	/// </summary>
	public Settings Settings { get; set; }

	/// <summary>
	/// Chart tile collection.
	/// </summary>
	public RowCollection Rows { get; }

	/// <summary>
	/// Chart decoration collection.
	/// </summary>
	public DecorationCollection Decorations { get; }

	/// <summary>
	/// Chart condition collection.
	/// </summary>
	public ConditionalList Conditionals { get; }

	/// <summary>
	/// Chart bookmark collection.
	/// </summary>
	public OrderedCollection<TickTime, Bookmark> Bookmarks { get; }

	/// <summary>
	/// Chart colorPalette collection.
	/// </summary>
	public Color[] ColorPalette
	{
		get => colorPalette;
		internal set => colorPalette = value.Length == PaletteColorCount ? value : throw new InvalidOperationException();
	}

	/// <summary>
	/// Gets or sets the resolved file path of this chart.
	/// </summary>
	public string ResolvedPath { get; internal set; } = string.Empty;

	/// <summary>
	/// Gets or sets the original file path of this chart.
	/// </summary>
	public string Filepath { get; internal set; } = string.Empty;

	/// <summary>
	/// Gets the directory containing the resolved chart file.
	/// </summary>
	public string ResolvedDirectory =>
		!string.IsNullOrEmpty(ResolvedPath) ? Path.GetDirectoryName(ResolvedPath) ?? "" : "";

	/// <summary>
	/// Default beats with levels.
	/// The beat is 1.
	/// </summary>
	public TickTime DefaultTick => Calculator.TickOf(1f);

	/// <summary>
	/// Initializes a new instance of the <see cref="Chart"/> class.
	/// </summary>
	public Chart()
	{
		Variables = new Variables();
		Calculator = new BeatCalculator(this);
		Settings = new Settings();
		Conditionals = new(this);
		Bookmarks = new(i => i.Tick);
		ColorPalette = new Color[21];
		Rows = new(this);
		Decorations = new(this);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Chart"/> class with the specified items.
	/// </summary>
	/// <param name="items">The items to add to the level.</param>
	public Chart(IEnumerable<IBaseEvent> items) : this()
	{
		foreach (IBaseEvent item in items)
			Add(item);
	}

	/// <summary>
	/// Gets the default chart.
	/// </summary>
	public static Chart Default
	{
		get
		{
			Chart rdlevel = [];
			rdlevel.ColorPalette =
			[
				Color.Black,
				Color.White,
				new(0xFF7F7F7Fu),
				new(0xFFC3C3C3u),
				new(0xFF880015u),
				new(0xFFB97A57u),
				Color.Red,
				new(0xFFFFAEC9u),
				new(0xFFFF7F27u),
				new(0xFFFFC90Eu),
				new(0xFFFFF200u),
				new(0xFFEFE4B0u),
				new(0xFF22B14Cu),
				new(0xFFB5E61Du),
				new(0xFF00A2E8u),
				new(0xFF99D9EAu),
				new(0xFF3F48CCu),
				new(0xFF7092BEu),
				new(0xFFA349A4u),
				new(0xFFC8BFE7u),
				new(0x00000000u)
			];
			rdlevel.Settings.RankMaxMistakes =
			[
				20,
				15,
				10,
				5
			];
			rdlevel.Settings.RankDescription =
			[
				"Better call 911, now!",
				"Ugh, you can do better",
				"Not bad I guess...",
				"We make a good team!",
				"You are really good!",
				"Wow! That's awesome!!"
			];
			PlaySong playsong = new();
			SetTheme settheme = new();
			playsong.Song = new Audio() { Filename = "sndOrientalTechno" };
			settheme.Preset = Theme.OrientalTechno;
			rdlevel.Add(playsong);
			rdlevel.Add(settheme);
			Row samurai = new() { Room = RoomIndex.Room1, Character = GameCharacter.Samurai };
			rdlevel.Rows.Add(samurai);
			samurai.Sound.Filename = "Shaker";
			samurai.Add(new AddClassicBeat());
			return rdlevel;
		}
	}
	/// <summary>
	/// Occurs when an event is added to the chart.
	/// </summary>
	/// <param name="e"></param>
	public void OnEventAdded(RDEventArgs e) => EventAdded?.Invoke(this, e);
	/// <summary>
	/// Occurs when an event is removed from the chart.
	/// </summary>
	/// <param name="e"></param>
	public void OnEventRemoved(RDEventArgs e) => EventRemoved?.Invoke(this, e);
	/// <summary>
	/// Adds an event to the level.
	/// </summary>
	/// <param name="item">The event to be added.</param>
	public override bool Add(IBaseEvent item) => Add(item, Global.Config.Strategy);

	/// <summary> 
	/// Adds an event to the level, with an option to keep the event's position.
	/// </summary>
	/// <param name="item">The event to be added.</param>
	/// <param name="strategy">The strategy to use when adding the event, which may affect how beat changes are handled (default is <see cref="BeatChangeStrategy.Default"/>).</param>
	public bool Add(IBaseEvent item, BeatChangeStrategy strategy = Extensions.Extensions.DefaultStrategy)
	{
		bool success = true;
		// Set the default beat calculator
		if (item is not BaseEvent be)
			return false;
		var originalCalculator = be._tick._calculator;
		be._tick._calculator = Calculator;
		// Some events can only be at the beginning of a bar
		(_, float beat) = be._tick;
		if (item is IBarBeginningEvent e && beat != 1f)
			throw new ArgumentException(
				$"The event of type {item.GetType().Name} can only be placed at the beginning of a bar (beat 1), but its beat is {beat}.");
		// Update the beat's associated level
		be._tick.ResetCache();
		if (item is Comment comment && string.IsNullOrEmpty(comment.Target))
			// Comment events may or may not be in the decoration section
			success &= base.Add(item);
		else if (item is TintRows tintRows && tintRows.Row == -1)
			success &= base.Add(item);
		else if (item is BaseRowAction rowAction)
			// Add to the corresponding row
			success &= AddInternal(rowAction);
		else if (item is BaseDecorationAction decoAction)
			// Add to the corresponding decoration
			success &= AddInternal(decoAction);
		// BPM and CPB
		else if (item is SetCrotchetsPerBar setCrochetsPerBar && setCrochetsPerBar.Active)
			success &= AddSetCrotchetsPerBarInternal(setCrochetsPerBar, strategy);
		else if (item is BaseBeatsPerMinute baseBeatsPerMinute && baseBeatsPerMinute.Active)
			success &= AddBaseBeatsPerMinuteInternal(baseBeatsPerMinute);
		// Other events
		else
			success &= base.Add(item);
		if (item is FloatingText floatingText)
			_floatingTexts.Add(floatingText);
		if (success)
			EventAdded?.Invoke(this, new RDEventArgs(item));
		if (!success)
			be._tick._calculator = originalCalculator;
		return success;
	}

	/// <summary>
	/// Determines whether the level contains the specified event.
	/// </summary>
	/// <param name="item">The event to check for.</param>
	/// <returns>True if the event is contained in the level; otherwise, false.</returns>
	public override bool Contains(IBaseEvent item) =>
		EventTypeRegistry.RowTypes.Contains(item.Type) && Rows.Any((i) => i.Contains(item)) ||
		EventTypeRegistry.DecorationTypes.Contains(item.Type) && Decorations.Any((i) => i.Contains(item)) ||
		base.Contains(item);

	/// <summary>
	/// Removes an event from the level.
	/// </summary>
	/// <param name="item">The event to be removed.</param>
	/// <returns>True if the event was successfully removed; otherwise, false.</returns>
	public override bool Remove(IBaseEvent item) => Remove(item, BeatChangeStrategy.Default);

	/// <summary>
	/// Removes an event from the level, with an option to keep the event's position.
	/// </summary>
	/// <param name="item">The event to be removed.</param>
	/// <param name="strategy">The strategy to use when removing the event, which may affect how beat changes are handled (default is <see cref="BeatChangeStrategy.Default"/>).</param>
	/// <returns>True if the event was successfully removed; otherwise, false.</returns>
	public bool Remove(IBaseEvent item, BeatChangeStrategy strategy = BeatChangeStrategy.Default)
	{
		bool success;
		BaseEvent bs = item as BaseEvent ?? throw new InvalidOperationException("Inner exception that shouldn't happen");
		if (item is BaseDecorationAction decoAction)
		{
			RemoveInternal(decoAction);
			bs._tick = bs._tick.WithoutLink();
			success = true;
		}
		else if (item is BaseRowAction rowAction)
		{
			RemoveInternal(rowAction);
			bs._tick = bs._tick.WithoutLink();
			success = true;
		}
		else if (Contains(item))
		{
			if (item is SetCrotchetsPerBar cpb)
				success = RemoveSetCrotchetsPerBarInternal(cpb, strategy);
			else if (item is BaseBeatsPerMinute bpm)
				success = RemoveBaseBeatsPerMinuteInternal(bpm);
			else
			{
				bool result = base.Remove(item);
				bs._tick = bs._tick.WithoutLink();
				success = result;
			}
		}
		else
			success = false;

		if (item is FloatingText floatingText)
			_floatingTexts.Remove(floatingText);
		if (success)
			EventRemoved?.Invoke(this, new RDEventArgs(item));
		return success;
	}

	internal bool AddInternal(BaseDecorationAction item)
	{
		item._tick._calculator = Calculator;
		if (base.Contains(item)) return false;
		Decoration? parent = item.Target is null ? null : Decorations[item.Target];
		if (parent == null) Decorations._unhandledRowEvents.Add(item);
		else if (!parent.Contains(item))
			parent.AddDirectly(item);

		base.Add(item);
		return true;
	}

	internal bool AddInternal(BaseRowAction item)
	{
		item._tick._calculator = Calculator;
		if (base.Contains(item)) return false;
		Row? parent = item.Parent ?? (item.Row >= 0 && item.Row < Rows.Count ? Rows[item.Row] : null);
		if (parent == null) Rows._unhandledRowEvents.Add(item);
		else if (!parent.Contains(item))
			parent.AddDirectly(item);

		base.Add(item);
		return true;
	}

	internal bool AddDirectlyInternal(IBaseEvent item)
	{
		BaseEvent e = item as BaseEvent ?? throw new InvalidOperationException("Inner exception that shouldn't happen");
		e._tick._calculator = Calculator;
		if (base.Contains(e)) return false;
		base.Add(e);
		return true;
	}

	internal bool RemoveInternal(BaseDecorationAction item)
	{
		Decoration? parent = item.Target is null ? null : Decorations[item.Target];
		if (parent == null) Decorations._unhandledRowEvents.Remove(item);
		else
			parent.Remove(item);

		return base.Remove(item);
	}

	internal bool RemoveInternal(BaseRowAction item)
	{
		Row? parent = item.Parent ?? ((item.Row >= 0 && item.Row < Rows.Count) ? Rows[item.Row] : null);
		if (parent == null) Rows._unhandledRowEvents.Remove(item);
		else
			parent.Remove(item);

		return base.Remove(item);
	}

	internal bool RemoveDirectlyInternal(IBaseEvent item)
	{
		BaseEvent e = item as BaseEvent ?? throw new InvalidOperationException("Inner exception that shouldn't happen");
		if (!base.Contains(e)) return false;
		base.Remove(e);
		e._tick = e._tick.WithoutLink();
		return true;
	}

	private bool AddSetCrotchetsPerBarInternal(SetCrotchetsPerBar item, BeatChangeStrategy strategy)
	{
		if (Contains(item))
			return false;
		(int bar, _) = item._tick;
		CpbCache cache = new(item.TickTime.Tick, bar, item.CrotchetsPerBar);
		bool extra = Calculator.AddCpbAt(cache, (byte)strategy, out CpbCache fix);
		base.Add(item);
		if (extra)
		{
			SetCrotchetsPerBar cpb = new() { _tick = new TickTime(Calculator, fix.Tick), _crotchetsPerBar = fix.Cpb - 1 };
			base.Add(cpb);
			EventAdded?.Invoke(this, new(cpb) { IsAutoPopulated = true, });
		}

		return true;
	}

	private bool RemoveSetCrotchetsPerBarInternal(SetCrotchetsPerBar item, BeatChangeStrategy strategy)
	{
		var node = EventsBeatOrder.FindNode(item._tick);
		if (node is null) return false;
		var col = node.Value;
		if (!col.ContainsType(EventType.SetCrotchetsPerBar)) return false;
		var lastcpb = col.OfType<SetCrotchetsPerBar>().Last();
		if (lastcpb != item) return false;
		(int bar, _) = item._tick;
		CpbCache cache = new(item.TickTime.Tick, bar, item.CrotchetsPerBar);
		bool extra = Calculator.RemoveCpbAt(cache, (byte)strategy, out CpbCache fix);
		base.Remove(item);
		if (extra)
		{
			SetCrotchetsPerBar cpb = new() { _tick = new TickTime(Calculator, fix.Tick), _crotchetsPerBar = fix.Cpb - 1 };
			base.Add(cpb);
			EventRemoved?.Invoke(this, new(cpb) { IsAutoPopulated = true, });
		}

		return true;
	}

	private bool AddBaseBeatsPerMinuteInternal(BaseBeatsPerMinute item)
	{
		Calculator.AddBpmAt(new BpmCache(item.TickTime.Tick, item.TickTime.TimeSpan, item.BeatsPerMinute));
		bool result = base.Add(item);
		return result;
	}

	private bool RemoveBaseBeatsPerMinuteInternal(BaseBeatsPerMinute item)
	{
		Calculator.RemoveBpmAt(new BpmCache(item.TickTime.Tick, item.TickTime.TimeSpan, item.BeatsPerMinute));
		bool result = base.Remove(item);
		item._tick = item._tick.WithoutLink();
		return result;
	}

	/// <summary>
	/// Releases resources held by this chart. If the chart was extracted from a ZIP archive, the
	/// temporary extraction directory is deleted.
	/// </summary>
	public void Dispose()
	{
	}

	/// <inheritdoc/>
	public override string ToString() => $"\"{Settings.Song}\" Count = {Count}";
}