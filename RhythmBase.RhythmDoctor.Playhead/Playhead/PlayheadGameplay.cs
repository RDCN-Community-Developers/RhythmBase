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
