using RhythmBase.Global.Components;
using RhythmBase.Global.Components.Vector;
using RhythmBase.RhythmDoctor.Events;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// The state of one game window.
/// </summary>
public readonly struct WindowSnapshot
{
	/// <summary>Gets the window index.</summary>
	public int Index { get; init; }
	/// <summary>Gets the window name.</summary>
	public string Name { get; init; }
	/// <summary>Gets whether the window is visible.</summary>
	public bool Visible { get; init; }
	/// <summary>Gets whether the window is transparent.</summary>
	public bool Transparent { get; init; }
	/// <summary>Gets whether the window is frameless.</summary>
	public bool Frameless { get; init; }
	/// <summary>Gets the horizontal scale (percent).</summary>
	public float ScaleX { get; init; }
	/// <summary>Gets the vertical scale (percent).</summary>
	public float ScaleY { get; init; }
	/// <summary>Gets the pivot X (percent).</summary>
	public float OriginX { get; init; }
	/// <summary>Gets the pivot Y (percent).</summary>
	public float OriginY { get; init; }
	/// <summary>Gets the content mode, when set.</summary>
	public WindowContentMode? ContentMode { get; init; }
	/// <summary>Gets the content room index.</summary>
	public int RoomIndex { get; init; }
	/// <summary>Gets the content X (percent).</summary>
	public float ContentX { get; init; }
	/// <summary>Gets the content Y (percent).</summary>
	public float ContentY { get; init; }
	/// <summary>Gets the content zoom (percent).</summary>
	public float Zoom { get; init; }
	/// <summary>Gets the content angle in degrees.</summary>
	public float Angle { get; init; }
	/// <summary>Gets the window dance state, when a dance has been applied.</summary>
	public WindowDanceSnapshot? Dance { get; init; }
}

/// <summary>
/// The window dance state.
/// </summary>
public readonly struct WindowDanceSnapshot
{
	/// <summary>Gets the dance preset.</summary>
	public WindowDancePreset Preset { get; init; }
	/// <summary>Gets the current dance X position (percent).</summary>
	public float X { get; init; }
	/// <summary>Gets the current dance Y position (percent).</summary>
	public float Y { get; init; }
	/// <summary>Gets the amplitude.</summary>
	public float Amplitude { get; init; }
	/// <summary>Gets the angle in degrees.</summary>
	public float Angle { get; init; }
	/// <summary>Gets the frequency.</summary>
	public float Frequency { get; init; }
	/// <summary>Gets the duration.</summary>
	public float Duration { get; init; }
}

/// <summary>
/// The state of every game window.
/// </summary>
public readonly struct WindowsSnapshot
{
	/// <summary>Gets the window snapshots.</summary>
	public IReadOnlyList<WindowSnapshot> Items { get; init; }
	/// <summary>Gets the index of the main window.</summary>
	public int MainWindow { get; init; }
	/// <summary>Gets the current window order.</summary>
	public int[] Order { get; init; }
}

/// <summary>
/// Tracks window name, visibility, resize and content.
/// </summary>
internal sealed class WindowModule : IPlayheadObject
{
	private readonly List<WindowState> _windows = [new WindowState()];
	private int _mainWindow;
	private int[] _order = [];

	internal WindowModule(Playhead playhead)
	{
		Playhead = playhead;
	}

	private WindowModule(WindowModule source)
	{
		Playhead = source.Playhead;
		_windows = new List<WindowState>(source._windows.Count);
		foreach (WindowState window in source._windows)
			_windows.Add(window.Clone());
		_mainWindow = source._mainWindow;
		_order = (int[])source._order.Clone();
	}

	public Playhead Playhead { get; private set; }

	public void Rebind(Playhead playhead) => Playhead = playhead;

	public PlayheadModules Module => PlayheadModules.Windows;

	public string Id => "windows";

	public PlayheadObjectKind Kind => PlayheadObjectKind.Window;

	public ReadOnlyEnumCollection<EventType> RecordingTypes { get; } =
	[
		EventType.RenameWindow,
		EventType.HideWindow,
		EventType.WindowResize,
		EventType.SetWindowContent,
		EventType.SetMainWindow,
		EventType.ReorderWindows,
		EventType.NewWindowDance,
	];

