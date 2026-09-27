using RhythmBase.Global.Components.Vector;
using RhythmBase.RhythmDoctor.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace RhythmBase.RhythmDoctor.Components;


internal interface IPlayheadModuleSnapshot
{
	TickTime TickTime { get; init; }
}
internal interface IPlayheadModule<TSnapshot> where TSnapshot : struct, IPlayheadModuleSnapshot
{
	ReadOnlyEnumCollection<EventType> RecordingTypes { get; }
	Playhead Playhead { get; }

	TSnapshot Snapshot();
	void MoveNext(IBaseEvent e);
}
internal struct TweenValue
{
	public float Start { get; }
	public float End { get; }
	public float Source { get; }
	public float Target { get; }
	public float Duration => End - Start;
	public float GetValue(float tick)
	{
		return tick > End ? Target : Source + (Target - Source) * (tick - Start) / (End - Start);
	}
	public TweenValue(float start, float end, float source, float target)
	{
		Start = start;
		End = end;
		Source = source;
		Target = target;
	}
	public TweenValue(float value) : this(0, 0, value, value) { }
}
internal class DecorationModule
{
	internal readonly struct TransformationSnapshot : IPlayheadModuleSnapshot
	{
		public TickTime TickTime { get; init; }
		public RotatedRectN Transformation { get; init; }
	}
	internal readonly struct VisibilitySnapshot : IPlayheadModuleSnapshot
	{
		public TickTime TickTime { get; init; }
		public bool Visible { get; init; }
	}
	internal class Visibility : IPlayheadModule<VisibilitySnapshot>
	{
		public ReadOnlyEnumCollection<EventType> RecordingTypes { get; } = [
			EventType.SetVisible,
		];
		private bool _visible;
		public Playhead Playhead { get; }
		public void MoveNext(IBaseEvent e)
		{
			if (e is SetVisible visible)
				_visible = visible.Visible;
		}
		public VisibilitySnapshot Snapshot() => new VisibilitySnapshot { TickTime = Playhead.CurrentTickTime, Visible = _visible };
	}
	internal class Transformation : IPlayheadModule<TransformationSnapshot>
	{
		// position
		private TweenValue _px = new(50);
		private TweenValue _py = new(50);
		// scale
		private TweenValue _w = new(100);
		private TweenValue _h = new(100);
		// pivot
		private TweenValue _ox = new(50);
		private TweenValue _oy = new(50);
		// angle
		private TweenValue _a = new(0);

		public Playhead Playhead { get; }
		public Transformation(Playhead playhead)
		{
			Playhead = playhead;
		}
		public ReadOnlyEnumCollection<EventType> RecordingTypes => [
			EventType.Move,
		];
		public void MoveNext(IBaseEvent e)
		{
			if (e is Move move)
			{
				TickTime t = e.TickTime;
				if (move.Position is PointE p)
				{
					if (p.X is Expression px)
						_px = new TweenValue(t.Tick, t.Tick + move.Duration, _px.GetValue(t.Tick), px.NumericValue);
					if (p.Y is Expression py)
						_py = new TweenValue(t.Tick, t.Tick + move.Duration, _py.GetValue(t.Tick), py.NumericValue);
				}
				if (move.Scale is SizeE s)
				{
					if (s.Width is Expression w)
						_w = new TweenValue(t.Tick, t.Tick + move.Duration, _w.GetValue(t.Tick), w.NumericValue);
					if (s.Height is Expression h)
						_h = new TweenValue(t.Tick, t.Tick + move.Duration, _h.GetValue(t.Tick), h.NumericValue);
				}
				if (move.Pivot is Point o)
				{
					if (o.X is float ox)
						_ox = new TweenValue(t.Tick, t.Tick + move.Duration, _ox.GetValue(t.Tick), ox);
					if (o.Y is float oy)
						_oy = new TweenValue(t.Tick, t.Tick + move.Duration, _oy.GetValue(t.Tick), oy);
				}
				if (move.Angle is Expression a)
					_a = new TweenValue(t.Tick, t.Tick + move.Duration, _a.GetValue(t.Tick), a.NumericValue);
			}
		}
		public TransformationSnapshot Snapshot()
		{
			return new TransformationSnapshot
			{
				TickTime = Playhead.CurrentTickTime,
				Transformation = new RotatedRectN(
					new(_px.GetValue(Playhead.CurrentTickTime.Tick), _py.GetValue(Playhead.CurrentTickTime.Tick)),
					new(_w.GetValue(Playhead.CurrentTickTime.Tick), _h.GetValue(Playhead.CurrentTickTime.Tick)),
					new(_ox.GetValue(Playhead.CurrentTickTime.Tick), _oy.GetValue(Playhead.CurrentTickTime.Tick)),
					_a.GetValue(Playhead.CurrentTickTime.Tick)
				)
			};
		}
	}

}
internal interface IRandomSimulator
{
	public int Next(int maxValue);
}
internal class RandomSimulator : IRandomSimulator
{
	private readonly Random _random;
	public RandomSimulator(Random random)
	{
		_random = random;
	}
	public int Next(int maxValue)
	{
		return _random.Next(maxValue);
	}
}

internal class Playhead
{
	public class PlayheadConfig
	{

	}
	public readonly struct PlayheadState
	{
		public TickTime TickTime { get; internal init; }
		public Chart Chart { get; internal init; }
	}
	public TickTime CurrentTickTime { get; private set; }
	public Chart CurrentChart { get; private set; }
	public Playhead(Level level)
	{

	}
	public void Forward(TimeSpan timeSpan)
	{
	}
	public void Forward(float tick)
	{
	}
	public void ForwardTo(TickTime tickTime)
	{
	}
}