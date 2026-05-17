using RhythmBase.RhythmDoctor.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace RhythmBase.Global.Components;


public interface IBeat<TSelf> : IComparable<TSelf>, IEquatable<TSelf> where TSelf : IBeat<TSelf>
{
    public TimeSpan TimeSpan { get; }
    public float BeatOnly { get; }
//#if NET8_0_OR_GREATER
//    internal abstract static TSelf FromBeatOnly(float beatOnly);
//    internal abstract static TSelf FromTimeSpan(TimeSpan timeSpan);
//#endif
}
