using System.Text;
using RhythmBase.Global.Components;
using RhythmBase.RhythmDoctor.Components.Lang;
using RhythmBase.RhythmDoctor.Events;
using RhythmBase.RhythmDoctor.Utils;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// The set of state modules a <see cref="Playhead"/> can simulate and expose.
/// </summary>
[Flags]
public enum PlayheadModules
{
	/// <summary>No module.</summary>
	None = 0,
	/// <summary>Conductor state: tick, bar/beat, BPM, crotchets per bar, speed and play style.</summary>
	Time = 1 << 0,
	/// <summary>Decoration state: visibility, transform and tint.</summary>
	Decorations = 1 << 1,
	/// <summary>Room state.</summary>
	Rooms = 1 << 2,
	/// <summary>Row state.</summary>
	Rows = 1 << 3,
	/// <summary>Per-room VFX preset overlays.</summary>
	Vfx = 1 << 4,
	/// <summary>Audio playback state.</summary>
	Audio = 1 << 5,
	/// <summary>Beat / gameplay state.</summary>
	Gameplay = 1 << 6,
	/// <summary>Text / UI state.</summary>
	Text = 1 << 7,
	/// <summary>Variables.</summary>
	Variables = 1 << 8,
	/// <summary>Game windows.</summary>
	Windows = 1 << 9,
	/// <summary>Hit records.</summary>
	Hits = 1 << 10,
	/// <summary>Every module.</summary>
	All = ~0,
}

/// <summary>
/// The reason a <see cref="Playhead.ForwardTo(TickTime)"/> call stopped.
/// </summary>
public enum PlayheadAdvanceStatus
{
	/// <summary>The requested time was reached.</summary>
	Reached,
	/// <summary>The play style holds or loops time, so the requested time can never be reached.</summary>
	NotReached,
	/// <summary>The simulation guard stopped the advance.</summary>
	GuardTriggered,
}

/// <summary>
/// The outcome of an advance operation.
/// </summary>
public readonly struct PlayheadAdvanceResult
{
	internal PlayheadAdvanceResult(PlayheadAdvanceStatus status, TickTime tickTime, int processedEvents)
	{
		Status = status;
		TickTime = tickTime;
		ProcessedEvents = processedEvents;
	}

	/// <summary>Gets the status.</summary>
	public PlayheadAdvanceStatus Status { get; }

	/// <summary>Gets the playhead time after the advance.</summary>
	public TickTime TickTime { get; }

	/// <summary>Gets how many events were processed by this advance.</summary>
	public int ProcessedEvents { get; }

	/// <summary>Gets whether the requested time was reached.</summary>
	public bool Reached => Status == PlayheadAdvanceStatus.Reached;
}

/// <summary>
/// Bounds a <see cref="Playhead.ForwardTo(TickTime)"/> loop so a holding or looping play style
/// cannot run forever.
/// </summary>
public sealed class LoopGuard
{
	/// <summary>Gets or sets the maximum number of scheduler steps.</summary>
	public int MaxSteps { get; set; } = 1_000_000;

	/// <summary>Gets or sets the maximum number of simulated ticks.</summary>
	public float MaxTicks { get; set; } = 10_000_000f;

	/// <summary>Gets or sets the maximum simulated wall-clock time.</summary>
	public TimeSpan MaxTime { get; set; } = TimeSpan.FromMinutes(30);

	/// <summary>Gets or sets whether repeated states are detected.</summary>
	public bool DetectCycles { get; set; } = true;
}

/// <summary>
/// Tunables for a <see cref="Playhead"/>.
/// </summary>
public sealed class PlayheadConfig
{
	/// <summary>
	/// Gets or sets the sandbox environment used to resolve variables and simulation functions.
	/// When <see langword="null"/> the playhead creates a minimal default environment.
	/// </summary>
	public IVariables? Variables { get; set; }

	/// <summary>Gets or sets the modules whose snapshots are produced.</summary>
	public PlayheadModules MonitoredModules { get; set; } = PlayheadModules.All;

	/// <summary>Gets or sets the guard applied to <see cref="Playhead.ForwardTo(TickTime)"/>.</summary>
	public LoopGuard LoopGuard { get; set; } = new();

	/// <summary>Gets or sets whether the playhead verifies the level was not modified.</summary>
	public bool EnforceReadOnly { get; set; } = true;

	/// <summary>
	/// Gets or sets whether the read-only check also hashes event content (slower). When
	/// <see langword="false"/> only the event count and level version are checked.
	/// </summary>
	public bool VerifyContentHash { get; set; }

