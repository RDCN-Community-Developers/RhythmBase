using RhythmBase.Global.Components;
using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components.Vector;
using RhythmBase.RhythmDoctor.Events;
using RhythmBase.RhythmDoctor.Utils;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Lightweight conductor state of a playhead.
/// </summary>
public readonly struct PlayheadState
{
	/// <summary>Gets the current time.</summary>
	public TickTime TickTime { get; init; }
	/// <summary>Gets the current bar.</summary>
	public int Bar { get; init; }
	/// <summary>Gets the current beat within the bar.</summary>
	public float Beat { get; init; }
	/// <summary>Gets the current play style.</summary>
	public PlayStyleType PlayStyle { get; init; }
}

/// <summary>
/// A source of random numbers for a sandbox environment. Implement it with an iterator to make a
/// deterministic sandbox.
/// </summary>
public interface IRandomSimulator
{
	/// <summary>Returns a random integer in <c>[minInclusive, maxExclusive)</c>.</summary>
	/// <param name="minInclusive">The inclusive lower bound.</param>
	/// <param name="maxExclusive">The exclusive upper bound.</param>
	/// <returns>The random value.</returns>
	int Next(int minInclusive, int maxExclusive);
}

/// <summary>
/// The default <see cref="IRandomSimulator"/> backed by <see cref="Random"/>.
/// </summary>
public sealed class RandomSimulator : IRandomSimulator
{
	private readonly Random _random = new();

	/// <inheritdoc/>
	public int Next(int minInclusive, int maxExclusive) => _random.Next(minInclusive, maxExclusive);
}

/// <summary>
/// A scalar that tweens between two values over a number of ticks (crotchets).
/// </summary>
internal struct FloatTween
{
	private float _from;
	private float _to;
	private float _start;
	private float _duration;
	private EaseType _ease;

	internal bool Active;

	internal static FloatTween Start(float from, float to, float startTick, float duration, EaseType ease)
	{
		return new FloatTween
		{
			_from = from,
			_to = to,
			_start = startTick,
			_duration = duration,
			_ease = ease,
			Active = true,
		};
	}

	internal static FloatTween Constant(float value) => new() { _from = value, _to = value, _ease = EaseType.Linear, Active = false };

	internal readonly float At(float tick)
	{
		if (!Active || _duration <= 0f)
			return _to;
		float t = (tick - _start) / _duration;
		t = t < 0f ? 0f : t > 1f ? 1f : t;
		float eased = _ease == EaseType.Unset ? t : (float)_ease.Calculate(t);
		return _from + ((_to - _from) * eased);
	}
}

/// <summary>
/// A decoration's transform at a point in time. Positions, sizes and origins are percentages.
/// </summary>
public readonly struct DecorationTransform
{
	/// <summary>Gets the X position (percent).</summary>
	public float X { get; init; }
	/// <summary>Gets the Y position (percent).</summary>
	public float Y { get; init; }
	/// <summary>Gets the width (percent).</summary>
	public float Width { get; init; }
	/// <summary>Gets the height (percent).</summary>
	public float Height { get; init; }
	/// <summary>Gets the origin X (percent).</summary>
	public float OriginX { get; init; }
	/// <summary>Gets the origin Y (percent).</summary>
	public float OriginY { get; init; }
	/// <summary>Gets the angle in degrees.</summary>
	public float Angle { get; init; }
}

/// <summary>
/// The state of a decoration at a point in time.
/// </summary>
public readonly struct DecorationSnapshot
{
	/// <summary>Gets the decoration id.</summary>
	public string Id { get; init; }
	/// <summary>Gets the decoration room.</summary>
	public SingleRoom Room { get; init; }
	/// <summary>Gets the decoration type.</summary>
	public DecorationType Type { get; init; }
	/// <summary>Gets whether the decoration is visible.</summary>
	public bool Visible { get; init; }
	/// <summary>Gets the transform.</summary>
	public DecorationTransform Transform { get; init; }
	/// <summary>Gets the tint color, when a tint has been applied.</summary>
	public PaletteColorWithAlpha? TintColor { get; init; }
	/// <summary>Gets the opacity (0-100).</summary>
	public int Opacity { get; init; }
	/// <summary>Gets whether tinting is enabled.</summary>
	public bool IsTint { get; init; }
	/// <summary>Gets whether text content has been set.</summary>
	public bool HasText { get; init; }
	/// <summary>Gets the text content.</summary>
	public string Text { get; init; }
	/// <summary>Gets the text size.</summary>
	public int TextSize { get; init; }
	/// <summary>Gets the text color.</summary>
	public PaletteColorWithAlpha TextColor { get; init; }
	/// <summary>Gets the text outline color.</summary>
	public PaletteColorWithAlpha OutlineColor { get; init; }
	/// <summary>Gets the font.</summary>
	public string Font { get; init; }
	/// <summary>Gets how many times the text has been advanced.</summary>
	public int AdvanceCount { get; init; }
}

