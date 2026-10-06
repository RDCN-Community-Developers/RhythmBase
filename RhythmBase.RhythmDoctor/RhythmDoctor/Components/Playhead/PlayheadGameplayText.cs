using System.Text;
using RhythmBase.Global.Components;
using RhythmBase.RhythmDoctor.Events;
using RhythmBase.RhythmDoctor.Extensions;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// How many beats one row has received.
/// </summary>
public readonly struct RowBeatSnapshot
{
	/// <summary>Gets the row index.</summary>
	public int Index { get; init; }
	/// <summary>Gets the number of beat events on the row.</summary>
	public int Beats { get; init; }
}

/// <summary>
/// The gameplay state: beat counts and heart-explode parameters.
/// </summary>
public readonly struct GameplaySnapshot
{
	/// <summary>Gets the per-row beat counts.</summary>
	public IReadOnlyList<RowBeatSnapshot> Rows { get; init; }
	/// <summary>Gets the total number of beat events.</summary>
	public int TotalBeats { get; init; }
	/// <summary>Gets the heart explode interval, when set.</summary>
	public float? HeartExplodeInterval { get; init; }
	/// <summary>Gets the heart explode interval type, when set.</summary>
	public HeartExplodeIntervalType? HeartExplodeIntervalType { get; init; }
	/// <summary>Gets the heart explode volume, when set.</summary>
	public int? HeartExplodeVolume { get; init; }
}

/// <summary>
/// Tracks beat events and heart-explode parameters.
/// </summary>
internal sealed class GameplayModule : IPlayheadObject
{
	private readonly int[] _rowBeats;
	private int _totalBeats;
	private float? _heartInterval;
	private HeartExplodeIntervalType? _heartIntervalType;
	private int? _heartVolume;

	internal GameplayModule(Playhead playhead)
	{
		Playhead = playhead;
		_rowBeats = new int[playhead.CurrentChart.Rows.Count];
	}

	private GameplayModule(GameplayModule source)
	{
		Playhead = source.Playhead;
		_rowBeats = (int[])source._rowBeats.Clone();
		_totalBeats = source._totalBeats;
		_heartInterval = source._heartInterval;
		_heartIntervalType = source._heartIntervalType;
		_heartVolume = source._heartVolume;
	}

	public Playhead Playhead { get; private set; }

	public void Rebind(Playhead playhead) => Playhead = playhead;

	public PlayheadModules Module => PlayheadModules.Gameplay;

	public string Id => "gameplay";

	public PlayheadObjectKind Kind => PlayheadObjectKind.Gameplay;

	public IReadOnlyList<PlayheadObjectSnapshot> Describe() => [new PlayheadObjectSnapshot { Id = Id, Kind = Kind, State = Snapshot() }];

	public ReadOnlyEnumCollection<EventType> RecordingTypes { get; } =
	[
		EventType.AddClassicBeat,
		EventType.AddOneshotBeat,
		EventType.AddFreeTimeBeat,
		EventType.PulseFreeTimeBeat,
		EventType.SetHeartExplodeInterval,
		EventType.SetHeartExplodeVolume,
	];

	public void MoveNext(IBaseEvent e)
	{
		switch (e)
		{
			case BaseBeat beat:
				_totalBeats++;
				if (beat.Row >= 0 && beat.Row < _rowBeats.Length)
					_rowBeats[beat.Row]++;
				break;
			case SetHeartExplodeInterval interval:
				_heartInterval = interval.Interval;
				_heartIntervalType = interval.IntervalType;
				break;
			case SetHeartExplodeVolume volume:
				_heartVolume = volume.Volume;
				break;
		}
	}

	public object Snapshot()
	{
		List<RowBeatSnapshot> rows = [];
		for (int i = 0; i < _rowBeats.Length; i++)
			rows.Add(new RowBeatSnapshot { Index = i, Beats = _rowBeats[i] });
		return new GameplaySnapshot
		{
			Rows = rows,
			TotalBeats = _totalBeats,
			HeartExplodeInterval = _heartInterval,
			HeartExplodeIntervalType = _heartIntervalType,
			HeartExplodeVolume = _heartVolume,
		};
	}

	public IPlayheadObject Clone(Playhead playhead) => new GameplayModule(this) { Playhead = playhead };
}