	/// <summary>
	/// Gets or sets an optional extra predicate ANDed with the built-in condition evaluation.
	/// </summary>
	public Func<IBaseEvent, bool>? ConditionPredicate { get; set; }

	/// <summary>Gets or sets the simulated two-player mode (global tag <c>p</c> and player-mode conditions).</summary>
	public bool TwoPlayerMode { get; set; }

	/// <summary>Gets or sets the simulated flashy-effects accessibility (global tag <c>f</c>).</summary>
	public bool Flashy { get; set; }

	/// <summary>Gets or sets the simulated narration accessibility (global tag <c>n</c>).</summary>
	public bool NarrationEnabled { get; set; }

	/// <summary>Gets or sets the simulated language; when <see langword="null"/> language conditions fail.</summary>
	public Language? Language { get; set; }

	/// <summary>Gets or sets an optional accessibility provider overriding the flashy/narration defaults.</summary>
	public Func<EffectType, bool>? AccessibilityProvider { get; set; }

	/// <summary>Gets or sets an optional provider returning the last hit result of a row, for <c>LastHit</c> conditions.</summary>
	public Func<sbyte, HitResult?>? LastHitProvider { get; set; }

	/// <summary>Gets or sets the random source used by <c>TagAction.RunRandom</c>.</summary>
	public IRandomSimulator Random { get; set; } = new RandomSimulator();

	/// <summary>Gets or sets an optional factory that clones the sandbox for <see cref="Playhead.Clone"/>.</summary>
	public Func<IVariables, IVariables>? VariablesCloner { get; set; }

	/// <summary>
	/// Gets or sets the simulated hit timeline used for <c>LastHit</c> conditions. Provide a
	/// deterministic list to reproduce runs.
	/// </summary>
	public IReadOnlyList<HitRecord>? HitTimeline { get; set; }

	/// <summary>Gets or sets the simulated platform name (defaults to <c>Windows</c>).</summary>
	public string? Platform { get; set; }

	/// <summary>Gets or sets the simulated button press count.</summary>
	public int ButtonPressCount { get; set; }

	/// <summary>Gets or sets the simulated window-dance vertical-wrap flag.</summary>
	public bool WindowDanceCanVerticalWrap { get; set; }

	/// <summary>Gets or sets the simulated booth flag.</summary>
	public bool Booth { get; set; }

	/// <summary>Gets or sets the simulated paigeLeaves flag.</summary>
	public bool PaigeLeaves { get; set; }

	/// <summary>Gets or sets the names of levels considered passed, for <c>passedLevel(Name)</c>.</summary>
	public IReadOnlyCollection<string>? PassedLevels { get; set; }
}

/// <summary>
/// A state module simulated by the playhead.
/// </summary>
public interface IPlayheadObject
{
	/// <summary>Gets the owning playhead.</summary>
	Playhead Playhead { get; }

	/// <summary>Gets the stable object id.</summary>
	string Id { get; }

	/// <summary>Gets the object kind.</summary>
	PlayheadObjectKind Kind { get; }

	/// <summary>Gets which <see cref="PlayheadModules"/> flag this object belongs to.</summary>
	PlayheadModules Module { get; }

	/// <summary>Gets the event types this object consumes.</summary>
	ReadOnlyEnumCollection<EventType> RecordingTypes { get; }

	/// <summary>Applies an event.</summary>
	/// <param name="e">The event.</param>
	void MoveNext(IBaseEvent e);

	/// <summary>Creates an aggregate snapshot of the object state.</summary>
	/// <returns>The snapshot object.</returns>
	object Snapshot();

	/// <summary>
	/// Describes the individual object states this object owns. A module that represents a
	/// collection (rooms, rows, windows) yields one entry per instance.
	/// </summary>
	/// <returns>The object snapshots.</returns>
	IReadOnlyList<PlayheadObjectSnapshot> Describe();

	/// <summary>Clones the object for another playhead.</summary>
	/// <param name="playhead">The playhead that will own the clone.</param>
	/// <returns>The cloned object.</returns>
	IPlayheadObject Clone(Playhead playhead);

	/// <summary>Re-points the object at the playhead that owns it (used after cloning).</summary>
	/// <param name="playhead">The owning playhead.</param>
	void Rebind(Playhead playhead);
}

