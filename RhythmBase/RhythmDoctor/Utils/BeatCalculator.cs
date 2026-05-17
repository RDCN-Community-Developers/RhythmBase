using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Events;
using RhythmBase.RhythmDoctor.Extensions;

namespace RhythmBase.RhythmDoctor.Utils
{
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
    internal record struct BpmCache(float BeatOnly, TimeSpan TimeSpan, float Bpm) : IComparable<BpmCache>
    {
        public static readonly BpmCache Default = new(1, TimeSpan.Zero, Utils.DefaultBPM);
        public readonly int CompareTo(BpmCache other) => BeatOnly.CompareTo(other.BeatOnly);
    }
    internal record struct CpbCache(float BeatOnly, int Bar, int Cpb) : IComparable<CpbCache>
    {
        public static readonly CpbCache Default = new(1, 1, Utils.DefaultCPB);
        public readonly int CompareTo(CpbCache other) => BeatOnly.CompareTo(other.BeatOnly);
    };

    /// <summary>
    /// Beat calculator that converts between absolute beats, bars, and time spans, while reacting to tempo and CPB changes.
    /// </summary>
    public class BeatCalculator : IBeatCalculator<RDBeat>
    {
        internal readonly RDLevel Collection;
        private BpmCache[] _bpmCache = [];
        private CpbCache[] _cpbCache = [];
        internal BeatCalculator(RDLevel level)
        {
            Collection = level;
        }
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
                if (a.BeatOnly == cpb.BeatOnly && a.Cpb == cpb.Cpb) // 已经有一个完全相同的 cpb 了
                    return false;
                bool needInsert = true;
                if (cpb.BeatOnly == a.BeatOnly) // 如果 cpb 的位置和前一个 cpb 相同但值不同，则覆盖前一个 cpb
                {
                    _cpbCache[index] = cpb;
                    needInsert = false;
                }
                if ((strategy & 0b01) == 0) // 保持范围内相对于第一个小节的节拍不动
                {
                    (int barDiff, int diff) = int.DivRem((int)(b.BeatOnly - cpb.BeatOnly), cpb.Cpb); // 新的小节个数, 需要修正的节拍长度
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
                            cpb.BeatOnly + barDiff * cpb.Cpb,
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
                        _cpbCache = [.. _cpbCache.Take(index + 1), cpb, .. _cpbCache.Skip(index + 1).Select(c => c with { BeatOnly = c.BeatOnly + dbeat })];
                    else
                        _cpbCache = [.. _cpbCache.Take(index + 1), .. _cpbCache.Skip(index + 1).Select(c => c with { BeatOnly = c.BeatOnly + dbeat })];
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
                int lenac = (int)(cpb.BeatOnly - a.BeatOnly);
                int lencb = (int)(b.BeatOnly - cpb.BeatOnly);
                if (a.Cpb == cpb.Cpb)
                {
                    _cpbCache = [.. _cpbCache[..index], .. _cpbCache[(index + 1)..]];
                    return false;
                }
                if ((strategy & 0b01) == 0)
                {
                    int diff = (int)(b.BeatOnly - cpb.BeatOnly) % a.Cpb;
                    int barDiff = (int)((b.BeatOnly - cpb.BeatOnly) / a.Cpb);
                    if (diff == 0)
                    {

                        _cpbCache = [.. _cpbCache[..index], .. _cpbCache[(index + 1)..].Select(c => c with { Bar = c.Bar - (b.Bar - a.Bar) + barDiff })];
                        return false;
                    }
                    else
                    {
                        fix = new(
                            cpb.BeatOnly + barDiff * a.Cpb,
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
                    _cpbCache = [.. _cpbCache[..index], .. _cpbCache[(index + 1)..].Select(c => c with { BeatOnly = c.BeatOnly + dbeat })];
                    return false;
                }
            }
        }

        private void MoveEvents(int previousCpb,
                                CpbCache target,
                                CpbCache? nextCpbBeat,
                                bool moveTrival)
        {
            RedBlackTree<RDBeat, TypedEventCollection<EventType, RDBeat>> allEvents = Collection.eventsBeatOrder;
            OrderedCollection<RDBeat, Bookmark> allBookmarks = Collection.Bookmarks;
            int diffBeatPerBar = target.Cpb - previousCpb; // 每小节节拍数的变化量
            float st =
                moveTrival && nextCpbBeat is CpbCache nextCpbNotNull1
                ? nextCpbNotNull1.BeatOnly
                : target.BeatOnly + int.Min(previousCpb, target.Cpb); // 需要迁移的组件的起始位置
            KeyValuePair<RDBeat, TypedEventCollection<EventType, RDBeat>>[] nodes = [.. allEvents.Where(e => e.Key.BeatOnly >= st)];
            foreach (var node in nodes)
            {
                (int bar, _) = node.Key;
                int offset = (
                    (nextCpbBeat is CpbCache nextCpbNotNull2 && bar > nextCpbNotNull2.Bar)
                    ? (nextCpbNotNull2.Bar - target.Bar)
                    : (bar - target.Bar)
                    ) * diffBeatPerBar; // 这个小节需要迁移的节拍数
                allEvents.Remove(node);
                RDBeat newBeat = node.Key + offset;
                foreach (IBaseEvent e in node.Value)
                {
                    BaseEvent _e = (e as BaseEvent)!;
                    _e._beat = newBeat;
                }
                if (offset < 0 && // 只有 cpb 减少时才会出现事件重叠的情况，且是向前重叠，和遍历方向一致
                    allEvents.ContainsKey(newBeat)) // 如果目标位置有事件就合并
                {
                    TypedEventCollection<EventType, RDBeat> existing = allEvents[newBeat];
                    foreach (IBaseEvent e in node.Value)
                        existing.Add(e);
                }
                else
                    allEvents.Insert(newBeat, node.Value);
            }
            Bookmark[] bookmarks = [.. allBookmarks.Where(b => b.Beat.BeatOnly >= st)];
            foreach (Bookmark bookmark in bookmarks)
            {
                (int bar, _) = bookmark.Beat;
                int offset = (
                    (nextCpbBeat is CpbCache nextCpbNotNull2 && bar > nextCpbNotNull2.Bar)
                    ? (nextCpbNotNull2.Bar - target.Bar)
                    : (bar - target.Bar)
                    ) * diffBeatPerBar; // 这个小节需要迁移的节拍数
                allBookmarks.Remove(bookmark);
                RDBeat newBeat = bookmark.Beat + offset;
                allBookmarks.Add(bookmark with { Beat = newBeat });
            }
        }
        internal void AddBpmAt(BpmCache bpm)
        {
            if (_bpmCache.Length == 0)
            {
                _bpmCache = [bpm];
                return;
            }
            int index = _bpmCache.BinarySearch(bpm);
            if (index < 0) index = ~index - 1;

            BpmCache a, b;
            if (index < 0) a = BpmCache.Default;
            else a = _bpmCache[index];
            if (a.BeatOnly == bpm.BeatOnly && (a.Bpm == bpm.Bpm)) return;
            if (index >= _bpmCache.Length - 1)
            {
                _bpmCache = [.. _bpmCache, bpm];
                return;
            }
            else
            {
                b = _bpmCache[index + 1];
                TimeSpan diff = TimeSpan.FromMinutes((b.BeatOnly - bpm.BeatOnly) / bpm.Bpm) - (b.TimeSpan - bpm.TimeSpan);
                for (int i = index + 1; i < _bpmCache.Length; ++i)
                {
                    BpmCache ti = _bpmCache[i];
                    ti.TimeSpan += diff;
                    _bpmCache[i] = ti;
                }
                if (a.BeatOnly != bpm.BeatOnly)
                    _bpmCache = [.. _bpmCache[..(index + 1)], bpm, .. _bpmCache[(index + 1)..]];
                else if (index < 0)
                    _bpmCache = [bpm, .. _bpmCache];
                else
                    _bpmCache[index] = bpm;
                return;
            }
        }
        internal void RemoveBpmAt(BpmCache bpm)
        {
            if (_bpmCache.Length == 0)
                return;
            int index = _bpmCache.BinarySearch(bpm);
            if (index < 0) return;
            BpmCache a, b;
            if (index == 0) a = BpmCache.Default;
            else a = _bpmCache[index - 1];
            if (index == _bpmCache.Length - 1)
            {
                _bpmCache = [.. _bpmCache[..(_bpmCache.Length - 1)]];
                return;
            }
            else
            {
                b = _bpmCache[index + 1];
                TimeSpan diff = TimeSpan.FromMinutes((b.BeatOnly - bpm.BeatOnly) / a.Bpm) - (b.TimeSpan - bpm.TimeSpan);
                for (int i = index + 1; i < _bpmCache.Length; ++i)
                {
                    BpmCache ti = _bpmCache[i];
                    ti.TimeSpan += diff;
                    _bpmCache[i] = ti;
                }
                _bpmCache = [.. _bpmCache[..index], .. _bpmCache[(index + 1)..]];
                return;
            }
        }
        /// <summary>
        /// Refreshes the BPM and CPB caches so subsequent conversions use up-to-date values.
        /// </summary>
        public void Refresh()
        {
            SetCrotchetsPerBar[] cpbList = [.. Collection.OfEvent<SetCrotchetsPerBar>()];
            BaseBeatsPerMinute[] bpmList = [.. Collection.OfEvent<BaseBeatsPerMinute>()];
            _cpbCache = new CpbCache[cpbList.Length];
            _bpmCache = new BpmCache[bpmList.Length];
            for (int i = 0; i < cpbList.Length; i++)
            {
                (int bar, _) = cpbList[i].Beat;
                _cpbCache[i] = new CpbCache(cpbList[i].Beat.BeatOnly, bar, cpbList[i].CrotchetsPerBar);
            }
            for (int i = 0; i < bpmList.Length; i++)
            {
                _bpmCache[i] = new BpmCache(bpmList[i].Beat.BeatOnly, bpmList[i].Beat.TimeSpan, bpmList[i].BeatsPerMinute);
            }
        }

        /// <summary>
        /// Converts a bar/beat pair to its absolute beat position using the current cache state.
        /// </summary>
        /// <param name="bar">The one-based bar index.</param>
        /// <param name="beat">The one-based beat within the bar.</param>
        /// <returns>The absolute beat position.</returns>
        public float BarBeatToBeatOnly(int bar, float beat) => BarBeatToBeatOnly(bar, beat, in _cpbCache);

        /// <summary>
        /// Converts a bar/beat pair to an absolute timespan using the current cache state.
        /// </summary>
        /// <param name="bar">The one-based bar index.</param>
        /// <param name="beat">The one-based beat within the bar.</param>
        /// <returns>The absolute time represented by the beat.</returns>
        public TimeSpan BarBeatToTimeSpan(int bar, float beat) => BeatOnlyToTimeSpan(BarBeatToBeatOnly(bar, beat));

        /// <summary>
        /// Converts an absolute beat position to its bar/beat representation using the current cache state.
        /// </summary>
        /// <param name="beat">The absolute beat position.</param>
        /// <returns>The one-based bar index and the one-based beat within that bar.</returns>
        public (int bar, float beat) BeatOnlyToBarBeat(float beat) => BeatOnlyToBarBeat(beat, in _cpbCache);

        /// <summary>
        /// Converts an absolute beat position to an absolute timespan using the current cache state.
        /// </summary>
        /// <param name="beat">The absolute beat position.</param>
        /// <returns>The corresponding absolute time.</returns>
        public TimeSpan BeatOnlyToTimeSpan(float beat) => BeatOnlyToTimeSpan(beat, in _bpmCache);

        /// <summary>
        /// Converts an absolute timespan to an absolute beat position using the current cache state.
        /// </summary>
        /// <param name="timeSpan">The absolute time to convert.</param>
        /// <returns>The corresponding absolute beat.</returns>
        public float TimeSpanToBeatOnly(TimeSpan timeSpan) => TimeSpanToBeatOnly(timeSpan, in _bpmCache);

        /// <summary>
        /// Converts an absolute timespan to its bar/beat representation using the current cache state.
        /// </summary>
        /// <param name="timeSpan">The absolute time to convert.</param>
        /// <returns>The one-based bar index and the one-based beat within that bar.</returns>
        public (int bar, float beat) TimeSpanToBarBeat(TimeSpan timeSpan) => BeatOnlyToBarBeat(TimeSpanToBeatOnly(timeSpan));
        private static float BarBeatToBeatOnly(int bar, float beat, in CpbCache[] cacheSet)
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
                    float beatOnly = cache.BeatOnly + beat - 1f;
                    return beatOnly;
                }
                break;
            }
            float finalBeatOnly = last.BeatOnly + (bar - last.Bar) * last.Cpb + beat - 1;
            return finalBeatOnly;
        }
        private static (int bar, float beat) BeatOnlyToBarBeat(float beat, in CpbCache[] cacheSet)
        {
            CpbCache last = CpbCache.Default;
            foreach (CpbCache cache in cacheSet)
            {
                float cbeat = cache.BeatOnly;
                if (cbeat < beat)
                {
                    last = cache;
                    continue;
                }
                if (cbeat == beat)
                    return (cache.Bar, 1f);
                break;
            }
            (int finalBar, float finalBeat) result2 = ((int)Math.Round(last.Bar + Math.Floor((double)((beat - last.BeatOnly) / last.Cpb))), (beat - last.BeatOnly) % last.Cpb + 1f);
            return result2;
        }
        private static TimeSpan BeatOnlyToTimeSpan(float beatOnly, in BpmCache[] cacheSet)
        {
            BpmCache last = BpmCache.Default;
            foreach (BpmCache cache in cacheSet)
            {
                float cbeat = cache.BeatOnly;
                if (cbeat < beatOnly)
                {
                    last = cache;
                    continue;
                }
                if (cbeat == beatOnly)
                    return cache.TimeSpan;
                break;
            }
            float durationFromLast = (beatOnly - last.BeatOnly) / last.Bpm;
            TimeSpan result = last.TimeSpan + TimeSpan.FromMinutes(durationFromLast);
            return result;
        }
        private static float TimeSpanToBeatOnly(TimeSpan timeSpan, in BpmCache[] cacheSet)
        {
            BpmCache last = BpmCache.Default;
            foreach (BpmCache cache in cacheSet)
            {
                TimeSpan ctime = cache.TimeSpan;
                if (ctime < timeSpan)
                {
                    last = cache;
                    continue;
                }
                if (ctime == timeSpan)
                    return cache.BeatOnly;
                break;
            }
            float beatOnly = last.BeatOnly + (float)((timeSpan - last.TimeSpan).TotalMinutes * last.Bpm);
            return beatOnly;
        }

        /// <summary>
        /// Creates an <see cref="RDBeat"/> from an absolute beat value.
        /// </summary>
        /// <param name="beatOnly">The absolute beat position.</param>
        /// <returns>An <see cref="RDBeat"/> bound to this calculator.</returns>
        public RDBeat BeatOf(float beatOnly) => new(this, beatOnly);

        /// <summary>
        /// Creates an <see cref="RDBeat"/> from a bar/beat pair.
        /// </summary>
        /// <param name="bar">The one-based bar index.</param>
        /// <param name="beat">The one-based beat within the bar.</param>
        /// <returns>An <see cref="RDBeat"/> bound to this calculator.</returns>
        public RDBeat BeatOf(int bar, float beat) => new(this, bar, beat);

        /// <summary>
        /// Creates an <see cref="RDBeat"/> from an absolute timespan.
        /// </summary>
        /// <param name="timeSpan">The absolute time to convert.</param>
        /// <returns>An <see cref="RDBeat"/> bound to this calculator.</returns>
        public RDBeat BeatOf(TimeSpan timeSpan) => new(this, timeSpan);

        /// <summary>
        /// Creates an <see cref="RDRange"/> representing the interval between two beats.
        /// </summary>
        /// <param name="beat1">The starting beat.</param>
        /// <param name="beat2">The ending beat.</param>
        /// <returns>The resulting interval.</returns>
        public RDRange IntervalOf(RDBeat beat1, RDBeat beat2) => new RDRange(new(this, beat1), new(this, beat2));

        /// <summary>
        /// Creates an <see cref="RDRange"/> representing the interval between two bar/beat pairs.
        /// </summary>
        /// <param name="beat1">The starting bar/beat tuple.</param>
        /// <param name="beat2">The ending bar/beat tuple.</param>
        /// <returns>The resulting interval.</returns>
        public RDRange IntervalOf((int bar, float beat) beat1, (int bar, float beat) beat2) => IntervalOf(BeatOf(beat1.bar, beat1.beat), BeatOf(beat2.bar, beat2.beat));

        /// <summary>
        /// Creates an <see cref="RDRange"/> representing the interval between two timespans.
        /// </summary>
        /// <param name="timeSpan1">The start time.</param>
        /// <param name="timeSpan2">The end time.</param>
        /// <returns>The resulting interval.</returns>
        public RDRange IntervalOf(TimeSpan timeSpan1, TimeSpan timeSpan2) => IntervalOf(BeatOf(timeSpan1), BeatOf(timeSpan2));
        IBeatRange<RDBeat> IBeatCalculator<RDBeat>.IntervalOf(RDBeat beat1, RDBeat beat2) => IntervalOf(beat1, beat2);
        IBeatRange<RDBeat> IBeatCalculator<RDBeat>.IntervalOf(TimeSpan timeSpan1, TimeSpan timeSpan2) => IntervalOf(timeSpan1, timeSpan2);
        /// <summary>
        /// Gets the BPM in effect at the specified beat.
        /// </summary>
        /// <param name="beat">The beat whose BPM should be retrieved.</param>
        /// <returns>The BPM active at the beat.</returns>
        public float BeatsPerMinuteOf(RDBeat beat)
        {
            BpmCache last = BpmCache.Default;
            foreach (BpmCache cache in _bpmCache)
            {
                float cbeat = cache.BeatOnly;
                if (cbeat < beat.BeatOnly)
                {
                    last = cache;
                    continue;
                }
                if (cbeat == beat.BeatOnly)
                    return cache.Bpm;
                break;
            }
            return last.Bpm;
        }

        /// <summary>
        /// Gets the CPB in effect at the specified beat.
        /// </summary>
        /// <param name="beat">The beat whose CPB should be retrieved.</param>
        /// <returns>The CPB active at the beat.</returns>
        public int CrotchetsPerBarOf(RDBeat beat)
        {
            CpbCache last = CpbCache.Default;
            foreach (CpbCache cache in _cpbCache)
            {
                float cbeat = cache.BeatOnly;
                if (cbeat < beat.BeatOnly)
                {
                    last = cache;
                    continue;
                }
                if (cbeat == beat.BeatOnly)
                    return cache.Cpb;
                break;
            }
            return last.Cpb;
        }
    }
}
