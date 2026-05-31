using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Converters;
namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents the base class for events that have a beats per minute (BPM) value.
/// </summary>
[JsonObjectHasSerializer(typeof(RDMemberConverter.BaseBeatsPerMinute<>))]
public abstract record class BaseBeatsPerMinute : BaseEvent
{
	///<inheritdoc/>
	public override TickTime TickTime
	{
		get => base.TickTime;
		set
		{
			base.TickTime = value;
			_ = base.TickTime._calculator;
			ResetTimeLine();
		}
	}
	///<inheritdoc/>
	public virtual float BeatsPerMinute
	{
		get => _bpm;
		set
		{
			_bpm = value;
			ResetTimeLine();
		}
	}
	/// <summary>
	/// Resets the timeline for all events in the same level that occur after this event.
	/// </summary>
	private void ResetTimeLine()
	{
		if (TickTime.BaseLevel != null)
		{
			foreach (IBaseEvent item in from i in TickTime.BaseLevel
										where i.TickTime > TickTime
										select i)
			{
				((BaseEvent)item)._beat.ResetBPM();
			}
		}
	}
	private float _bpm = DefaultBpm;
}