/// <summary>
/// The state of every decoration at a point in time.
/// </summary>
public readonly struct DecorationsSnapshot
{
	/// <summary>Gets the decoration snapshots.</summary>
	public IReadOnlyList<DecorationSnapshot> Items { get; init; }
}

/// <summary>
/// The state of one VFX preset on one room.
/// </summary>
public readonly struct VfxPresetState
{
	/// <summary>Gets the room index.</summary>
	public int Room { get; init; }
	/// <summary>Gets the preset.</summary>
	public VfxPreset Preset { get; init; }
	/// <summary>Gets whether the preset is enabled.</summary>
	public bool Enabled { get; init; }
	/// <summary>Gets the intensity.</summary>
	public float Intensity { get; init; }
	/// <summary>Gets the threshold.</summary>
	public float Threshold { get; init; }
}

/// <summary>
/// The layered VFX preset state of every room.
/// </summary>
public readonly struct VfxSnapshot
{
	/// <summary>Gets the active preset layers.</summary>
	public IReadOnlyList<VfxPresetState> Presets { get; init; }
}

/// <summary>
/// The VFX preset layers of one room.
/// </summary>
public readonly struct VfxRoomSnapshot
{
	/// <summary>Gets the room index.</summary>
	public int Room { get; init; }
	/// <summary>Gets the active preset layers of the room.</summary>
	public IReadOnlyList<VfxPresetState> Presets { get; init; }
}

/// <summary>
/// A recorded audio command.
/// </summary>
public readonly struct AudioCommand
{
	/// <summary>Gets the time of the command.</summary>
	public TickTime TickTime { get; init; }
	/// <summary>Gets the event type.</summary>
	public EventType Type { get; init; }
	/// <summary>Gets the audio file name.</summary>
	public string? Filename { get; init; }
	/// <summary>Gets the volume.</summary>
	public float Volume { get; init; }
	/// <summary>Gets the pitch.</summary>
	public float Pitch { get; init; }
	/// <summary>Gets the pan.</summary>
	public float Pan { get; init; }
}

/// <summary>
/// The audio state at a point in time.
/// </summary>
public readonly struct AudioSnapshot
{
	/// <summary>Gets the current song file name.</summary>
	public string? CurrentSong { get; init; }
	/// <summary>Gets whether a song is playing.</summary>
	public bool IsPlaying { get; init; }
	/// <summary>Gets the song volume.</summary>
	public float SongVolume { get; init; }
	/// <summary>Gets the song pitch.</summary>
	public float SongPitch { get; init; }
	/// <summary>Gets the song pan.</summary>
	public float SongPan { get; init; }
	/// <summary>Gets the audio commands seen so far.</summary>
	public IReadOnlyList<AudioCommand> Commands { get; init; }
	/// <summary>Gets player 1's clap sound.</summary>
	public AudioStateSnapshot? ClapP1 { get; init; }
	/// <summary>Gets player 2's clap sound.</summary>
	public AudioStateSnapshot? ClapP2 { get; init; }
	/// <summary>Gets the CPU clap sound.</summary>
	public AudioStateSnapshot? ClapCpu { get; init; }
	/// <summary>Gets the clap row type.</summary>
	public RowType? ClapRowType { get; init; }
	/// <summary>Gets whether the counting sound is enabled.</summary>
	public bool? CountingEnabled { get; init; }
	/// <summary>Gets the counting sound voice source.</summary>
	public CountingSoundVoiceSource? CountingVoice { get; init; }
	/// <summary>Gets the counting sound volume.</summary>
	public int? CountingVolume { get; init; }
	/// <summary>Gets the beat sound.</summary>
	public AudioStateSnapshot? BeatSound { get; init; }
	/// <summary>Gets the last game sound type.</summary>
	public SoundType? GameSoundType { get; init; }
	/// <summary>Gets the last game sound.</summary>
	public AudioStateSnapshot? GameSound { get; init; }
}

