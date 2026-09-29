using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Serialization;
using RhythmBase.RhythmDoctor.Utils;
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
	/// <inheritdoc/>
	public override bool Active
	{
		get => base.Active;
		set
		{
			if(!_tick.IsEmpty)
			{
				OrderedEventCollection<IBaseEvent> b = _tick.BaseChart;
				if (value && !base.Active)
				{
					_tick._calculator.AddBpmAt(new BpmCache(_tick.Tick, _tick.TimeSpan, BeatsPerMinute));
					b.Add(this);
				}
				else if (!value && base.Active)
				{
					_tick._calculator.RemoveBpmAt(new BpmCache(_tick.Tick, _tick.TimeSpan, BeatsPerMinute));
					bool result = b.Remove(this);
					_tick = _tick.WithoutLink();
				}
			}
			base.Active = value;
		}
	}
	///<inheritdoc/>
	public virtual float BeatsPerMinute
	{
		get => _bpm;
		set
		{
			if (_bpm == value) return;
			Chart? chart = TickTime.BaseChart;
			if (chart is not null && Active)
			{
				// 通过既有的删除/新增流程让 BeatCalculator 的 _bpmCache 同步为新的速度值
				chart.Remove(this);
				_bpm = value;
				chart.Add(this);
			}
			else
			{
				_bpm = value;
			}
			ResetTimeLine();
		}
	}
	/// <summary>
	/// Resets the timeline for all events in the same level that occur after this event.
	/// </summary>
	private void ResetTimeLine()
	{
		if (TickTime.BaseChart != null)
		{
			foreach (IBaseEvent item in from i in TickTime.BaseChart
										where i.TickTime > TickTime
										select i)
			{
				((BaseEvent)item)._tick.ResetBPM();
			}
		}
	}
	private float _bpm = DefaultBpm;
}
