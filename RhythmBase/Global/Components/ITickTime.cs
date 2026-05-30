namespace RhythmBase.Global.Components;


/// <summary>
/// Represents a beat that can be compared and equated with other beats of the same type.
/// </summary>
/// <typeparam name="TSelf">The type that implements this interface.</typeparam>
public interface ITickTime<TSelf> : IComparable<TSelf>, IEquatable<TSelf> where TSelf : ITickTime<TSelf>
{
    /// <summary>
    /// Gets the time span of the beat.
    /// </summary>
    public TimeSpan TimeSpan { get; }
    /// <summary>
    /// Gets the beat-only value.
    /// </summary>
    public float Tick { get; }
//#if NET8_0_OR_GREATER
//    internal abstract static TSelf FromBeatOnly(float beatOnly);
//    internal abstract static TSelf FromTimeSpan(TimeSpan timeSpan);
//#endif
}