/// <summary>
/// The playback parameters of one audio.
/// </summary>
public readonly struct AudioStateSnapshot
{
	/// <summary>Gets the file name.</summary>
	public string? Filename { get; init; }
	/// <summary>Gets the volume.</summary>
	public float Volume { get; init; }
	/// <summary>Gets the pitch.</summary>
	public float Pitch { get; init; }
	/// <summary>Gets the pan.</summary>
	public float Pan { get; init; }
}

/// <summary>
/// The conductor snapshot.
/// </summary>
public readonly struct ConductorSnapshot
{
	/// <summary>Gets the time.</summary>
	public TickTime TickTime { get; init; }
	/// <summary>Gets the beats per minute.</summary>
	public float BeatsPerMinute { get; init; }
	/// <summary>Gets the crotchets per bar.</summary>
	public int CrotchetsPerBar { get; init; }
	/// <summary>Gets the speed.</summary>
	public float Speed { get; init; }
	/// <summary>Gets the play style.</summary>
	public PlayStyleType PlayStyle { get; init; }
}

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

/// <summary>
/// Tracks the visibility, transform and tint of one decoration.
/// </summary>
internal sealed class DecorationModule : IPlayheadObject
{
	private readonly string _id;
	private readonly SingleRoom _room;
	private readonly DecorationType _type;
	private float _visibleAt;
	private bool _visible;
	private FloatTween _x = FloatTween.Constant(50f);
	private FloatTween _y = FloatTween.Constant(50f);
	private FloatTween _width = FloatTween.Constant(100f);
	private FloatTween _height = FloatTween.Constant(100f);
	private FloatTween _originX = FloatTween.Constant(50f);
	private FloatTween _originY = FloatTween.Constant(50f);
	private FloatTween _angle = FloatTween.Constant(0f);
	private PaletteColorWithAlpha? _tintColor;
	private FloatTween _opacity = FloatTween.Constant(100f);
	private bool _isTint;
	private bool _hasText;
	private string _text = "";
	private int _textSize = 8;
	private PaletteColorWithAlpha _textColor;
	private PaletteColorWithAlpha _outlineColor;
	private string _font = "";
	private int _advanceCount;

	internal DecorationModule(Playhead playhead, Decoration decoration)
	{
		Playhead = playhead;
		_id = decoration.Id;
		_room = decoration.Room;
		_type = decoration.Type;
		_visible = decoration.Visible;
	}

	private DecorationModule(DecorationModule source)
	{
		Playhead = source.Playhead;
		_id = source._id;
		_room = source._room;
		_type = source._type;
		_visibleAt = source._visibleAt;
		_visible = source._visible;
		_x = source._x;
		_y = source._y;
		_width = source._width;
		_height = source._height;
		_originX = source._originX;
		_originY = source._originY;
		_angle = source._angle;
		_tintColor = source._tintColor;
		_opacity = source._opacity;
		_isTint = source._isTint;
		_hasText = source._hasText;
		_text = source._text;
		_textSize = source._textSize;
		_textColor = source._textColor;
		_outlineColor = source._outlineColor;
		_font = source._font;
		_advanceCount = source._advanceCount;
	}

	public Playhead Playhead { get; private set; }

	public void Rebind(Playhead playhead) => Playhead = playhead;

	public PlayheadModules Module => PlayheadModules.Decorations;

	public string Id => _id;

	public PlayheadObjectKind Kind => _type == DecorationType.Text ? PlayheadObjectKind.TextDecoration : PlayheadObjectKind.SpriteDecoration;

	public IReadOnlyList<PlayheadObjectSnapshot> Describe() => [new PlayheadObjectSnapshot { Id = Id, Kind = Kind, State = Snapshot() }];

	public ReadOnlyEnumCollection<EventType> RecordingTypes { get; } =
	[
		EventType.Move,
		EventType.SetVisible,
		EventType.Tint,
		EventType.SetText,
		EventType.TintText,
		EventType.SetFont,
		EventType.AdvanceTextDecoration,
	];

	public void MoveNext(IBaseEvent e)
	{
		if (e is not BaseDecorationAction action || action.Target != _id)
			return;
		float tick = e.TickTime.Tick;
		switch (action)
		{
			case Move move:
				ApplyMove(move, tick);
				break;
			case SetVisible visible:
				_visible = visible.Visible;
				_visibleAt = tick;
				break;
			case Tint tint:
				ApplyTint(tint, tick);
				break;
			case SetText text:
				ApplySetText(text, tick);
				break;
			case TintText tintText:
				if (tintText.TintColor is { } tintColor)
					_tintColor = tintColor;
				if (tintText.BorderColor is { } borderColor)
					_outlineColor = borderColor;
				break;
			case SetFont font:
				_font = font.Font.ToString() ?? "";
				break;
			case AdvanceTextDecoration:
				_advanceCount++;
				break;
		}
	}