/// <summary>
/// An immutable, per-module snapshot of a playhead at a point in time.
/// </summary>
public readonly struct PlayheadSnapshot
{
	private readonly IReadOnlyDictionary<Type, object> _modules;

	internal PlayheadSnapshot(TickTime tickTime, IReadOnlyDictionary<Type, object> modules, IReadOnlyList<PlayheadObjectSnapshot> objects)
	{
		TickTime = tickTime;
		_modules = modules;
		Objects = objects;
	}

	/// <summary>Gets the time of the snapshot.</summary>
	public TickTime TickTime { get; }

	/// <summary>Gets the raw module snapshots keyed by snapshot type.</summary>
	public IReadOnlyDictionary<Type, object> Modules => _modules;

	/// <summary>Gets the object-oriented snapshots, one per simulated object.</summary>
	public IReadOnlyList<PlayheadObjectSnapshot> Objects { get; init; }

	/// <summary>Tries to get a module snapshot.</summary>
	/// <typeparam name="T">The snapshot type.</typeparam>
	/// <param name="value">The snapshot when present.</param>
	/// <returns><see langword="true"/> when the module was monitored.</returns>
	public bool TryGet<T>(out T value)
	{
		if (_modules.TryGetValue(typeof(T), out object? snapshot) && snapshot is T typed)
		{
			value = typed;
			return true;
		}
		value = default!;
		return false;
	}

	/// <summary>Gets a module snapshot.</summary>
	/// <typeparam name="T">The snapshot type.</typeparam>
	/// <returns>The snapshot.</returns>
	/// <exception cref="KeyNotFoundException">When the module was not monitored.</exception>
	public T Get<T>() => TryGet<T>(out T value) ? value : throw new KeyNotFoundException($"The module snapshot '{typeof(T).Name}' is not monitored.");
}

/// <summary>
/// A read-only simulation of a level. It plays events forward from a tick, keeps the state of
/// the monitored modules and exposes that state through <see cref="Snapshot(PlayheadModules)"/>.
/// </summary>
/// <remarks>
/// Creating and using a playhead assumes the level is not modified afterwards. State lives in the
/// playhead and its modules; the chart itself is never changed.
/// </remarks>
public sealed class Playhead
{
	private readonly BeatCalculator _calculator;
	private readonly EventScheduler _scheduler;
	private readonly List<IPlayheadObject> _modules;
	private readonly ConductorModule _conductor;
	private readonly ConditionEvaluator _condition;
	private readonly ExpressionContext _expressionContext;
	private readonly ExpressionEvaluator _expressionEvaluator;
	private readonly VariablesModule _variablesModule;
	private readonly HitModule? _hitModule;
	private readonly int[] _timesRun;
	private readonly List<(int Index, string Tag)> _tagged;
	private readonly List<(float Tick, int Index)> _pendingTagged;
	private readonly HashSet<int> _disabledTaggedEvents;
	private readonly IRandomSimulator _random;
	private int _tagDepth;
	private readonly int _creationEventCount;
	private readonly int _creationVersion;
	private readonly long _creationFingerprint;
	private float _currentTick;
	private int _lastEventCount;
	private long _lastFingerprint;

	/// <summary>
	/// Initializes a playhead over a level.
	/// </summary>
	/// <param name="level">The level to play. Must not be <see langword="null"/>.</param>
	/// <param name="config">Optional configuration.</param>
	public Playhead(Level level, PlayheadConfig? config = null)
		: this(level?.MainChart ?? throw new ArgumentNullException(nameof(level)), config)
	{
	}

	/// <summary>
	/// Initializes a playhead over a chart.
	/// </summary>
	/// <param name="chart">The chart to play. Must not be <see langword="null"/>.</param>
	/// <param name="config">Optional configuration.</param>
	public Playhead(Chart chart, PlayheadConfig? config = null)
	{
		CurrentChart = chart ?? throw new ArgumentNullException(nameof(chart));
		Config = config ?? new PlayheadConfig();
		_calculator = chart.Calculator;
		_currentTick = 1f;
		_scheduler = new EventScheduler(chart);
		VariablesBase baseEnvironment = Config.Variables as VariablesBase ?? EmptyVariables.Instance;
		_expressionContext = new ExpressionContext(new RuntimeVariables(baseEnvironment, Config));
		_expressionEvaluator = new ExpressionEvaluator(_expressionContext);
		IReadOnlyList<HitRecord>? hitTimeline = Config.HitTimeline ?? (Config.Variables as HitVariables)?.Hits;
		_hitModule = hitTimeline is null ? null : new HitModule(this, hitTimeline);
		if (Config.Variables is HitVariables hitVariables)
			hitVariables.TickProvider = () => _currentTick;
		_condition = new ConditionEvaluator(
			chart,
			Config,
			_expressionContext,
			row => _hitModule?.LastHit(row, _currentTick) ?? Config.LastHitProvider?.Invoke(row));
		_timesRun = new int[_scheduler.Count];
		_tagged = [];
		for (int i = 0; i < _scheduler.Count; i++)
		{
			IBaseEvent taggedEvent = _scheduler.EventAt(i);
			if (!string.IsNullOrEmpty(taggedEvent.Tag))
				_tagged.Add((i, taggedEvent.Tag));
		}
		_disabledTaggedEvents = [];
		_pendingTagged = [];
		_random = Config.Random;
		_creationEventCount = chart.Count;
		_creationVersion = chart.Settings.Version;
		_creationFingerprint = ComputeFingerprint(chart);
		_lastEventCount = _creationEventCount;
		_lastFingerprint = _creationFingerprint;
		_modules = [];
		_conductor = new ConductorModule(this);
		_modules.Add(_conductor);
		_variablesModule = new VariablesModule(this, _expressionContext);
		_modules.Add(_variablesModule);
		BuildModules(chart);
		if (_hitModule is not null)
			_modules.Add(_hitModule);
	}