/// <summary>
/// A recorded text command.
/// </summary>
public readonly struct TextCommand
{
	/// <summary>Gets the time of the command.</summary>
	public TickTime TickTime { get; init; }
	/// <summary>Gets the event type.</summary>
	public EventType Type { get; init; }
	/// <summary>Gets the text.</summary>
	public string Text { get; init; }
	/// <summary>Gets the target rooms.</summary>
	public Room Rooms { get; init; }
	/// <summary>Gets the duration.</summary>
	public float Duration { get; init; }
	/// <summary>Gets the X position (percent).</summary>
	public float X { get; init; }
	/// <summary>Gets the Y position (percent).</summary>
	public float Y { get; init; }
	/// <summary>Gets the color.</summary>
	public PaletteColorWithAlpha Color { get; init; }
	/// <summary>Gets the outline color.</summary>
	public PaletteColorWithAlpha OutlineColor { get; init; }
	/// <summary>Gets the size.</summary>
	public int Size { get; init; }
	/// <summary>Gets the rotation angle in degrees.</summary>
	public float Angle { get; init; }
	/// <summary>Gets the anchor / alignment.</summary>
	public FloatingTextAnchorStyle Anchor { get; init; }
	/// <summary>Gets the fade-out mode.</summary>
	public FloatingTextFadeOutMode Mode { get; init; }
	/// <summary>Gets the floating-text id (used to match <c>AdvanceText</c>); -1 for other text.</summary>
	public int Id { get; init; }
}

/// <summary>
/// The state of one floating text.
/// </summary>
public readonly struct FloatingTextSnapshot
{
	/// <summary>Gets the initial floating text command.</summary>
	public TextCommand Command { get; init; }
	/// <summary>Gets how many times the text has been advanced.</summary>
	public int Advance { get; init; }
	/// <summary>Gets the currently displayed section.</summary>
	public string CurrentText { get; init; }
	/// <summary>Gets when the current section was shown (the event or the last <c>AdvanceText</c>), so the fade restarts each time.</summary>
	public TickTime ChangedAt { get; init; }
}

/// <summary>
/// The text / UI state.
/// </summary>
public readonly struct TextSnapshot
{
	/// <summary>Gets the text commands seen so far.</summary>
	public IReadOnlyList<TextCommand> Commands { get; init; }
	/// <summary>Gets the most recent dialogue, when set.</summary>
	public string? Dialogue { get; init; }
	/// <summary>Gets the most recent status sign, when set.</summary>
	public string? Status { get; init; }
	/// <summary>Gets how many times each floating text (by id) has been advanced.</summary>
	public IReadOnlyDictionary<int, int> FloatingTextAdvances { get; init; }
}

/// <summary>
/// Records floating text, dialogue, status signs and narration.
/// </summary>
internal sealed class TextModule : IPlayheadObject
{
	private readonly List<TextCommand> _commands = [];
	private readonly List<(int Id, string[] Sections, TextCommand Command)> _floatingTexts = [];
	private readonly Dictionary<int, int> _textAdvances = [];
	private readonly Dictionary<int, List<TickTime>> _textAdvanceTimes = [];
	private string? _dialogue;
	private string? _status;

	internal TextModule(Playhead playhead)
	{
		Playhead = playhead;
	}

	private TextModule(TextModule source)
	{
		Playhead = source.Playhead;
		_commands = [.. source._commands];
		_floatingTexts = [.. source._floatingTexts];
		_textAdvances = new Dictionary<int, int>(source._textAdvances);
		foreach (var pair in source._textAdvanceTimes)
			_textAdvanceTimes[pair.Key] = new List<TickTime>(pair.Value);
		_dialogue = source._dialogue;
		_status = source._status;
	}

	public Playhead Playhead { get; private set; }

	public void Rebind(Playhead playhead) => Playhead = playhead;

	public PlayheadModules Module => PlayheadModules.Text;

	public string Id => "text";

	public PlayheadObjectKind Kind => PlayheadObjectKind.Text;