	private void ApplySetText(SetText text, float tick)
	{
		_text = Playhead.Interpolate(text.Text);
		_textSize = text.Size;
		_textColor = text.Color;
		_outlineColor = text.OutlineColor;
		_hasText = true;
		_advanceCount = 0;
		if (text.Position is { } position)
		{
			if (position.X is { } x)
				_x = FloatTween.Start(_x.At(tick), x, tick, text.Duration, EaseType.Linear);
			if (position.Y is { } y)
				_y = FloatTween.Start(_y.At(tick), y, tick, text.Duration, EaseType.Linear);
		}
	}

	private void ApplyMove(Move move, float tick)
	{
		float duration = move.Duration;
		if (move.Position is { } position)
		{
			if (position.X is { } x)
				_x = FloatTween.Start(_x.At(tick), Playhead.EvaluateExpression(x), tick, duration, move.Ease);
			if (position.Y is { } y)
				_y = FloatTween.Start(_y.At(tick), Playhead.EvaluateExpression(y), tick, duration, move.Ease);
		}
		if (move.Scale is { } scale)
		{
			if (scale.Width is { } width)
				_width = FloatTween.Start(_width.At(tick), Playhead.EvaluateExpression(width), tick, duration, move.Ease);
			if (scale.Height is { } height)
				_height = FloatTween.Start(_height.At(tick), Playhead.EvaluateExpression(height), tick, duration, move.Ease);
		}
		if (move.Angle is { } angle)
			_angle = FloatTween.Start(_angle.At(tick), Playhead.EvaluateExpression(angle), tick, duration, move.Ease);
		if (move.Pivot is { } pivot)
		{
			if (pivot.X is { } ox)
				_originX = FloatTween.Start(_originX.At(tick), ox, tick, duration, move.Ease);
			if (pivot.Y is { } oy)
				_originY = FloatTween.Start(_originY.At(tick), oy, tick, duration, move.Ease);
		}
	}

	private void ApplyTint(Tint tint, float tick)
	{
		_tintColor = tint.TintColor;
		_isTint = tint.IsTint ?? true;
		if (tint.Opacity is { } opacity)
			_opacity = FloatTween.Start(_opacity.At(tick), opacity, tick, tint.Duration, tint.Ease);
	}

	public object Snapshot()
	{
		float tick = Playhead.CurrentTickTime.Tick;
		return new DecorationSnapshot
		{
			Id = _id,
			Room = _room,
			Type = _type,
			Visible = _visible,
			TintColor = _tintColor,
			Opacity = (int)Math.Round(_opacity.At(tick)),
			IsTint = _isTint,
			HasText = _hasText,
			Text = _text,
			TextSize = _textSize,
			TextColor = _textColor,
			OutlineColor = _outlineColor,
			Font = _font,
			AdvanceCount = _advanceCount,
			Transform = new DecorationTransform
			{
				X = _x.At(tick),
				Y = _y.At(tick),
				Width = _width.At(tick),
				Height = _height.At(tick),
				OriginX = _originX.At(tick),
				OriginY = _originY.At(tick),
				Angle = _angle.At(tick),
			},
		};
	}

	public IPlayheadObject Clone(Playhead playhead) => new DecorationModule(this) { Playhead = playhead };
}

/// <summary>
/// Tracks the layered VFX presets of every room.
/// </summary>
internal sealed class VfxModule : IPlayheadObject
{
	private readonly Dictionary<(int Room, VfxPreset Preset), VfxLayer> _layers = [];

	internal VfxModule(Playhead playhead)
	{
		Playhead = playhead;
	}

	private VfxModule(VfxModule source)
	{
		Playhead = source.Playhead;
		foreach (KeyValuePair<(int Room, VfxPreset Preset), VfxLayer> pair in source._layers)
			_layers[pair.Key] = pair.Value;
	}

	public Playhead Playhead { get; private set; }

	public void Rebind(Playhead playhead) => Playhead = playhead;

	public PlayheadModules Module => PlayheadModules.Vfx;

	public string Id => "vfx";

	public PlayheadObjectKind Kind => PlayheadObjectKind.Vfx;