	private Playhead(Chart chart, PlayheadConfig config, BeatCalculator calculator, EventScheduler scheduler, List<IPlayheadObject> modules, ConductorModule conductor, ExpressionContext expressionContext, int[] timesRun, List<(int Index, string Tag)> tagged, List<(float Tick, int Index)> pendingTagged, HashSet<int> disabledTaggedEvents, IRandomSimulator random, float tick, int creationEventCount, int creationVersion)
	{
		CurrentChart = chart;
		Config = config;
		_calculator = calculator;
		_scheduler = scheduler;
		_modules = modules;
		_conductor = conductor;
		_expressionContext = expressionContext;
		_expressionEvaluator = new ExpressionEvaluator(expressionContext);
		_variablesModule = null!;
		_hitModule = null;
		foreach (IPlayheadObject module in modules)
		{
			if (module is VariablesModule variables)
			{
				_variablesModule = variables;
				break;
			}
		}
		foreach (IPlayheadObject module in modules)
		{
			if (module is HitModule hit)
			{
				_hitModule = hit;
				break;
			}
		}
		if (expressionContext.Variables is RuntimeVariables runtime && runtime.Inner is HitVariables hitVariables)
			hitVariables.TickProvider = () => _currentTick;
		_condition = new ConditionEvaluator(
			chart,
			config,
			expressionContext,
			row => _hitModule?.LastHit(row, _currentTick) ?? config.LastHitProvider?.Invoke(row));
		_timesRun = timesRun;
		_tagged = tagged;
		_pendingTagged = pendingTagged;
		_disabledTaggedEvents = disabledTaggedEvents;
		_random = random;
		_currentTick = tick;
		_creationEventCount = creationEventCount;
		_creationVersion = creationVersion;
		_creationFingerprint = ComputeFingerprint(chart);
		_lastEventCount = creationEventCount;
		_lastFingerprint = _creationFingerprint;
	}

	/// <summary>Gets the chart being played.</summary>
	public Chart CurrentChart { get; }

	/// <summary>Gets the configuration.</summary>
	public PlayheadConfig Config { get; }

	/// <summary>Gets the shared expression context (the 30 fixed slots and the sandbox).</summary>
	internal ExpressionContext ExpressionContext => _expressionContext;

	/// <summary>Gets the current time.</summary>
	public TickTime CurrentTickTime => new(_calculator, _currentTick);

	/// <summary>Gets whether the scheduler reached the end of the chart.</summary>
	public bool IsFinished => _scheduler.IsFinished;

	/// <summary>Gets the conductor state.</summary>
	public PlayheadState State
	{
		get
		{
			(int bar, float beat) = _calculator.TickToBarBeat(_currentTick);
			return new PlayheadState
			{
				TickTime = CurrentTickTime,
				Bar = bar,
				Beat = beat,
				PlayStyle = _conductor.PlayStyle,
			};
		}
	}

	/// <summary>
	/// Advances the playhead by a number of ticks (<see cref="TickTime.Tick"/>).
	/// </summary>
	/// <param name="tick">The tick delta.</param>
	/// <returns>The advance result.</returns>
	public PlayheadAdvanceResult Forward(float tick) => AdvanceTo(_currentTick + tick);

