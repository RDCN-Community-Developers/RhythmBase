using System.Text;
using RhythmBase.Global.Components;
using RhythmBase.RhythmDoctor.Components.Lang;
using RhythmBase.RhythmDoctor.Events;
using RhythmBase.RhythmDoctor.Utils;

namespace RhythmBase.RhythmDoctor.Components;

public sealed partial class Playhead
{

	private void HandleTagAction(TagAction tagAction)
	{
		if (_tagDepth >= 64)
			return;
		_tagDepth++;
		try
		{
			string name = tagAction.ActionTag;
			switch (tagAction.Action)
			{
				case ActionTagAction.Run:
					RunTagged(name, exact: true, random: false);
					break;
				case ActionTagAction.RunAll:
					RunTagged(name, exact: false, random: false);
					break;
				case ActionTagAction.RunRandom:
					RunTagged(name, exact: true, random: true);
					break;
				case ActionTagAction.Enable:
					SetTagEnabled(name, exact: true, enabled: true);
					break;
				case ActionTagAction.Disable:
					SetTagEnabled(name, exact: true, enabled: false);
					break;
				case ActionTagAction.EnableAll:
					SetTagEnabled(name, exact: false, enabled: true);
					break;
				case ActionTagAction.DisableAll:
					SetTagEnabled(name, exact: false, enabled: false);
					break;
			}
		}
		finally
		{
			_tagDepth--;
		}
	}

	private void RunTagged(string name, bool exact, bool random)
	{
		List<int> matches = [];
		foreach ((int index, string tag) in _tagged)
		{
			if (_disabledTaggedEvents.Contains(index))
				continue;
			if (exact ? tag == name : tag.Contains(name))
				matches.Add(index);
		}
		if (matches.Count == 0)
			return;
		if (random)
		{
			RunTaggedAt(matches[_random.Next(0, matches.Count)]);
			return;
		}
		foreach (int index in matches)
			RunTaggedAt(index);
	}

	private void RunTaggedAt(int index)
	{
		// A tagged event with a condition duration is deferred, like the game's TimedAction.
		float delay = _scheduler.EventAt(index).Condition.Duration;
		if (delay > 0f)
		{
			_pendingTagged.Add((_currentTick + delay, index));
			_pendingTagged.Sort(static (a, b) => a.Tick.CompareTo(b.Tick));
			return;
		}
		ProcessEvent(_scheduler.EventAt(index), index, viaTag: true);
	}

	private void DrainPendingTagged(float upTo)
	{
		while (_pendingTagged.Count > 0 && _pendingTagged[0].Tick <= upTo)
		{
			int index = _pendingTagged[0].Index;
			_pendingTagged.RemoveAt(0);
			ProcessEvent(_scheduler.EventAt(index), index, viaTag: true);
		}
	}

	private void SetTagEnabled(string name, bool exact, bool enabled)
	{
		foreach ((int index, string tag) in _tagged)
		{
			if (!(exact ? tag == name : tag.Contains(name)))
				continue;
			if (enabled)
				_disabledTaggedEvents.Remove(index);
			else
				_disabledTaggedEvents.Add(index);
		}
	}
}