	public IReadOnlyList<PlayheadObjectSnapshot> Describe()
	{
		float tick = Playhead.CurrentTickTime.Tick;
		Dictionary<int, List<VfxPresetState>> byRoom = [];
		foreach (KeyValuePair<(int Room, VfxPreset Preset), VfxLayer> pair in _layers)
		{
			int room = pair.Key.Room;
			if (!byRoom.TryGetValue(room, out List<VfxPresetState>? list))
			{
				list = [];
				byRoom[room] = list;
			}
			list.Add(new VfxPresetState
			{
				Room = room,
				Preset = pair.Key.Preset,
				Enabled = pair.Value.Enabled,
				Intensity = pair.Value.Intensity.At(tick),
				Threshold = pair.Value.Threshold.At(tick),
			});
		}
		List<PlayheadObjectSnapshot> objects = [];
		foreach (KeyValuePair<int, List<VfxPresetState>> pair in byRoom)
			objects.Add(new PlayheadObjectSnapshot { Id = "vfx.room" + pair.Key, Kind = PlayheadObjectKind.Vfx, State = new VfxRoomSnapshot { Room = pair.Key, Presets = pair.Value } });
		return objects;
	}

	public ReadOnlyEnumCollection<EventType> RecordingTypes { get; } = [EventType.SetVfxPreset];

	public void MoveNext(IBaseEvent e)
	{
		if (e is not SetVfxPreset set)
			return;
		float tick = e.TickTime.Tick;
		foreach (byte room in set.Rooms.Rooms)
		{
			(int Room, VfxPreset Preset) key = (room, set.Preset);
			if (!_layers.TryGetValue(key, out VfxLayer layer))
				layer = new VfxLayer { Intensity = FloatTween.Constant(set.Intensity ?? 100f), Threshold = FloatTween.Constant(set.Threshold ?? 0f) };
			layer.Enabled = set.Enable;
			if (set.Intensity is { } intensity)
				layer.Intensity = FloatTween.Start(layer.Intensity.At(tick), intensity, tick, set.Duration, set.Ease);
			if (set.Threshold is { } threshold)
				layer.Threshold = FloatTween.Start(layer.Threshold.At(tick), threshold, tick, set.Duration, set.Ease);
			_layers[key] = layer;
		}
	}

	public object Snapshot()
	{
		float tick = Playhead.CurrentTickTime.Tick;
		List<VfxPresetState> states = [];
		foreach (KeyValuePair<(int Room, VfxPreset Preset), VfxLayer> pair in _layers)
		{
			(int room, VfxPreset preset) = pair.Key;
			VfxLayer layer = pair.Value;
			states.Add(new VfxPresetState
			{
				Room = room,
				Preset = preset,
				Enabled = layer.Enabled,
				Intensity = layer.Intensity.At(tick),
				Threshold = layer.Threshold.At(tick),
			});
		}
		return new VfxSnapshot { Presets = states };
	}

	public IPlayheadObject Clone(Playhead playhead) => new VfxModule(this) { Playhead = playhead };

	private struct VfxLayer
	{
		public bool Enabled;
		public FloatTween Intensity;
		public FloatTween Threshold;
	}
}

/// <summary>
/// Records audio commands and tracks the current song.
/// </summary>
internal sealed class AudioModule : IPlayheadObject
{
	private readonly List<AudioCommand> _commands = [];
	private string? _currentSong;
	private float _songVolume;
	private float _songPitch;
	private float _songPan;
	private bool _isPlaying;
	private Audio? _clapP1;
	private Audio? _clapP2;
	private Audio? _clapCpu;
	private RowType? _clapRowType;
	private bool? _countingEnabled;
	private CountingSoundVoiceSource? _countingVoice;
	private int? _countingVolume;
	private Audio? _beatSound;
	private SoundType? _gameSoundType;
	private Audio? _gameSound;

	internal AudioModule(Playhead playhead)
	{
		Playhead = playhead;
	}

	private AudioModule(AudioModule source)
	{
		Playhead = source.Playhead;
		_commands = [.. source._commands];
		_currentSong = source._currentSong;
		_songVolume = source._songVolume;
		_songPitch = source._songPitch;
		_songPan = source._songPan;
		_isPlaying = source._isPlaying;
		_clapP1 = source._clapP1;
		_clapP2 = source._clapP2;
		_clapCpu = source._clapCpu;
		_clapRowType = source._clapRowType;
		_countingEnabled = source._countingEnabled;
		_countingVoice = source._countingVoice;
		_countingVolume = source._countingVolume;
		_beatSound = source._beatSound;
		_gameSoundType = source._gameSoundType;
		_gameSound = source._gameSound;
	}