	/// <summary>
	/// Advances the playhead by a duration.
	/// </summary>
	/// <param name="timeSpan">The time delta.</param>
	/// <returns>The advance result.</returns>
	public PlayheadAdvanceResult Forward(TimeSpan timeSpan)
	{
		float current = _calculator.TimeSpanToTick(CurrentTickTime.TimeSpan);
		float target = _calculator.TimeSpanToTick(CurrentTickTime.TimeSpan + timeSpan);
		return AdvanceTo(_currentTick + (target - current));
	}

	/// <summary>
	/// Advances the playhead to a target time.
	/// </summary>
	/// <param name="target">The target time.</param>
	/// <returns>The advance result. When a hold/loop play style prevents reaching
	/// <paramref name="target"/>, <see cref="PlayheadAdvanceResult.Status"/> is
	/// <see cref="PlayheadAdvanceStatus.NotReached"/>.</returns>
	public PlayheadAdvanceResult ForwardTo(TickTime target) => AdvanceTo(target.Tick);

	/// <summary>
	/// Advances the playhead to a target tick.
	/// </summary>
	/// <param name="tick">The target tick.</param>
	/// <returns>The advance result.</returns>
	public PlayheadAdvanceResult ForwardTo(float tick) => AdvanceTo(tick);

	/// <summary>
	/// Creates an immutable snapshot of the selected modules at the current time.
	/// </summary>
	/// <param name="modules">The modules to include. Defaults to the configured set.</param>
	/// <returns>The snapshot.</returns>
	public PlayheadSnapshot Snapshot(PlayheadModules? modules = null)
	{
		PlayheadModules selection = modules ?? Config.MonitoredModules;
		Dictionary<Type, object> snapshots = [];
		List<DecorationSnapshot> decorations = [];
		List<RoomSnapshot> rooms = [];
		List<RowSnapshot> rows = [];
		List<WindowSnapshot> windows = [];
		List<PlayheadObjectSnapshot> objects = [];
		foreach (IPlayheadObject module in _modules)
		{
			if ((selection & module.Module) == 0)
				continue;
			object snapshot = module.Snapshot();
			switch (snapshot)
			{
				case DecorationSnapshot decoration:
					decorations.Add(decoration);
					break;
				case RoomSnapshot room:
					rooms.Add(room);
					break;
				case RowSnapshot row:
					rows.Add(row);
					break;
				case WindowSnapshot window:
					windows.Add(window);
					break;
				default:
					snapshots[snapshot.GetType()] = snapshot;
					break;
			}
			objects.AddRange(module.Describe());
		}
		if (decorations.Count != 0)
			snapshots[typeof(DecorationsSnapshot)] = new DecorationsSnapshot { Items = decorations };
		if (rooms.Count != 0)
			snapshots[typeof(RoomsSnapshot)] = new RoomsSnapshot { Items = rooms };
		if (rows.Count != 0)
			snapshots[typeof(RowsSnapshot)] = new RowsSnapshot { Items = rows };
		if (windows.Count != 0)
			snapshots[typeof(WindowsSnapshot)] = new WindowsSnapshot { Items = windows };
		return new PlayheadSnapshot(CurrentTickTime, snapshots, objects);
	}

	/// <summary>
	/// Creates an independent copy of the playhead that continues from the current state.
	/// </summary>
	/// <returns>The clone.</returns>
	public Playhead Clone()
	{
		IVariables environment = Config.Variables is null
			? EmptyVariables.Instance
			: Config.VariablesCloner is not null ? Config.VariablesCloner(Config.Variables) : Config.Variables;
		VariablesBase baseEnvironment = environment as VariablesBase ?? EmptyVariables.Instance;
		ExpressionContext expressionContext = new(new RuntimeVariables(baseEnvironment, Config), _expressionContext);
		List<IPlayheadObject> modules = [];
		ConductorModule? conductor = null;
		foreach (IPlayheadObject module in _modules)
		{
			IPlayheadObject clone = module.Clone(this);
			if (clone is ConductorModule c)
				conductor = c;
			modules.Add(clone);
		}
		Playhead playhead = new(
			CurrentChart,
			Config,
			_calculator,
			_scheduler.Clone(),
			modules,
			conductor ?? new ConductorModule(_conductor),
			expressionContext,
			(int[])_timesRun.Clone(),
			_tagged,
			[.. _pendingTagged],
			new HashSet<int>(_disabledTaggedEvents),
			_random,
			_currentTick,
			_creationEventCount,
			_creationVersion);
		foreach (IPlayheadObject module in playhead._modules)
			module.Rebind(playhead);
		return playhead;
	}

