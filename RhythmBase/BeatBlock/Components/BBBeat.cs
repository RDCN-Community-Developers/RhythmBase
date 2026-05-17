using RhythmBase.RhythmDoctor.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace RhythmBase.BeatBlock.Components
{
    public struct BBBeat : IBeat<BBBeat>
    {
        public TimeSpan TimeSpan { get; }
        public float BeatOnly { get; }
        public int CompareTo(BBBeat other)
        {
            throw new NotImplementedException();
        }
        public bool Equals(BBBeat other)
        {
            throw new NotImplementedException();
        }
        public BBBeat(float beatOnly)
        {
            TimeSpan = TimeSpan.Zero;
            BeatOnly = beatOnly;
        }
    }
}