	public IReadOnlyList<PlayheadObjectSnapshot> Describe()
	{
		List<PlayheadObjectSnapshot> objects = [];
		for (int i = 0; i < _floatingTexts.Count; i++)
		{
			(int id, string[] sections, TextCommand command) = _floatingTexts[i];
			int advance = id >= 0 && _textAdvances.TryGetValue(id, out int count) ? count : 0;
			int index = sections.Length > 0 ? Math.Min(advance, sections.Length - 1) : 0;
			string current = sections.Length > 0 ? sections[index] : command.Text;
			// The fade restarts whenever a new section is revealed (LyricsGame.AdvanceText).
			TickTime changedAt = command.TickTime;
			if (index > 0 && id >= 0 && _textAdvanceTimes.TryGetValue(id, out List<TickTime>? times) && times.Count >= index)
				changedAt = times[index - 1];
			objects.Add(new PlayheadObjectSnapshot
			{
				Id = id >= 0 ? "floating" + id : "floating" + i,
				Kind = PlayheadObjectKind.FloatingText,
				State = new FloatingTextSnapshot { Command = command, Advance = advance, CurrentText = current, ChangedAt = changedAt },
			});
		}
		if (_dialogue is not null)
			objects.Add(new PlayheadObjectSnapshot { Id = "dialogue", Kind = PlayheadObjectKind.Dialogue, State = _dialogue });
		if (_status is not null)
			objects.Add(new PlayheadObjectSnapshot { Id = "status", Kind = PlayheadObjectKind.StatusSign, State = _status });
		return objects;
	}

	public ReadOnlyEnumCollection<EventType> RecordingTypes { get; } =
	[
		EventType.FloatingText,
		EventType.ShowDialogue,
		EventType.ShowStatusSign,
		EventType.ReadNarration,
		EventType.AdvanceText,
	];

	public void MoveNext(IBaseEvent e)
	{
		switch (e)
		{
			case FloatingText floating:
				string floatingText = Playhead.Interpolate(floating.Text);
				TextCommand command = new()
				{
					TickTime = e.TickTime,
					Type = e.Type,
					Text = floatingText,
					Rooms = floating.Rooms,
					Duration = floating.Duration,
					X = floating.Position.X ?? 0f,
					Y = floating.Position.Y ?? 0f,
					Color = floating.Color,
					OutlineColor = floating.OutlineColor,
					Size = floating.Size,
					Angle = floating.Angle,
					Anchor = floating.Anchor,
					Mode = floating.Mode,
					Id = floating.Id,
				};
				_commands.Add(command);
				_floatingTexts.Add((floating.Id, SplitSections(floatingText), command));
				break;
			case ShowDialogue dialogue:
				_dialogue = Playhead.Interpolate(dialogue.Text);
				_commands.Add(new TextCommand { TickTime = e.TickTime, Type = e.Type, Text = _dialogue, Rooms = dialogue.Rooms });
				break;
			case ShowStatusSign status:
				_status = Playhead.Interpolate(status.Text);
				_commands.Add(new TextCommand { TickTime = e.TickTime, Type = e.Type, Text = _status, Duration = status.Duration });
				break;
			case ReadNarration narration:
				_commands.Add(new TextCommand { TickTime = e.TickTime, Type = e.Type, Text = Playhead.Interpolate(narration.Text) });
				break;
			case AdvanceText advance:
				if (advance.Header is { } header)
				{
					int count = _textAdvances.TryGetValue(header.Id, out int current) ? current : 0;
					_textAdvances[header.Id] = count + 1;
					if (!_textAdvanceTimes.TryGetValue(header.Id, out List<TickTime>? times))
						_textAdvanceTimes[header.Id] = times = [];
					times.Add(e.TickTime);
				}
				break;
		}
	}

	// Cumulative reveal sections, matching SpecificEventExtensions.SplitText: '/'-separated
	// pieces accumulate so advancing reveals more text.
	private static string[] SplitSections(string text)
	{
		if (string.IsNullOrEmpty(text))
			return [];
		List<string> sections = [];
		StringBuilder builder = new();
		for (int i = 0; i < text.Length; i++)
		{
			char c = text[i];
			if (c == '/')
			{
				sections.Add(builder.ToString());
			}
			else if (c == '\\' && i + 1 < text.Length)
			{
				char next = text[++i];
				builder.Append(next == 'n' ? '\n' : next == '/' ? '/' : next);
			}
			else
			{
				builder.Append(c);
			}
		}
		sections.Add(builder.ToString());
		return [.. sections];
	}

	public object Snapshot() => new TextSnapshot
	{
		Commands = _commands,
		Dialogue = _dialogue,
		Status = _status,
		FloatingTextAdvances = new Dictionary<int, int>(_textAdvances),
	};

	public IPlayheadObject Clone(Playhead playhead) => new TextModule(this) { Playhead = playhead };
}