	public void MoveNext(IBaseEvent e)
	{
		if (e is not BaseWindowEvent window)
			return;
		WindowState state = Ensure(window.TargetWindow);
		float tick = e.TickTime.Tick;
		switch (e)
		{
			case RenameWindow rename:
				string renameText = Playhead.Interpolate(rename.Text);
				state.Name = rename.Action switch
				{
					WindowNameAction.Set => renameText,
					WindowNameAction.Append => state.Name + renameText,
					WindowNameAction.Reset => "",
					_ => state.Name + renameText,
				};
				break;
			case HideWindow hide:
				state.Visible = hide.Show;
				state.Transparent = hide.Transparent;
				state.Frameless = hide.Frameless;
				break;
			case WindowResize resize:
				if (resize.Scale is { } scale)
				{
					if (scale.Width is { } width)
						state.ScaleX = FloatTween.Start(state.ScaleX.At(tick), Playhead.EvaluateExpression(width), tick, resize.Duration, resize.Ease);
					if (scale.Height is { } height)
						state.ScaleY = FloatTween.Start(state.ScaleY.At(tick), Playhead.EvaluateExpression(height), tick, resize.Duration, resize.Ease);
				}
				if (resize.Pivot is { } pivot)
				{
					if (pivot.X is { } ox)
						state.OriginX = FloatTween.Start(state.OriginX.At(tick), ox, tick, resize.Duration, resize.Ease);
					if (pivot.Y is { } oy)
						state.OriginY = FloatTween.Start(state.OriginY.At(tick), oy, tick, resize.Duration, resize.Ease);
				}
				break;
			case SetWindowContent content:
				if (content.ContentMode is { } mode)
					state.ContentMode = mode;
				state.RoomIndex = content.RoomIndex;
				if (content.Position is { } position)
				{
					if (position.X is { } x)
						state.ContentX = FloatTween.Start(state.ContentX.At(tick), x, tick, content.Duration, content.Ease);
					if (position.Y is { } y)
						state.ContentY = FloatTween.Start(state.ContentY.At(tick), y, tick, content.Duration, content.Ease);
				}
				if (content.Zoom is { } zoom)
					state.Zoom = FloatTween.Start(state.Zoom.At(tick), zoom, tick, content.Duration, content.Ease);
				if (content.Angle is { } angle)
					state.Angle = FloatTween.Start(state.Angle.At(tick), angle, tick, content.Duration, content.Ease);
				break;
			case SetMainWindow main:
				_mainWindow = main.TargetWindow;
				break;
			case ReorderWindows reorder:
				_order = [.. reorder.Order];
				break;
			case NewWindowDance dance:
				state.DancePreset = dance.Preset;
				state.DanceAmplitude = dance.Amplitude ?? 0f;
				state.DanceAngle = dance.Angle ?? 0f;
				state.DanceFrequency = dance.Frequency;
				state.DanceDuration = dance.Duration;
				state.DanceX = FloatTween.Start(state.DanceX.At(tick), dance.Position.X ?? 50f, tick, dance.Duration, dance.Ease);
				state.DanceY = FloatTween.Start(state.DanceY.At(tick), dance.Position.Y ?? 50f, tick, dance.Duration, dance.Ease);
				break;
		}
	}

	private WindowState Ensure(int index)
	{
		if (index < 0)
			index = 0;
		while (_windows.Count <= index)
			_windows.Add(new WindowState());
		return _windows[index];
	}

	public object Snapshot()
	{
		float tick = Playhead.CurrentTickTime.Tick;
		List<WindowSnapshot> items = [];
		for (int i = 0; i < _windows.Count; i++)
			items.Add(BuildWindow(i, tick));
		return new WindowsSnapshot { Items = items, MainWindow = _mainWindow, Order = (int[])_order.Clone() };
	}

	public IReadOnlyList<PlayheadObjectSnapshot> Describe()
	{
		float tick = Playhead.CurrentTickTime.Tick;
		List<PlayheadObjectSnapshot> objects = [];
		for (int i = 0; i < _windows.Count; i++)
			objects.Add(new PlayheadObjectSnapshot { Id = "window" + i, Kind = PlayheadObjectKind.Window, State = BuildWindow(i, tick) });
		return objects;
	}

	private WindowSnapshot BuildWindow(int i, float tick)
	{
		WindowState window = _windows[i];
		return new WindowSnapshot
		{
			Index = i,
			Name = window.Name,
			Visible = window.Visible,
			Transparent = window.Transparent,
			Frameless = window.Frameless,
			ScaleX = window.ScaleX.At(tick),
			ScaleY = window.ScaleY.At(tick),
			OriginX = window.OriginX.At(tick),
			OriginY = window.OriginY.At(tick),
			ContentMode = window.ContentMode,
			RoomIndex = window.RoomIndex,
			ContentX = window.ContentX.At(tick),
			ContentY = window.ContentY.At(tick),
			Zoom = window.Zoom.At(tick),
			Angle = window.Angle.At(tick),
			Dance = window.DancePreset is { } dancePreset
				? new WindowDanceSnapshot
				{
					Preset = dancePreset,
					X = window.DanceX.At(tick),
					Y = window.DanceY.At(tick),
					Amplitude = window.DanceAmplitude,
					Angle = window.DanceAngle,
					Frequency = window.DanceFrequency,
					Duration = window.DanceDuration,
				}
				: null,
		};
	}

	public IPlayheadObject Clone(Playhead playhead) => new WindowModule(this) { Playhead = playhead };

	private sealed class WindowState
	{
		public string Name = "";
		public bool Visible = true;
		public bool Transparent;
		public bool Frameless;
		public FloatTween ScaleX = FloatTween.Constant(100f);
		public FloatTween ScaleY = FloatTween.Constant(100f);
		public FloatTween OriginX = FloatTween.Constant(50f);
		public FloatTween OriginY = FloatTween.Constant(50f);
		public WindowContentMode? ContentMode;
		public int RoomIndex;
		public FloatTween ContentX = FloatTween.Constant(50f);
		public FloatTween ContentY = FloatTween.Constant(50f);
		public FloatTween Zoom = FloatTween.Constant(100f);
		public FloatTween Angle = FloatTween.Constant(0f);
		public WindowDancePreset? DancePreset;
		public FloatTween DanceX = FloatTween.Constant(50f);
		public FloatTween DanceY = FloatTween.Constant(50f);
		public float DanceAmplitude;
		public float DanceAngle;
		public float DanceFrequency;
		public float DanceDuration;

		public WindowState Clone() => (WindowState)MemberwiseClone();
	}
}


