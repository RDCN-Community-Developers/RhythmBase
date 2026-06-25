using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Events;
using RhythmBase.RhythmDoctor.Extensions;

namespace RhythmBase.RhythmDoctor.Utils;

/// <summary>
/// Describes how beat data should be preserved or shifted when crotchets-per-bar (CPB) changes occur.
/// </summary>
[Flags]
public enum BeatChangeStrategy : byte
{
	/// <summary>
	/// Uses time signature replacement strategy to correct incomplete bar.
	/// Events maintain their absolute beat position in the global timeline.
	/// </summary>
	GlobalBeatCorrection = 0b00, // 使用拍号替换策略，修正非完整拍号

	/// <summary>
	/// Preserves the relative beat position of events within their own bar.
	/// Does not apply time signature replacement strategy.
	/// </summary>
	PreserveBarRelativePosition = 0b01, // 不使用拍号替换策略，保持变动部分小节内事件相对于自身所在小节的节拍不动

	/// <summary>
	/// Maintains relative beat position of events after the change range,
	/// anchored to the end of the change range.
	/// </summary>
	PreservePostChangePosition = 0b10, // 让变动范围之后的事件保持相对于变动范围末尾的节拍不动

	/// <summary>
	/// Default engine behavior, equivalent to <see cref="GlobalBeatCorrection"/>.
	/// </summary>
	Default = 0b00,

	/// <summary>
	/// Rhythm Doctor Level Editor behavior.
	/// </summary>
	RDLE = 0b11,
}

internal record struct CpbCache(float Tick, int Bar, int Cpb) : IComparable<CpbCache>
{
	public static readonly CpbCache Default = new(1, 1, DefaultCpb);
	public readonly int CompareTo(CpbCache other) => Tick.CompareTo(other.Tick);
};
partial class BeatCalculator
{
	private CpbCache[] _cpbCache = [];
	internal bool AddCpbAt(CpbCache cpb, byte strategy, out CpbCache fix) // 返回值表示是否需要插入一个新的 cpb 来修正节拍位置，fix 是需要插入的 cpb
	{
		bool moveTrival = (strategy & 0b10) == 0; // 是否需要移动变动部分小节内相对于事件自身所在小节的节拍不动的事件
		fix = CpbCache.Default;
		if (_cpbCache.Length == 0)
		{
			// 如果 cpb 的值不是默认值 (8), 需要迁移后续事件
			if ((strategy & 0b01) != 0) MoveEvents(8, cpb, null, moveTrival);
			_cpbCache = [cpb]; // 唯一的 cpb
			return false;
		}
		int index = _cpbCache.BinarySearch(cpb);
		if (index < 0) index = ~index - 1;

		CpbCache a, b;
		if (index < 0) a = CpbCache.Default;
		else a = _cpbCache[index];
		if (index >= _cpbCache.Length - 1) // cpb 被添加到最后
		{
			if ((strategy & 0b01) != 0) MoveEvents(a.Cpb, cpb, null, moveTrival);
			_cpbCache = [.. _cpbCache, cpb];
			return false;
		}
		else // cpb 被添加到中间
		{
			b = _cpbCache[index + 1]; // 下一个 cpb
			if (a.Tick == cpb.Tick && a.Cpb == cpb.Cpb) // 已经有一个完全相同的 cpb 了
				return false;
			bool needInsert = true;
			if (cpb.Tick == a.Tick) // 如果 cpb 的位置和前一个 cpb 相同但值不同，则覆盖前一个 cpb
			{
				_cpbCache[index] = cpb;
				needInsert = false;
			}
			if ((strategy & 0b01) == 0) // 保持范围内相对于第一个小节的节拍不动
			{
				(int barDiff, int diff) = int.DivRem((int)(b.Tick - cpb.Tick), cpb.Cpb); // 新的小节个数, 需要修正的节拍长度
				if (diff == 0) // 不需要修正节拍长度，直接移动后续 cpb 的位置
				{
					if (needInsert)
						_cpbCache = [.. _cpbCache.Take(index + 1), cpb, .. _cpbCache.Skip(index + 1).Select(c => c with { Bar = c.Bar - (b.Bar - cpb.Bar) + barDiff })];
					else
						_cpbCache = [.. _cpbCache.Take(index + 1), .. _cpbCache.Skip(index + 1).Select(c => c with { Bar = c.Bar - (b.Bar - cpb.Bar) + barDiff })];
					return false;
				}
				else // 需要修正节拍长度，在 cpb 和下一个 cpb 之间插入一个新的 cpb 来修正节拍位置
				{
					fix = new(
							cpb.Tick + barDiff * cpb.Cpb,
							cpb.Bar + barDiff,
							diff
							);
					barDiff += 1;
					if (needInsert)
						_cpbCache = [.. _cpbCache.Take(index + 1), cpb, fix, .. _cpbCache.Skip(index + 1).Select(c => c with { Bar = c.Bar - (b.Bar - cpb.Bar) + barDiff })];
					else
						_cpbCache = [.. _cpbCache.Take(index + 1), fix, .. _cpbCache.Skip(index + 1).Select(c => c with { Bar = c.Bar - (b.Bar - cpb.Bar) + barDiff })];
					return true;
				}
			}
			else // 保持小节内相对于自身所在小节的节拍不动
			{

				int dbeatPerBar = cpb.Cpb - a.Cpb; // 每小节节拍数的变化量
				int barCount = b.Bar - cpb.Bar; // 需要迁移的事件跨越的小节数
				int dbeat = dbeatPerBar * barCount; // 需要迁移的节拍数
				MoveEvents(a.Cpb, cpb, b, moveTrival);
				if (needInsert)
					_cpbCache = [.. _cpbCache.Take(index + 1), cpb, .. _cpbCache.Skip(index + 1).Select(c => c with { Tick = c.Tick + dbeat })];
				else
					_cpbCache = [.. _cpbCache.Take(index + 1), .. _cpbCache.Skip(index + 1).Select(c => c with { Tick = c.Tick + dbeat })];
				return false;
			}
		}
	}

