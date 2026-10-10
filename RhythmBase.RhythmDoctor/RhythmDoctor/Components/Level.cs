using RhythmBase.Global.Components;
using RhythmBase.RhythmDoctor.Events;
using RhythmBase.RhythmDoctor.Extensions;
using RhythmBase.RhythmDoctor.Utils;
using System.Collections;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Represents a Rhythm Doctor level: a container that holds the main chart plus any charts
/// referenced via <see cref="GoToLevel"/>, keyed by file name. Referenced charts are resolved
/// only once, so cyclic references between charts share the same <see cref="Chart"/> instance.
/// </summary>
public partial class Level :
	ILevel<Level, Chart>,
	IArchiveLevel<Level, Chart>,
	IEventEnumerable<IBaseEvent>,
	ILevel
{
	private readonly ChartDictionary<Chart> _charts;
	private readonly HashSet<Chart> _owned = [];
	/// <summary>
	/// Message used for transitional wrapper members that forward to <see cref="MainChart"/>.
	/// These members exist for backward compatibility and will be removed or changed in a future
	/// API update; access the chart directly via <see cref="MainChart"/> instead.
	/// </summary>
	internal const string ObsoleteWrapperMessage =
		"This member is a transitional wrapper around MainChart and will be removed or changed in a future API update. Access the chart directly via MainChart instead.";

	/// <summary>
	/// Initializes a new instance of the <see cref="Level"/> class with a new empty main chart.
	/// </summary>
	public Level() : this([])
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Level"/> class wrapping the specified main chart.
	/// </summary>
	/// <param name="mainChart">The main chart of the level. Cannot be <c>null</c>.</param>
	/// <param name="charts">Additional charts keyed by file name.</param>
	public Level(Chart mainChart, IReadOnlyDictionary<string, Chart>? charts = null)
	{
		MainChart = mainChart ?? throw new ArgumentNullException(nameof(mainChart));
		_charts = [];
		RegisterChart(DefaultChartName, mainChart);
		if (charts is not null)
			foreach (var pair in charts)
				RegisterChart(pair.Key, pair.Value);
	}

	/// <summary>
	/// The name of the main chart entry.
	/// </summary>
	public static string DefaultChartName => "main";

	/// <summary>
	/// Gets the main chart of the level.
	/// </summary>
	public Chart MainChart { get; }

	/// <summary>
	/// Gets the charts contained in this level, keyed by chart name. The main chart is registered
	/// under <see cref="DefaultChartName"/>. Keys and <see cref="IChart.Name"/> stay in sync.
	/// </summary>
	public ChartDictionary<Chart> Charts => _charts;

	/// <inheritdoc/>
	public string Filepath { get; internal set; } = string.Empty;

	/// <inheritdoc/>
	public string ResolvedPath { get; internal set; } = string.Empty;

	/// <summary>
	/// Gets the directory containing the resolved file.
	/// </summary>
	public string ResolvedDirectory =>
		!string.IsNullOrEmpty(ResolvedPath) ? Path.GetDirectoryName(ResolvedPath) ?? "" : "";

	/// <summary>
	/// Gets the default level.
	/// </summary>
	public static Level Default => new(Chart.Default);

	/// <inheritdoc/>
	public override string ToString() => MainChart.ToString();

	#region helpers forwarding to MainChart
	/// <summary>
	/// Gets or sets the main chart settings.
	/// </summary>
	[Obsolete(ObsoleteWrapperMessage)]
	public Settings Settings { get => MainChart.Settings; set => MainChart.Settings = value; }
	/// <summary>
	/// Gets the beat calculator of the main chart.
	/// </summary>
	[Obsolete(ObsoleteWrapperMessage)]
	public BeatCalculator Calculator => MainChart.Calculator;
	/// <summary>
	/// Gets the main chart tile collection.
	/// </summary>
	[Obsolete(ObsoleteWrapperMessage)]
	public RowCollection Rows => MainChart.Rows;
	/// <summary>
	/// Gets the main chart decoration collection.
	/// </summary>
	[Obsolete(ObsoleteWrapperMessage)]
	public DecorationCollection Decorations => MainChart.Decorations;
	/// <summary>
	/// Gets the main chart condition collection.
	/// </summary>
	[Obsolete(ObsoleteWrapperMessage)]
	public ConditionalList Conditionals => MainChart.Conditionals;
	/// <summary>
	/// Gets the main chart bookmark collection.
	/// </summary>
	[Obsolete(ObsoleteWrapperMessage)]
	public OrderedCollection<TickTime, Bookmark> Bookmarks => MainChart.Bookmarks;
	/// <summary>
	/// Gets the main chart color palette.
	/// </summary>
	[Obsolete(ObsoleteWrapperMessage)]
	public Color[] ColorPalette => MainChart.ColorPalette;
	/// <summary>
	/// Gets the number of events in the main chart.
	/// </summary>
	[Obsolete(ObsoleteWrapperMessage)]
	public int Count => MainChart.Count;
	/// <summary>
	/// Gets the default tick of the main chart.
	/// </summary>
	[Obsolete(ObsoleteWrapperMessage)]
	public TickTime DefaultTick => MainChart.DefaultTick;
	/// <summary>
	/// Gets a tick time in the main chart at the specified tick.
	/// </summary>
	[Obsolete(ObsoleteWrapperMessage)]
	public TickTime TickOf(float tick) => MainChart.TickOf(tick);
	/// <summary>
	/// Gets a tick time in the main chart at the specified bar and beat.
	/// </summary>
	[Obsolete(ObsoleteWrapperMessage)]
	public TickTime TickOf(int bar, float beat) => MainChart.TickOf(bar, beat);
	/// <summary>
	/// Gets a tick time in the main chart at the specified time span.
	/// </summary>
	[Obsolete(ObsoleteWrapperMessage)]
	public TickTime TickOf(TimeSpan timeSpan) => MainChart.TickOf(timeSpan);
	/// <summary>
	/// Occurs when a new event is added to the main chart.
	/// </summary>
	[Obsolete(ObsoleteWrapperMessage)]
	public event RDEventHandler? OnEventAdded { add => MainChart.EventAdded += value; remove => MainChart.EventAdded -= value; }
	/// <summary>
	/// Occurs when an event is removed from the main chart.
	/// </summary>
	[Obsolete(ObsoleteWrapperMessage)]
	public event RDEventHandler? OnEventRemoved { add => MainChart.EventRemoved += value; remove => MainChart.EventRemoved -= value; }
	/// <summary>
	/// Adds an event to the main chart.
	/// </summary>
	[Obsolete(ObsoleteWrapperMessage)]
	public void Add(IBaseEvent item) => MainChart.Add(item);
	/// <summary>
	/// Adds an event to the main chart with the specified beat change strategy.
	/// </summary>
	[Obsolete(ObsoleteWrapperMessage)]
	public bool Add(IBaseEvent item, BeatChangeStrategy strategy = BeatChangeStrategy.Default) => MainChart.Add(item, strategy);
	/// <summary>
	/// Removes an event from the main chart.
	/// </summary>
	[Obsolete(ObsoleteWrapperMessage)]
	public bool Remove(IBaseEvent item) => MainChart.Remove(item);
	/// <summary>
	/// Removes an event from the main chart with the specified beat change strategy.
	/// </summary>
	[Obsolete(ObsoleteWrapperMessage)]
	public bool Remove(IBaseEvent item, BeatChangeStrategy strategy = BeatChangeStrategy.Default) => MainChart.Remove(item, strategy);
	/// <summary>
	/// Determines whether the main chart contains the specified event.
	/// </summary>
	[Obsolete(ObsoleteWrapperMessage)]
	public bool Contains(IBaseEvent item) => MainChart.Contains(item);
	/// <summary>
	/// Gets the enumerator of the main chart.
	/// </summary>
	public IEnumerator<IBaseEvent> GetEnumerator() => MainChart.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	/// <inheritdoc/>
	public RhythmBase.Global.Components.RedBlackTree<TickTime, TypedEventCollection> EventsBeatOrder => MainChart.EventsBeatOrder;
	/// <inheritdoc/>
	TickTimeRange IEventEnumerable<IBaseEvent>.Range => TickTimeRange.Infinity;
	/// <inheritdoc/>
	public RhythmBase.Global.Components.ReadOnlyEnumCollection<EventType> Types => MainChart.Types;
	#endregion

	/// <inheritdoc/>
	public void Dispose()
	{
		foreach (Chart chart in _owned)
			chart.Dispose();
		GC.SuppressFinalize(this);
	}

	internal void RegisterChart(string name, Chart chart)
	{
		_charts.Add(name, chart);
		_owned.Add(chart);
		chart._parentLevel = this;
	}

	internal bool TryGetChart(string name, out Chart? chart) => _charts.TryGetChart(name, out chart);
}
