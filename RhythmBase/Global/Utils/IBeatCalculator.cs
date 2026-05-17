using RhythmBase.RhythmDoctor.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace RhythmBase.Global.Utils
{
    public interface IBeatCalculator<TBeat>
        where TBeat : struct, IBeat<TBeat>
    {
        TBeat BeatOf(float beatOnly);
        TBeat BeatOf(TimeSpan timeSpan);
        TimeSpan BeatOnlyToTimeSpan(float beat);
        float BeatsPerMinuteOf(TBeat beat);
        IBeatRange<TBeat> IntervalOf(TBeat beat1, TBeat beat2);
        IBeatRange<TBeat> IntervalOf(TimeSpan timeSpan1, TimeSpan timeSpan2);
        void Refresh();
        float TimeSpanToBeatOnly(TimeSpan timeSpan);
    }
}
