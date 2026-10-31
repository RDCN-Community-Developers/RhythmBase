using System.Text.Json;
using RhythmBase.Global.Components;
using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components.Vector;
using RhythmBase.RhythmDoctor.Events;
using RhythmBase.RhythmDoctor.Extensions;
using RhythmBase.RhythmDoctor.Utils;

namespace RhythmBase.RhythmDoctor.Components;

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
	private Dictionary<RowType, ClapSounds> _claps = [];
	private bool? _countingEnabled;
	private CountingSoundVoiceSource? _countingVoice;
	private int? _countingVolume;
	private Audio? _beatSound;
	private Dictionary<int, Audio> _rowBeatSounds = [];
	private SoundType? _gameSoundType;
	private Audio? _gameSound;
	private static readonly Audio DefaultBeatSound = new() { Filename = "Shaker" };

	/// <summary>A <c>SetClapSounds</c> override for one <see cref="RowType"/> (classic vs. one-shot).</summary>
	private sealed record ClapSounds(Audio? P1, Audio? P2, Audio? Cpu);

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
		_claps = new(source._claps);
		_countingEnabled = source._countingEnabled;
		_countingVoice = source._countingVoice;
		_countingVolume = source._countingVolume;
		_beatSound = source._beatSound;
		_rowBeatSounds = new(source._rowBeatSounds);
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
		EventType.AddClassicBeat,
		EventType.AddOneshotBeat,
		EventType.AddFreeTimeBeat,
		EventType.PulseFreeTimeBeat,
		EventType.SayReadyGetSetGo,
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
				_commands.Add(Create(e, _currentSong, _songVolume, _songPitch, _songPan, song.Song.Offset, song.Loop));
				break;
			case PlaySound sound:
				_commands.Add(Create(e, sound.Sound?.Filename.ToString(), sound.Sound?.Volume ?? 0f, sound.Sound?.Pitch ?? 0f, sound.Sound?.Pan ?? 0f, sound.Sound?.Offset ?? default));
				break;
			case SetClapSounds clap:
				_clapP1 = clap.P1Sound;
				_clapP2 = clap.P2Sound;
				_clapCpu = clap.CpuSound;
				_clapRowType = clap.RowType;
				_claps[clap.RowType] = new ClapSounds(clap.P1Sound, clap.P2Sound, clap.CpuSound);
				_commands.Add(Create(e, clap.P1Sound?.Filename.ToString(), clap.P1Sound?.Volume ?? 0f, clap.P1Sound?.Pitch ?? 0f, clap.P1Sound?.Pan ?? 0f, clap.P1Sound?.Offset ?? default));
				break;
			case SetCountingSound counting:
				_countingEnabled = counting.Enabled;
				_countingVoice = counting.VoiceSource;
				_countingVolume = counting.Volume;
				_commands.Add(Create(e, null, counting.Volume, 0f, 0f));
				break;
			case SetBeatSound beat:
				// The game applies SetBeatSound to a single row (Row.SetPulseSounds); it never plays a
				// sound at the event itself. Row -1 is a RhythmBase convenience for every row.
				if (beat.Row >= 0)
					_rowBeatSounds[beat.Row] = beat.Sound;
				else
					_beatSound = beat.Sound;
				_commands.Add(Create(e, beat.Sound?.Filename.ToString(), beat.Sound?.Volume ?? 0f, beat.Sound?.Pitch ?? 0f, beat.Sound?.Pan ?? 0f, beat.Sound?.Offset ?? default));
				break;
			case SetGameSound game:
				_gameSoundType = game.SoundType;
				_gameSound = System.Linq.Enumerable.FirstOrDefault(System.Linq.Enumerable.OfType<Audio>(game.Sounds.Values));
				_commands.Add(Create(e, _gameSound?.Filename.ToString(), _gameSound?.Volume ?? 0f, _gameSound?.Pitch ?? 0f, _gameSound?.Pan ?? 0f, _gameSound?.Offset ?? default));
				break;
			case AddClassicBeat classic:
				AddClassicBeatCommands(classic);
				break;
			case AddOneshotBeat oneshot:
				AddOneshotBeatCommands(oneshot);
				break;
			case AddFreeTimeBeat free:
				AddBeat(free, null, free.TickTime.Tick);
				EmitHit(free, free.Row, free.TickTime.Tick, oneshot: false);
				break;
			case PulseFreeTimeBeat pulse:
				AddBeat(pulse, null, pulse.TickTime.Tick);
				break;
			case SayReadyGetSetGo say:
				AddSayReadyGetSetGoCommands(say);
				break;
			default:
				_commands.Add(Create(e, null, 0f, 0f, 0f));
				break;
		}
	}

	/// <summary>
	/// Emits a sound for each pulse of a classic beat (Add7BeatsClassic in the game): the pulses are
	/// <see cref="AddClassicBeat.Tick"/> beats apart and swung when <see cref="AddClassicBeat.Swing"/> is set.
	/// </summary>
	private void AddClassicBeatCommands(AddClassicBeat beat)
	{
		int length = Math.Max(1, beat.Length);
		// SetRowXs hides beatboxes: a hidden ('x') pulse makes no sound (Row.show == false). The 7th
		// beat is always audible (BeatClassic.Add7BeatsFreetimeFlexible).
		PatternCollection pattern = beat.Pattern;
		if (beat.Hold != 0f)
		{
			// Held beats pulse on every beatbox except the last: beatboxes 1-6 play the held-pulse
			// start/end sounds and the 7th plays the held clap (BeatClassic.Add7BeatsFreetimeFlexible).
			// The game's short/long and alternating variants need per-sound offset metadata, so the
			// long, non-alternating forms are used.
			bool p2 = IsP2(beat.Row);
			for (int pulse = 0; pulse < length; pulse++)
			{
				if (!IsBeatboxShown(pattern, 8 - length + pulse))
					continue;
				float start = beat.TickTime.Tick + PulseOffset(beat, pulse);
				float end = start + beat.Hold;
				if (pulse == length - 1)
				{
					EmitType(beat, start, p2 ? SoundType.ClapSoundHoldLongStartP2 : SoundType.ClapSoundHoldLongStart);
					EmitType(beat, end, p2 ? SoundType.ClapSoundHoldLongEndP2 : SoundType.ClapSoundHoldLongEnd);
				}
				else
				{
					EmitType(beat, start, p2 ? SoundType.PulseSoundHoldStartP2 : SoundType.PulseSoundHoldStart);
					EmitType(beat, end, p2 ? SoundType.PulseSoundHoldEndP2 : SoundType.PulseSoundHoldEnd);
				}
			}
			return;
		}
		Audio sound = beat.Sound ?? ResolveBeatSound(beat.Row);
		for (int pulse = 0; pulse < length; pulse++)
		{
			int beatbox = 8 - length + pulse;
			if (!IsBeatboxShown(pattern, beatbox))
				continue;
			float tick = beat.TickTime.Tick + PulseOffset(beat, pulse);
			_commands.Add(Create(beat, sound.Filename.ToString(), sound.Volume, sound.Pitch, sound.Pan, sound.Offset, false, tick));
			// The 7th beat is the one the player hits: it carries the clap hit sound.
			if (pulse == length - 1)
				EmitHit(beat, beat.Row, tick, oneshot: false);
		}
	}

	/// <summary>Beatboxes 1-6 follow the row's <c>SetRowXs</c> pattern ('x' hides the beat); the 7th always plays.</summary>
	private static bool IsBeatboxShown(PatternCollection pattern, int beatbox)
		=> beatbox == 7 || beatbox is < 1 or > 6 || pattern[beatbox - 1] != Pattern.X;

	/// <summary>Gets the swing-aware beat offset of a classic pulse (LevelEvent_AddClassicBeat.GetOffsetFromSwingNew).</summary>
	private static float PulseOffset(AddClassicBeat beat, int pulse)
		=> beat.Swing != 0f && pulse % 2 == 1
			? (beat.Tick * (pulse + 1)) - beat.Swing
			: (float)Math.Round(beat.Tick * pulse, 4);

	/// <summary>
	/// Emits a one-shot beat sound. Freeze/burn/hold/skip shots use the official game sounds
	/// (resolved by the player from <see cref="SoundType"/>); otherwise the custom or beat sound is used.
	/// </summary>
	private void AddOneshotBeatCommands(AddOneshotBeat beat)
	{
		SoundType? type = null;
		Audio? sound = beat.Sound;
		if (sound is null)
		{
			type = beat.FreezeBurnMode switch
			{
				OneshotType.Freezeshot => SoundType.FreezeshotSound,
				OneshotType.Burnshot => SoundType.BurnshotSound,
				_ when beat.Skipshot => SoundType.Skipshot,
				_ when beat.Hold => SoundType.HoldshotSound,
				_ => null,
			};
			if (type is null)
				sound = ResolveBeatSound(beat.Row);
		}
		if (type is not null)
		{
			_commands.Add(Create(beat, sound?.Filename.ToString(), sound?.Volume ?? 100f, sound?.Pitch ?? 100f, sound?.Pan ?? 0f, sound?.Offset ?? default, false, beat.TickTime.Tick + beat.Delay, type));
			return;
		}

		// Triangle one-shots play their beat sound once per subdivision; square and triangle shots
		// swap in the dedicated subdivision clips when subdivSound is enabled (LevelEvent_AddOneshotBeat.Run).
		Audio beatSound = sound ?? ResolveBeatSound(beat.Row);
		int subdivisions = beat.PulseType == OneshotPulseShapeType.Triangle ? Math.Max(1, (int)beat.Subdivisions) : 1;
		for (int subdivision = 0; subdivision < subdivisions; subdivision++)
		{
			string? filename = beatSound.Filename.ToString();
			if (beat.SubdivisionSound)
			{
				if (beat.PulseType == OneshotPulseShapeType.Triangle)
					filename = $"sndTriangleshot{subdivision + 1}";
				else if (beat.PulseType == OneshotPulseShapeType.Square)
					filename = "sndSquareshot";
			}
			float offset = beat.Tick * subdivision / subdivisions;
			float boom = beat.TickTime.Tick + offset;
			_commands.Add(Create(beat, filename, beatSound.Volume, beatSound.Pitch, beatSound.Pan, beatSound.Offset, false, boom));
			// The player hits one beat after the boom (plus the freeze delay): that is the clap hit.
			EmitHit(beat, beat.Row, boom + beat.Tick + beat.Delay, oneshot: true);
		}
	}

	private void AddBeat(IBaseEvent e, Audio? sound, float tick)
	{
		Audio resolved = sound ?? ResolveBeatSound((e as BaseRowAction)?.Row ?? -1);
		_commands.Add(Create(e, resolved.Filename.ToString(), resolved.Volume, resolved.Pitch, resolved.Pan, resolved.Offset, false, tick));
	}

	/// <summary>
	/// Emits the "Ready, Get Set, Go" count-in. The phrase is expanded into words (LevelEvent_SayReadyGetSetGo.Prepare)
	/// spaced <see cref="SayReadyGetSetGo.Tick"/> beats apart; each word plays a click plus a voiced line
	/// whose filename prefix depends on the voice source (LevelEvent_SayReadyGetSetGo.PlaySound).
	/// </summary>
	private void AddSayReadyGetSetGoCommands(SayReadyGetSetGo say)
	{
		if (say.VoiceSource is SayReadyGetSetGoVoiceSource.NoneBottom or SayReadyGetSetGoVoiceSource.NoneTop)
			return;
		foreach ((SayReadyGetSetGoWord word, float beatOffset) in SayWords(say.PhraseToSay, say.Tick))
		{
			float tick = say.TickTime.Tick + beatOffset;
			if (ClickFor(word) is { } click)
				_commands.Add(Create(say, click, 100f, 100f, 0f, default, false, tick));
			if (WordName(word) is { } name && VoicePrefix(say.VoiceSource, name) is { } prefix)
				_commands.Add(Create(say, prefix + Midfix(word) + name, say.Volume, 100f, 0f, default, false, tick));
		}
	}

	private static IEnumerable<(SayReadyGetSetGoWord Word, float BeatOffset)> SayWords(SayReadyGetSetGoWord phrase, float tick)
	{
		switch (phrase)
		{
			case SayReadyGetSetGoWord.SayReaDyGetSetGoNew:
				yield return (SayReadyGetSetGoWord.JustSayRea, 0f);
				yield return (SayReadyGetSetGoWord.JustSayDy, tick);
				yield return (SayReadyGetSetGoWord.JustSayGet, tick * 2f);
				yield return (SayReadyGetSetGoWord.JustSaySet, tick * 3f);
				yield return (SayReadyGetSetGoWord.JustSayGo, tick * 4f);
				break;
			case SayReadyGetSetGoWord.SayGetSetGo:
				yield return (SayReadyGetSetGoWord.JustSayGet, 0f);
				yield return (SayReadyGetSetGoWord.JustSaySet, tick);
				yield return (SayReadyGetSetGoWord.JustSayGo, tick * 2f);
				break;
			case SayReadyGetSetGoWord.SayReaDyGetSetOne:
				yield return (SayReadyGetSetGoWord.JustSayRea, 0f);
				yield return (SayReadyGetSetGoWord.JustSayDy, tick);
				yield return (SayReadyGetSetGoWord.JustSayGet, tick * 2f);
				yield return (SayReadyGetSetGoWord.JustSaySet, tick * 3f);
				yield return (SayReadyGetSetGoWord.Count1, tick * 4f);
				break;
			case SayReadyGetSetGoWord.SayGetSetOne:
				yield return (SayReadyGetSetGoWord.JustSayGet, 0f);
				yield return (SayReadyGetSetGoWord.JustSaySet, tick);
				yield return (SayReadyGetSetGoWord.Count1, tick * 2f);
				break;
			case SayReadyGetSetGoWord.SayReadyGetSetGo:
				// "Ready" leaves one beat of rest before "Get Set Go" (the game skips index 1).
				yield return (SayReadyGetSetGoWord.JustSayReady, 0f);
				yield return (SayReadyGetSetGoWord.JustSayGet, tick * 2f);
				yield return (SayReadyGetSetGoWord.JustSaySet, tick * 3f);
				yield return (SayReadyGetSetGoWord.JustSayGo, tick * 4f);
				break;
			default:
				yield return (phrase, 0f);
				break;
		}
	}

	private static string? WordName(SayReadyGetSetGoWord word) => word switch
	{
		SayReadyGetSetGoWord.JustSayReady => "Ready",
		SayReadyGetSetGoWord.JustSayRea => "Rea",
		SayReadyGetSetGoWord.JustSayDy => "Dy",
		SayReadyGetSetGoWord.JustSayGet => "Get",
		SayReadyGetSetGoWord.JustSaySet => "Set",
		SayReadyGetSetGoWord.JustSayGo => "Go",
		SayReadyGetSetGoWord.JustSayAnd => "And",
		SayReadyGetSetGoWord.JustSayStop => "Stop",
		SayReadyGetSetGoWord.JustSayAndStop => "AndStop",
		SayReadyGetSetGoWord.SaySwitch => "Switch",
		SayReadyGetSetGoWord.SayWatch => "Watch",
		SayReadyGetSetGoWord.SayListen => "Listen",
		SayReadyGetSetGoWord.Count1 => "1",
		SayReadyGetSetGoWord.Count2 => "2",
		SayReadyGetSetGoWord.Count3 => "3",
		SayReadyGetSetGoWord.Count4 => "4",
		SayReadyGetSetGoWord.Count5 => "5",
		SayReadyGetSetGoWord.Count6 => "6",
		SayReadyGetSetGoWord.Count7 => "7",
		SayReadyGetSetGoWord.Count8 => "8",
		SayReadyGetSetGoWord.Count9 => "9",
		SayReadyGetSetGoWord.Count10 => "10",
		_ => null,
	};

	private static string Midfix(SayReadyGetSetGoWord word) => word switch
	{
		SayReadyGetSetGoWord.JustSayAnd or SayReadyGetSetGoWord.JustSayStop or SayReadyGetSetGoWord.JustSayAndStop => "AndStop_",
		SayReadyGetSetGoWord.Count1 or SayReadyGetSetGoWord.Count2 or SayReadyGetSetGoWord.Count3 or SayReadyGetSetGoWord.Count4
			or SayReadyGetSetGoWord.Count5 or SayReadyGetSetGoWord.Count6 or SayReadyGetSetGoWord.Count7 or SayReadyGetSetGoWord.Count8
			or SayReadyGetSetGoWord.Count9 or SayReadyGetSetGoWord.Count10 => "CountEnglish",
		SayReadyGetSetGoWord.SaySwitch or SayReadyGetSetGoWord.SayWatch or SayReadyGetSetGoWord.SayListen => "",
		_ => "RDGS_",
	};

	private static string? ClickFor(SayReadyGetSetGoWord word) => word switch
	{
		SayReadyGetSetGoWord.JustSayReady or SayReadyGetSetGoWord.JustSayRea or SayReadyGetSetGoWord.JustSayDy => "sndClickLow",
		SayReadyGetSetGoWord.JustSayGet or SayReadyGetSetGoWord.JustSaySet => "sndClickMid",
		SayReadyGetSetGoWord.JustSayGo or SayReadyGetSetGoWord.JustSayStop => "sndClickHi",
		_ => null,
	};

	private static string? VoicePrefix(SayReadyGetSetGoVoiceSource source, string name)
	{
		static bool In(string value, params string[] set) => Array.IndexOf(set, value) >= 0;
		return source switch
		{
			SayReadyGetSetGoVoiceSource.Nurse => "sndJyi - ",
			SayReadyGetSetGoVoiceSource.NurseTired => In(name, "Rea", "Dy", "Get", "Set", "Go", "AndStop", "And", "Stop", "Switch", "Watch", "Listen") ? "sndJyiTired - " : "sndJyi - ",
			SayReadyGetSetGoVoiceSource.NurseSwing => In(name, "Rea", "Dy", "Get", "Set", "Go") ? "sndJyiSwing - " : "sndJyi - ",
			SayReadyGetSetGoVoiceSource.NurseSwingCalm => In(name, "Rea", "Dy", "Get", "Set", "Go")
				? "sndJyiSwingCalm - "
				: In(name, "Rea", "Dy", "Get", "Set", "Go", "AndStop", "And", "Stop", "Switch", "Watch", "Listen") ? "sndJyiTired - " : "sndJyi - ",
			SayReadyGetSetGoVoiceSource.IanExcited => "sndIan - ",
			SayReadyGetSetGoVoiceSource.IanCalm => In(name, "AndStop", "And", "Stop", "Watch", "Listen") ? "sndIan - " : "sndIanCalm - ",
			SayReadyGetSetGoVoiceSource.IanSlow => In(name, "AndStop", "And", "Stop", "Switch", "Watch", "Listen")
				? (In(name, "AndStop", "And", "Stop", "Watch", "Listen") ? "sndIan - " : "sndIanCalm - ")
				: "sndIanSlow - ",
			_ => null,
		};
	}

	/// <summary>
	/// Resolves the sound a beat uses: the row's <c>SetBeatSound</c>, else the row's own pulse sound
	/// from the level, else the last global override and finally the shaker default.
	/// </summary>
	private Audio ResolveBeatSound(int row)
	{
		if (row >= 0 && _rowBeatSounds.TryGetValue(row, out Audio? sound) && sound is not null)
			return sound;
		if (GetRow(row) is { } definition && !string.IsNullOrEmpty(definition.Sound.Filename.ToString()))
			return definition.Sound;
		return _beatSound ?? DefaultBeatSound;
	}

	private Row? GetRow(int index)
	{
		Chart chart = Playhead.CurrentChart;
		return index >= 0 && index < chart.Rows.Count ? chart.Rows[index] : null;
	}

	private bool IsP2(int row) => GetRow(row)?.Player == PlayerType.P2;

	/// <summary>
	/// Emits the clap that plays when a beat is hit. The default sound comes from the game's
	/// <c>ClapSound*</c> table by beat kind and player; a <c>SetClapSounds</c> override for the same
	/// kind takes precedence (Beat.hitSoundData / BeatOneshot.hitSoundData).
	/// </summary>
	private void EmitHit(IBaseEvent e, int row, float tick, bool oneshot)
	{
		RowType kind = oneshot ? RowType.Oneshot : RowType.Classic;
		PlayerType player = GetRow(row)?.Player ?? PlayerType.P1;
		if (_claps.TryGetValue(kind, out ClapSounds? set))
		{
			Audio? clap = player switch
			{
				PlayerType.P2 => set.P2,
				PlayerType.Cpu => set.Cpu,
				_ => set.P1,
			};
			if (clap is not null && !string.IsNullOrEmpty(clap.Filename.ToString()))
			{
				_commands.Add(Create(e, clap.Filename.ToString(), clap.Volume, clap.Pitch, clap.Pan, clap.Offset, false, tick));
				return;
			}
		}
		SoundType type = (oneshot, player) switch
		{
			(true, PlayerType.P2) => SoundType.ClapSoundP2Oneshot,
			(true, PlayerType.Cpu) => SoundType.ClapSoundCPUOneshot,
			(true, _) => SoundType.ClapSoundP1Oneshot,
			(false, PlayerType.P2) => SoundType.ClapSoundP2Classic,
			(false, PlayerType.Cpu) => SoundType.ClapSoundCPUClassic,
			_ => SoundType.ClapSoundP1Classic,
		};
		EmitType(e, tick, type);
	}

	private void EmitType(IBaseEvent e, float tick, SoundType type)
		=> _commands.Add(Create(e, null, 100f, 100f, 0f, default, false, tick, type));

	private static AudioStateSnapshot? ToAudio(Audio? audio) => audio is null
		? null
		: new AudioStateSnapshot
		{
			Filename = audio.Filename.ToString(),
			Volume = audio.Volume,
			Pitch = audio.Pitch,
			Pan = audio.Pan,
		};

	private static AudioCommand Create(IBaseEvent e, string? filename, float volume, float pitch, float pan, TimeSpan offset = default, bool loop = false, float? tick = null, SoundType? soundType = null) => new()
	{
		TickTime = e.TickTime,
		Type = e.Type,
		Filename = filename,
		Volume = volume,
		Pitch = pitch,
		Pan = pan,
		Offset = offset,
		Loop = loop,
		Tick = tick ?? e.TickTime.Tick,
		SoundType = soundType,
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





