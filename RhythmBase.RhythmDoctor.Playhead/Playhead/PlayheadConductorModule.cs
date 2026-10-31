using System.Text.Json;
using RhythmBase.Global.Components;
using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components.Vector;
using RhythmBase.RhythmDoctor.Events;
using RhythmBase.RhythmDoctor.Extensions;
using RhythmBase.RhythmDoctor.Utils;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Tracks BPM, crotchets per bar, speed and play style.
/// </summary>
internal sealed class ConductorModule : IPlayheadObject
{
	private Playhead _playhead;
	private float _bpm = 100f;
	private int _cpb = 4;
	private float _speed = 1f;
	private PlayStyleType _playStyle = PlayStyleType.Normal;
	private PlayStyleType _playStyleCache = PlayStyleType.Normal;
	private int _tempCounter;
	private int _nextBarToGo;
	private bool _hasNextBar;

	internal ConductorModule(Playhead playhead)
	{
		_playhead = playhead;
	}

	internal ConductorModule(ConductorModule source)
	{
		_playhead = source._playhead;
		_bpm = source._bpm;
		_cpb = source._cpb;
		_speed = source._speed;
		_playStyle = source._playStyle;
		_playStyleCache = source._playStyleCache;
		_tempCounter = source._tempCounter;
		_nextBarToGo = source._nextBarToGo;
		_hasNextBar = source._hasNextBar;
	}

	public Playhead Playhead => _playhead;

	public void Rebind(Playhead playhead) => _playhead = playhead;

	public PlayheadModules Module => PlayheadModules.Time;

	public string Id => "conductor";

	public PlayheadObjectKind Kind => PlayheadObjectKind.Conductor;

	public IReadOnlyList<PlayheadObjectSnapshot> Describe() => [new PlayheadObjectSnapshot { Id = Id, Kind = Kind, State = Snapshot() }];

	public ReadOnlyEnumCollection<EventType> RecordingTypes { get; } =
	[
		EventType.SetBeatsPerMinute,
		EventType.SetCrotchetsPerBar,
		EventType.SetSpeed,
		EventType.SetPlayStyle,
	];

	internal PlayStyleType PlayStyle => _playStyle;

	internal float BeatsPerMinute => _bpm;

	internal int CrotchetsPerBar => _cpb;

	internal float Speed => _speed;

	/// <summary>
	/// Gets whether the current play style holds or loops time, preventing a target beyond the
	/// current bar from being reached.
	/// </summary>
	internal bool IsHolding => _playStyle is PlayStyleType.Loop or PlayStyleType.BeatLoopOnly or PlayStyleType.Prolong or PlayStyleType.ProlongOneBar;

	internal float CurrentBarEndTick(BeatCalculator calculator, float tick)
	{
		(int bar, _) = calculator.TickToBarBeat(tick);
		return calculator.BarBeatToTick(bar + 1, 1);
	}

	public void MoveNext(IBaseEvent e)
	{
		switch (e)
		{
			case SetBeatsPerMinute bpm:
				_bpm = bpm.BeatsPerMinute;
				break;
			case SetCrotchetsPerBar cpb:
				_cpb = cpb.CrotchetsPerBar;
				break;
			case SetSpeed speed:
				_speed = speed.Speed;
				break;
			case SetPlayStyle style:
				ApplyPlayStyle(style);
				break;
		}
	}

	/// <summary>Holds time (no bar advance).</summary>
	internal const int BoundaryHold = 0;
	/// <summary>Replays the current bar.</summary>
	internal const int BoundaryLoopBar = -1;
	/// <summary>Replays the current beat.</summary>
	internal const int BoundaryLoopBeat = -2;

	private void ApplyPlayStyle(SetPlayStyle style)
	{
		switch (style.PlayStyle)
		{
			case PlayStyleType.Immediately:
			case PlayStyleType.ExtraImmediately:
				_playStyle = PlayStyleType.Normal;
				_playStyleCache = PlayStyleType.Normal;
				break;
			case PlayStyleType.OnNextBar:
				_playStyleCache = PlayStyleType.Normal;
				break;
			case PlayStyleType.Loop:
				_playStyleCache = PlayStyleType.Loop;
				break;
			case PlayStyleType.BeatLoopOnly:
				_playStyleCache = PlayStyleType.BeatLoopOnly;
				break;
			case PlayStyleType.Prolong:
				_playStyleCache = PlayStyleType.Prolong;
				break;
			case PlayStyleType.ProlongOneBar:
				_playStyle = PlayStyleType.Prolong;
				_playStyleCache = PlayStyleType.Normal;
				_tempCounter = 1;
				break;
			case PlayStyleType.ScrubToNext:
			case PlayStyleType.Normal:
				_playStyleCache = PlayStyleType.Normal;
				break;
		}

		if (style.PlayStyle != PlayStyleType.Loop)
		{
			(int eventBar, _) = style.TickTime;
			_nextBarToGo = style.IsRelative ? eventBar + style.NextBar : style.NextBar;
			_hasNextBar = true;
		}
	}

	/// <summary>
	/// Applies the pending play style at a bar boundary and returns what the playhead should do:
	/// a positive bar number to advance to, <see cref="BoundaryHold"/>,
	/// <see cref="BoundaryLoopBar"/> or <see cref="BoundaryLoopBeat"/>.
	/// </summary>
	internal int BeginBarBoundary(int currentBar)
	{
		if (_tempCounter > 0)
		{
			_tempCounter--;
			if (_tempCounter == 0)
				_playStyle = PlayStyleType.Normal;
		}
		else
		{
			_playStyle = _playStyleCache;
		}

		switch (_playStyle)
		{
			case PlayStyleType.Prolong:
				return BoundaryHold;
			case PlayStyleType.Loop:
				return BoundaryLoopBar;
			case PlayStyleType.BeatLoopOnly:
				return BoundaryLoopBeat;
			default:
				int targetBar = _hasNextBar && _nextBarToGo > 0 ? _nextBarToGo : currentBar + 1;
				_hasNextBar = false;
				_nextBarToGo = 0;
				return targetBar > 0 ? targetBar : currentBar + 1;
		}
	}

	public object Snapshot() => new ConductorSnapshot
	{
		TickTime = _playhead.CurrentTickTime,
		BeatsPerMinute = _bpm,
		CrotchetsPerBar = _cpb,
		Speed = _speed,
		PlayStyle = _playStyle,
	};

	public IPlayheadObject Clone(Playhead playhead) => new ConductorModule(this) { _playhead = playhead };
}
