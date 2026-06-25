using RhythmBase.BeatBlock.Events;

namespace RhythmBase.BeatBlock.Components;

/// <summary>
/// Provides methods for calculating beats and time spans in a <see cref="Level"/>.
/// </summary>
public partial class BeatCalculator
{
	/// <summary>
	/// Refreshes the calculator state.
	/// </summary>
	public partial void Refresh()
	{
		SetBeatsPerMinute[] bpmList = [.. Collection.OfType<SetBeatsPerMinute>()];
		_bpmCache = new BpmCache[bpmList.Length];
		for (int i = 0; i < bpmList.Length; i++)
		{
			_bpmCache[i] = new BpmCache(bpmList[i].TickTime.Tick, bpmList[i].TickTime.TimeSpan, bpmList[i].Bpm);
		}
	}
}