	private void BuildModules(Chart chart)
	{
		PlayheadModules monitored = Config.MonitoredModules;
		if ((monitored & PlayheadModules.Decorations) != 0)
		{
			foreach (Decoration decoration in chart.Decorations)
				_modules.Add(new DecorationModule(this, decoration));
		}
		if ((monitored & PlayheadModules.Rooms) != 0)
			_modules.Add(new RoomModule(this));
		if ((monitored & PlayheadModules.Rows) != 0)
			_modules.Add(new RowModule(this));
		if ((monitored & PlayheadModules.Vfx) != 0)
			_modules.Add(new VfxModule(this));
		if ((monitored & PlayheadModules.Audio) != 0)
			_modules.Add(new AudioModule(this));
		if ((monitored & PlayheadModules.Gameplay) != 0)
			_modules.Add(new GameplayModule(this));
		if ((monitored & PlayheadModules.Text) != 0)
			_modules.Add(new TextModule(this));
		if ((monitored & PlayheadModules.Windows) != 0)
			_modules.Add(new WindowModule(this));
	}

	internal void ProcessEvent(IBaseEvent e, int index, bool viaTag = false)
	{
		if (!viaTag && Config.ConditionPredicate is not null && !Config.ConditionPredicate(e))
			return;
		if (!_condition.ShouldRun(e, _timesRun[index], viaTag))
			return;
		foreach (IPlayheadObject module in _modules)
		{
			if (module.RecordingTypes.Contains(e.Type))
				module.MoveNext(e);
		}
		_timesRun[index]++;
		if (e is CallCustomMethod call)
			ApplyCallCustomMethod(call);
		if (e is TagAction tagAction)
			HandleTagAction(tagAction);
	}

	/// <summary>
	/// Evaluates an event expression against the shared context. Numeric literals are returned
	/// directly; expression strings (optionally wrapped in <c>{ }</c>) are parsed and evaluated.
	/// </summary>
	internal float EvaluateExpression(Expression expression)
	{
		if (expression.IsNumeric)
			return expression.NumericValue;
		string text = expression.ExpressionValue;
		if (string.IsNullOrWhiteSpace(text))
			return 0f;
		text = text.Trim();
		if (text.Length >= 2 && text[0] == '{' && text[text.Length - 1] == '}')
			text = text.Substring(1, text.Length - 2);
		try
		{
			return _expressionEvaluator.EvaluateFloat(text);
		}
		catch (Exception)
		{
			return 0f;
		}
	}

	private void ApplyCallCustomMethod(CallCustomMethod call)
	{
		if (string.IsNullOrWhiteSpace(call.MethodName))
			return;
		string text = Interpolate(call.MethodName);
		string target = ClassifyTarget(ref text);
		if (target != "level")
		{
			// vfx./room targets are recorded as custom state so callers can observe them.
			_variablesModule.ApplyCustom(target, text);
			return;
		}
		// The expression language mirrors the game's CallCustomMethod: assignments, ++/--, and
		// function calls resolve against the shared context (fixed slots + sandbox).
		try
		{
			_expressionEvaluator.Evaluate(text);
		}
		catch (Exception)
		{
			// Unknown functions are ignored, matching the game's non-fatal behavior.
		}
	}

	/// <summary>
	/// Replaces every <c>{ expression }</c> in a custom-method string with its value.
	/// </summary>
	internal string Interpolate(string text)
	{
		if (text.IndexOf('{') < 0)
			return text;
		StringBuilder builder = new();
		int i = 0;
		while (i < text.Length)
		{
			char c = text[i];
			if (c == '{')
			{
				int end = text.IndexOf('}', i + 1);
				if (end < 0)
				{
					builder.Append(text, i, text.Length - i);
					break;
				}
				string inner = text.Substring(i + 1, end - i - 1);
				try
				{
					builder.Append(_expressionEvaluator.EvaluateString(inner));
				}
				catch (Exception)
				{
					builder.Append('0');
				}
				i = end + 1;
			}
			else
			{
				builder.Append(c);
				i++;
			}
		}
		return builder.ToString();
	}

	private static string ClassifyTarget(ref string text)
	{
		text = text.Trim();
		if (text.StartsWith("vfx.", StringComparison.Ordinal))
		{
			text = text.Substring(4);
			return "vfx";
		}
		if (text.StartsWith("level.", StringComparison.Ordinal))
		{
			text = text.Substring(6);
			return "level";
		}
		if (text.StartsWith("room", StringComparison.Ordinal))
		{
			int dot = text.IndexOf('.');
			if (dot > 4)
			{
				string selector = text.Substring(4, dot - 4);
				if (IsRoomSelector(selector))
				{
					text = text.Substring(dot + 1);
					return "room" + NormalizeSelector(selector);
				}
			}
		}
		return "level";
	}