	public Playhead Playhead { get; private set; }

	public void Rebind(Playhead playhead) => Playhead = playhead;

	public PlayheadModules Module => PlayheadModules.Audio;

	public string Id => "audio";

	public PlayheadObjectKind Kind => PlayheadObjectKind.Audio;

	public IReadOnlyList<PlayheadObjectSnapshot> Describe() => [new PlayheadObjectSnapshot { Id = Id, Kind = Kind, State = Snapshot() }];

	public ReadOnlyEnumCollection<EventType> RecordingTypes { get; } =
	[
		EventType.PlaySong,
		EventType.PlaySound,
		EventType.SetGameSound,
		EventType.SetClapSounds,
		EventType.SetCountingSound,
		EventType.SetBeatSound,
	];

	public void MoveNext(IBaseEvent e)
	{
		switch (e)
		{
			case PlaySong song:
				_currentSong = song.Song.Filename.ToString();
				_songVolume = song.Song.Volume;
				_songPitch = song.Song.Pitch;
				_songPan = song.Song.Pan;
				_isPlaying = true;
				_commands.Add(Create(e, _currentSong, _songVolume, _songPitch, _songPan));
				break;
			case PlaySound sound:
				_commands.Add(Create(e, sound.Sound?.Filename.ToString(), sound.Sound?.Volume ?? 0f, sound.Sound?.Pitch ?? 0f, sound.Sound?.Pan ?? 0f));
				break;
			case SetClapSounds clap:
				_clapP1 = clap.P1Sound;
				_clapP2 = clap.P2Sound;
				_clapCpu = clap.CpuSound;
				_clapRowType = clap.RowType;
				_commands.Add(Create(e, clap.P1Sound?.Filename.ToString(), clap.P1Sound?.Volume ?? 0f, clap.P1Sound?.Pitch ?? 0f, clap.P1Sound?.Pan ?? 0f));
				break;
			case SetCountingSound counting:
				_countingEnabled = counting.Enabled;
				_countingVoice = counting.VoiceSource;
				_countingVolume = counting.Volume;
				_commands.Add(Create(e, null, counting.Volume, 0f, 0f));
				break;
			case SetBeatSound beat:
				_beatSound = beat.Sound;
				_commands.Add(Create(e, beat.Sound?.Filename.ToString(), beat.Sound?.Volume ?? 0f, beat.Sound?.Pitch ?? 0f, beat.Sound?.Pan ?? 0f));
				break;
			case SetGameSound game:
				_gameSoundType = game.SoundType;
				_gameSound = System.Linq.Enumerable.FirstOrDefault(System.Linq.Enumerable.OfType<Audio>(game.Sounds.Values));
				_commands.Add(Create(e, _gameSound?.Filename.ToString(), _gameSound?.Volume ?? 0f, _gameSound?.Pitch ?? 0f, _gameSound?.Pan ?? 0f));
				break;
			default:
				_commands.Add(Create(e, null, 0f, 0f, 0f));
				break;
		}
	}

	private static AudioStateSnapshot? ToAudio(Audio? audio) => audio is null
		? null
		: new AudioStateSnapshot
		{
			Filename = audio.Filename.ToString(),
			Volume = audio.Volume,
			Pitch = audio.Pitch,
			Pan = audio.Pan,
		};

	private static AudioCommand Create(IBaseEvent e, string? filename, float volume, float pitch, float pan) => new()
	{
		TickTime = e.TickTime,
		Type = e.Type,
		Filename = filename,
		Volume = volume,
		Pitch = pitch,
		Pan = pan,
	};

	public object Snapshot() => new AudioSnapshot
	{
		CurrentSong = _currentSong,
		IsPlaying = _isPlaying,
		SongVolume = _songVolume,
		SongPitch = _songPitch,
		SongPan = _songPan,
		Commands = _commands,
		ClapP1 = ToAudio(_clapP1),
		ClapP2 = ToAudio(_clapP2),
		ClapCpu = ToAudio(_clapCpu),
		ClapRowType = _clapRowType,
		CountingEnabled = _countingEnabled,
		CountingVoice = _countingVoice,
		CountingVolume = _countingVolume,
		BeatSound = ToAudio(_beatSound),
		GameSoundType = _gameSoundType,
		GameSound = ToAudio(_gameSound),
	};

	public IPlayheadObject Clone(Playhead playhead) => new AudioModule(this) { Playhead = playhead };
}