	internal bool RemoveCpbAt(CpbCache cpb, byte strategy, out CpbCache fix)
	{
		bool moveTrival = (strategy & 0b10) == 0;
		fix = CpbCache.Default;
		if (_cpbCache.Length == 0)
			return false;
		int index = _cpbCache.BinarySearch(cpb);
		if (index < 0) return false;

		CpbCache a, b;
		if (index == 0) a = CpbCache.Default;
		else a = _cpbCache[index - 1];
		if (index == _cpbCache.Length - 1)
		{
			if ((strategy & 0b01) != 0) MoveEvents(cpb.Cpb, cpb with { Cpb = a.Cpb }, null, moveTrival);
			_cpbCache = [.. _cpbCache[..(_cpbCache.Length - 1)]];
			return false;
		}
		else
		{
			b = _cpbCache[index + 1];
			int lenac = (int)(cpb.Tick - a.Tick);
			int lencb = (int)(b.Tick - cpb.Tick);
			if (a.Cpb == cpb.Cpb)
			{
				_cpbCache = [.. _cpbCache[..index], .. _cpbCache[(index + 1)..]];
				return false;
			}
			if ((strategy & 0b01) == 0)
			{
				int diff = (int)(b.Tick - cpb.Tick) % a.Cpb;
				int barDiff = (int)((b.Tick - cpb.Tick) / a.Cpb);
				if (diff == 0)
				{

					_cpbCache = [.. _cpbCache[..index], .. _cpbCache[(index + 1)..].Select(c => c with { Bar = c.Bar - (b.Bar - a.Bar) + barDiff })];
					return false;
				}
				else
				{
					fix = new(
							cpb.Tick + barDiff * a.Cpb,
							cpb.Bar + barDiff,
							diff
							);
					barDiff += 1;
					_cpbCache = [.. _cpbCache[..index], fix, .. _cpbCache[(index + 1)..].Select(c => c with { Bar = c.Bar - (b.Bar - a.Bar) + barDiff })];
					return true;
				}
			}
			else
			{
				int dbeatPerBar = a.Cpb - cpb.Cpb;
				int barCount = b.Bar - cpb.Bar;
				int dbeat = dbeatPerBar * barCount;
				MoveEvents(cpb.Cpb, cpb with { Cpb = a.Cpb }, b, moveTrival);
				_cpbCache = [.. _cpbCache[..index], .. _cpbCache[(index + 1)..].Select(c => c with { Tick = c.Tick + dbeat })];
				return false;
			}
		}
	}