	private static bool IsRoomSelector(string selector)
	{
		if (selector.Length == 1)
			return selector[0] >= '1' && selector[0] <= '9';
		return selector.Length == 3 && selector[0] == '[' && selector[1] >= '0' && selector[1] <= '9' && selector[2] == ']';
	}

	private static string NormalizeSelector(string selector) => selector.Length == 3 ? selector[1].ToString() : selector;

	/// <summary>
	/// Evaluates a piece of custom code against the shared context (used by the variables object).
	/// </summary>
	internal object? EvaluateCode(string code) => _expressionEvaluator.Evaluate(code);

	/// <summary>
	/// Creates a fresh playhead at the start of the level.
	/// </summary>
	/// <returns>The new playhead.</returns>
	public Playhead Reset() => new(CurrentChart, Config);

	/// <summary>
	/// Creates a fresh playhead advanced to a target time.
	/// </summary>
	/// <param name="target">The target time.</param>
	/// <returns>The new playhead.</returns>
	public Playhead SeekTo(TickTime target) => SeekTo(target.Tick);

	/// <summary>
	/// Creates a fresh playhead advanced to a target tick.
	/// </summary>
	/// <param name="tick">The target tick.</param>
	/// <returns>The new playhead.</returns>
	public Playhead SeekTo(float tick)
	{
		Playhead playhead = new(CurrentChart, Config);
		playhead.AdvanceTo(tick);
		return playhead;
	}

	private void HandleTagAction(TagAction tagAction)
	{
		if (_tagDepth >= 64)
			return;
		_tagDepth++;
		try
		{
			string name = tagAction.ActionTag;
			switch (tagAction.Action)
			{
				case ActionTagAction.Run:
					RunTagged(name, exact: true, random: false);
					break;
				case ActionTagAction.RunAll:
					RunTagged(name, exact: false, random: false);
					break;
				case ActionTagAction.RunRandom:
					RunTagged(name, exact: true, random: true);
					break;
				case ActionTagAction.Enable:
					SetTagEnabled(name, exact: true, enabled: true);
					break;
				case ActionTagAction.Disable:
					SetTagEnabled(name, exact: true, enabled: false);
					break;
				case ActionTagAction.EnableAll:
					SetTagEnabled(name, exact: false, enabled: true);
					break;
				case ActionTagAction.DisableAll:
					SetTagEnabled(name, exact: false, enabled: false);
					break;
			}
		}
		finally
		{
			_tagDepth--;
		}
	}

	private void RunTagged(string name, bool exact, bool random)
	{
		List<int> matches = [];
		foreach ((int index, string tag) in _tagged)
		{
			if (_disabledTaggedEvents.Contains(index))
				continue;
			if (exact ? tag == name : tag.Contains(name))
				matches.Add(index);
		}
		if (matches.Count == 0)
			return;
		if (random)
		{
			RunTaggedAt(matches[_random.Next(0, matches.Count)]);
			return;
		}
		foreach (int index in matches)
			RunTaggedAt(index);
	}

	private void RunTaggedAt(int index)
	{
		// A tagged event with a condition duration is deferred, like the game's TimedAction.
		float delay = _scheduler.EventAt(index).Condition.Duration;
		if (delay > 0f)
		{
			_pendingTagged.Add((_currentTick + delay, index));
			_pendingTagged.Sort(static (a, b) => a.Tick.CompareTo(b.Tick));
			return;
		}
		ProcessEvent(_scheduler.EventAt(index), index, viaTag: true);
	}

	private void DrainPendingTagged(float upTo)
	{
		while (_pendingTagged.Count > 0 && _pendingTagged[0].Tick <= upTo)
		{
			int index = _pendingTagged[0].Index;
			_pendingTagged.RemoveAt(0);
			ProcessEvent(_scheduler.EventAt(index), index, viaTag: true);
		}
	}

	private void SetTagEnabled(string name, bool exact, bool enabled)
	{
		foreach ((int index, string tag) in _tagged)
		{
			if (!(exact ? tag == name : tag.Contains(name)))
				continue;
			if (enabled)
				_disabledTaggedEvents.Remove(index);
			else
				_disabledTaggedEvents.Add(index);
		}
	}

