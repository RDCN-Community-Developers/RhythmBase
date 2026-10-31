using System.Text;
using RhythmBase.Global.Components;
using RhythmBase.RhythmDoctor.Components.Lang;
using RhythmBase.RhythmDoctor.Events;
using RhythmBase.RhythmDoctor.Utils;

namespace RhythmBase.RhythmDoctor.Components;

public sealed partial class Playhead
{

	private PlayheadAdvanceResult AdvanceTo(float targetTick)
	{
		if (Config.EnforceReadOnly)
			VerifyUnchanged();

		if (targetTick <= _currentTick)
		{
			_currentTick = targetTick;
			_scheduler.SeekTo(targetTick);
			return new PlayheadAdvanceResult(PlayheadAdvanceStatus.Reached, CurrentTickTime, 0);
		}

		LoopGuard guard = Config.LoopGuard;
		float simulated = 0f;
		int steps = 0;
		int totalProcessed = 0;

		while (_currentTick < targetTick)
		{
			if (++steps > guard.MaxSteps || simulated > guard.MaxTicks)
				return new PlayheadAdvanceResult(PlayheadAdvanceStatus.GuardTriggered, CurrentTickTime, totalProcessed);

			(int bar, float beat) = _calculator.TickToBarBeat(_currentTick);
			float barEnd = _calculator.BarBeatToTick(bar + 1, 1);
			float nextEvent = _scheduler.NextTick;

			float boundary = targetTick;
			bool barBoundary = false;
			if (nextEvent < boundary)
				boundary = nextEvent;
			if (barEnd < boundary)
			{
				boundary = barEnd;
				barBoundary = true;
			}
			else if (barEnd == boundary)
			{
				barBoundary = true;
			}

			totalProcessed += barBoundary
				? _scheduler.ProcessBefore(barEnd, this)
				: _scheduler.ProcessUpTo(boundary, this);
			DrainPendingTagged(boundary);

			if (barBoundary)
			{
				int decision = _conductor.BeginBarBoundary(bar);
				if (decision == ConductorModule.BoundaryHold)
				{
					simulated += barEnd - _currentTick;
					_currentTick = barEnd;
					PlayheadAdvanceStatus holdStatus = targetTick > barEnd ? PlayheadAdvanceStatus.NotReached : PlayheadAdvanceStatus.Reached;
					return new PlayheadAdvanceResult(holdStatus, CurrentTickTime, totalProcessed);
				}
				if (decision == ConductorModule.BoundaryLoopBar)
				{
					float barStart = _calculator.BarBeatToTick(bar, 1);
					simulated += barEnd - barStart;
					_currentTick = barStart;
					_scheduler.SeekTo(barStart);
					continue;
				}
				if (decision == ConductorModule.BoundaryLoopBeat)
				{
					float beatStart = _calculator.BarBeatToTick(bar, (float)Math.Floor(beat));
					simulated += barEnd - beatStart;
					_currentTick = beatStart;
					_scheduler.SeekTo(beatStart);
					continue;
				}

				float targetBarStart = _calculator.BarBeatToTick(decision, 1);
				simulated += Math.Abs(targetBarStart - _currentTick);
				_currentTick = targetBarStart;
				_scheduler.SeekTo(targetBarStart);
				totalProcessed += _scheduler.ProcessUpTo(targetBarStart, this);
				continue;
			}

			simulated += boundary - _currentTick;
			_currentTick = boundary;
		}

		return new PlayheadAdvanceResult(PlayheadAdvanceStatus.Reached, CurrentTickTime, totalProcessed);
	}

	private void VerifyUnchanged()
	{
		int count = CurrentChart.Count;
		int version = CurrentChart.Settings.Version;
		bool changed = count != _lastEventCount || version != _creationVersion;
		if (!changed && Config.VerifyContentHash)
		{
			long fingerprint = ComputeFingerprint(CurrentChart);
			changed = fingerprint != _lastFingerprint;
			_lastFingerprint = fingerprint;
		}
		if (changed)
			throw new InvalidOperationException("The chart was modified after the playhead was created; playheads are read-only.");
		_lastEventCount = count;
	}

	private static long ComputeFingerprint(Chart chart)
	{
		unchecked
		{
			long hash = 1469598103934665603L;
			foreach (IBaseEvent e in chart)
			{
				Mix(ref hash, e.Type.ToString());
				Mix(ref hash, e.TickTime.Tick.ToString("R"));
				Mix(ref hash, e.Active ? "1" : "0");
				Mix(ref hash, e.Tag ?? "");
				Mix(ref hash, e.RunTag ? "1" : "0");
				Mix(ref hash, e.Condition.Serialize());
			}
			Mix(ref hash, chart.Decorations.Count.ToString());
			Mix(ref hash, chart.Rows.Count.ToString());
			return hash;
		}
	}

	private static void Mix(ref long hash, string text)
	{
		foreach (char c in text)
		{
			hash ^= c;
			hash *= 1099511628211L;
		}
		hash ^= '|';
		hash *= 1099511628211L;
	}
}