	private void MoveEvents(int previousCpb,
													CpbCache target,
													CpbCache? nextCpbBeat,
													bool moveTrival)
	{
		RedBlackTree<TickTime, TypedEventCollection> allEvents = Collection.EventsBeatOrder;
		OrderedCollection<TickTime, Bookmark> allBookmarks = Collection.Bookmarks;
		int diffBeatPerBar = target.Cpb - previousCpb; // 每小节节拍数的变化量
		float st =
				moveTrival && nextCpbBeat is CpbCache nextCpbNotNull1
				? nextCpbNotNull1.Tick
				: target.Tick + int.Min(previousCpb, target.Cpb); // 需要迁移的组件的起始位置
		KeyValuePair<TickTime, TypedEventCollection>[] nodes = [.. allEvents.Where(e => e.Key.Tick >= st)];
		foreach (var node in nodes)
		{
			(int bar, _) = node.Key;
			int offset = (
					(nextCpbBeat is CpbCache nextCpbNotNull2 && bar > nextCpbNotNull2.Bar)
					? (nextCpbNotNull2.Bar - target.Bar)
					: (bar - target.Bar)
					) * diffBeatPerBar; // 这个小节需要迁移的节拍数
			allEvents.Remove(node);
			TickTime newBeat = node.Key + offset;
			foreach (IBaseEvent e in node.Value)
			{
				BaseEvent _e = (e as BaseEvent)!;
				_e._beat = newBeat;
			}
			if (offset < 0 && // 只有 cpb 减少时才会出现事件重叠的情况，且是向前重叠，和遍历方向一致
					allEvents.ContainsKey(newBeat)) // 如果目标位置有事件就合并
			{
				TypedEventCollection existing = allEvents[newBeat];
				foreach (IBaseEvent e in node.Value)
					existing.Add(e);
			}
			else
				allEvents.Insert(newBeat, node.Value);
		}
		Bookmark[] bookmarks = [.. allBookmarks.Where(b => b.Tick.Tick >= st)];
		foreach (Bookmark bookmark in bookmarks)
		{
			(int bar, _) = bookmark.Tick;
			int offset = (
					(nextCpbBeat is CpbCache nextCpbNotNull2 && bar > nextCpbNotNull2.Bar)
					? (nextCpbNotNull2.Bar - target.Bar)
					: (bar - target.Bar)
					) * diffBeatPerBar; // 这个小节需要迁移的节拍数
			allBookmarks.Remove(bookmark);
			TickTime newBeat = bookmark.Tick + offset;
			allBookmarks.Add(bookmark with { Tick = newBeat });
		}
	}
	public partial void Refresh() // 潜在问题：没有处理两个不同值的 bpm/cpb 在同一位置的情况
	{
		SetCrotchetsPerBar[] cpbList = [.. Collection.OfEvent<SetCrotchetsPerBar>()];
		BaseBeatsPerMinute[] bpmList = [.. Collection.OfEvent<BaseBeatsPerMinute>()];
		_cpbCache = new CpbCache[cpbList.Length];
		_bpmCache = new BpmCache[bpmList.Length];
		for (int i = 0; i < cpbList.Length; i++)
		{
			(int bar, _) = cpbList[i].TickTime;
			_cpbCache[i] = new CpbCache(cpbList[i].TickTime.Tick, bar, cpbList[i].CrotchetsPerBar);
		}
		for (int i = 0; i < bpmList.Length; i++)
		{
			_bpmCache[i] = new BpmCache(bpmList[i].TickTime.Tick, bpmList[i].TickTime.TimeSpan, bpmList[i].BeatsPerMinute);
		}
	}

	/// <summary>
	/// Converts a bar/beat pair to its absolute beat position using the current cache state.
	/// </summary>
	/// <param name="bar">The one-based bar index.</param>
	/// <param name="beat">The one-based beat within the bar.</param>
	/// <returns>The absolute beat position.</returns>
	public float BarBeatToTick(int bar, float beat) => BarBeatToTick(bar, beat, in _cpbCache);