	private PlayheadAdvanceResult AdvanceTo(float targetTick)
	{
		if (Config.EnforceReadOnly)
			VerifyUnchanged();

		if (targetTick <= _currentTick)
		{
			_currentTick = targetTick;
			_scheduler.SeekTo(targetTick);
			return new PlayheadAdvanceResult(PlayheadAdvanceStatus.Reached, CurrentTickTime, 0);
		}

		LoopGuard guard = Config.LoopGuard;
		float simulated = 0f;
		int steps = 0;
		int totalProcessed = 0;

		while (_currentTick < targetTick)
		{
			if (++steps > guard.MaxSteps || simulated > guard.MaxTicks)
				return new PlayheadAdvanceResult(PlayheadAdvanceStatus.GuardTriggered, CurrentTickTime, totalProcessed);

			(int bar, float beat) = _calculator.TickToBarBeat(_currentTick);
			float barEnd = _calculator.BarBeatToTick(bar + 1, 1);
			float nextEvent = _scheduler.NextTick;

			float boundary = targetTick;
			bool barBoundary = false;
			if (nextEvent < boundary)
				boundary = nextEvent;
			if (barEnd < boundary)
			{
				boundary = barEnd;
				barBoundary = true;
			}
			else if (barEnd == boundary)
			{
				barBoundary = true;
			}

			totalProcessed += barBoundary
				? _scheduler.ProcessBefore(barEnd, this)
				: _scheduler.ProcessUpTo(boundary, this);
			DrainPendingTagged(boundary);

			if (barBoundary)
			{
				int decision = _conductor.BeginBarBoundary(bar);
				if (decision == ConductorModule.BoundaryHold)
				{
					simulated += barEnd - _currentTick;
					_currentTick = barEnd;
					PlayheadAdvanceStatus holdStatus = targetTick > barEnd ? PlayheadAdvanceStatus.NotReached : PlayheadAdvanceStatus.Reached;
					return new PlayheadAdvanceResult(holdStatus, CurrentTickTime, totalProcessed);
				}
				if (decision == ConductorModule.BoundaryLoopBar)
				{
					float barStart = _calculator.BarBeatToTick(bar, 1);
					simulated += barEnd - barStart;
					_currentTick = barStart;
					_scheduler.SeekTo(barStart);
					continue;
				}
				if (decision == ConductorModule.BoundaryLoopBeat)
				{
					float beatStart = _calculator.BarBeatToTick(bar, (float)Math.Floor(beat));
					simulated += barEnd - beatStart;
					_currentTick = beatStart;
					_scheduler.SeekTo(beatStart);
					continue;
				}

				float targetBarStart = _calculator.BarBeatToTick(decision, 1);
				simulated += Math.Abs(targetBarStart - _currentTick);
				_currentTick = targetBarStart;
				_scheduler.SeekTo(targetBarStart);
				totalProcessed += _scheduler.ProcessUpTo(targetBarStart, this);
				continue;
			}

			simulated += boundary - _currentTick;
			_currentTick = boundary;
		}

		return new PlayheadAdvanceResult(PlayheadAdvanceStatus.Reached, CurrentTickTime, totalProcessed);
	}

	private void VerifyUnchanged()
	{
		int count = CurrentChart.Count;
		int version = CurrentChart.Settings.Version;
		bool changed = count != _lastEventCount || version != _creationVersion;
		if (!changed && Config.VerifyContentHash)
		{
			long fingerprint = ComputeFingerprint(CurrentChart);
			changed = fingerprint != _lastFingerprint;
			_lastFingerprint = fingerprint;
		}
		if (changed)
			throw new InvalidOperationException("The chart was modified after the playhead was created; playheads are read-only.");
		_lastEventCount = count;
	}

	private static long ComputeFingerprint(Chart chart)
	{
		unchecked
		{
			long hash = 1469598103934665603L;
			foreach (IBaseEvent e in chart)
			{
				Mix(ref hash, e.Type.ToString());
				Mix(ref hash, e.TickTime.Tick.ToString("R"));
				Mix(ref hash, e.Active ? "1" : "0");
				Mix(ref hash, e.Tag ?? "");
				Mix(ref hash, e.RunTag ? "1" : "0");
				Mix(ref hash, e.Condition.Serialize());
			}
			Mix(ref hash, chart.Decorations.Count.ToString());
			Mix(ref hash, chart.Rows.Count.ToString());
			return hash;
		}
	}

	private static void Mix(ref long hash, string text)
	{
		foreach (char c in text)
		{
			hash ^= c;
			hash *= 1099511628211L;
		}
		hash ^= '|';
		hash *= 1099511628211L;
	}
}