	/// <summary>
	/// Converts a bar/beat pair to an absolute timespan using the current cache state.
	/// </summary>
	/// <param name="bar">The one-based bar index.</param>
	/// <param name="beat">The one-based beat within the bar.</param>
	/// <returns>The absolute time represented by the beat.</returns>
	public TimeSpan BarBeatToTimeSpan(int bar, float beat) => TickToTimeSpan(BarBeatToTick(bar, beat));

	/// <summary>
	/// Converts an absolute beat position to its bar/beat representation using the current cache state.
	/// </summary>
	/// <param name="beat">The absolute beat position.</param>
	/// <returns>The one-based bar index and the one-based beat within that bar.</returns>
	public (int bar, float beat) TickToBarBeat(float beat) => TickToBarBeat(beat, in _cpbCache);
	/// <summary>
	/// Converts an absolute timespan to its bar/beat representation using the current cache state.
	/// </summary>
	/// <param name="timeSpan">The absolute time to convert.</param>
	/// <returns>The one-based bar index and the one-based beat within that bar.</returns>
	public (int bar, float beat) TimeSpanToBarBeat(TimeSpan timeSpan) => TickToBarBeat(TimeSpanToTick(timeSpan));
	private static float BarBeatToTick(int bar, float beat, in CpbCache[] cacheSet)
	{
		CpbCache last = CpbCache.Default;
		foreach (var cache in cacheSet)
		{
			int cbar = cache.Bar;
			if (cbar < bar)
			{
				last = cache;
				continue;
			}
			if (cbar == bar)
			{
				float tick = cache.Tick + beat - 1f;
				return tick;
			}
			break;
		}
		float finalTick = last.Tick + (bar - last.Bar) * last.Cpb + beat - 1;
		return finalTick;
	}
	private static (int bar, float beat) TickToBarBeat(float beat, in CpbCache[] cacheSet)
	{
		CpbCache last = CpbCache.Default;
		foreach (CpbCache cache in cacheSet)
		{
			float cbeat = cache.Tick;
			if (cbeat < beat)
			{
				last = cache;
				continue;
			}
			if (cbeat == beat)
				return (cache.Bar, 1f);
			break;
		}
		(int finalBar, float finalBeat) result2 = ((int)Math.Round(last.Bar + Math.Floor((double)((beat - last.Tick) / last.Cpb))), (beat - last.Tick) % last.Cpb + 1f);
		return result2;
	}
	/// <summary>
	/// Creates an <see cref="TickTime"/> from a bar/beat pair.
	/// </summary>
	/// <param name="bar">The one-based bar index.</param>
	/// <param name="beat">The one-based beat within the bar.</param>
	/// <returns>An <see cref="TickTime"/> bound to this calculator.</returns>
	public TickTime TickOf(int bar, float beat) => new(this, bar, beat);

	/// <summary>
	/// Creates a <see cref="TickTimeRange"/> representing the interval between two bar/beat pairs.
	/// </summary>
	/// <param name="beat1">The starting bar/beat tuple.</param>
	/// <param name="beat2">The ending bar/beat tuple.</param>
	/// <returns>The resulting interval.</returns>
	public TickTimeRange IntervalOf((int bar, float beat) beat1, (int bar, float beat) beat2) => IntervalOf(TickOf(beat1.bar, beat1.beat), TickOf(beat2.bar, beat2.beat));

	/// <summary>
	/// Gets the CPB in effect at the specified beat.
	/// </summary>
	/// <param name="beat">The beat whose CPB should be retrieved.</param>
	/// <returns>The CPB active at the beat.</returns>
	public int CrotchetsPerBarOf(TickTime beat)
	{
		CpbCache last = CpbCache.Default;
		foreach (CpbCache cache in _cpbCache)
		{
			float cbeat = cache.Tick;
			if (cbeat < beat.Tick)
			{
				last = cache;
				continue;
			}
			if (cbeat == beat.Tick)
				return cache.Cpb;
			break;
		}
		return last.Cpb;
	}
}