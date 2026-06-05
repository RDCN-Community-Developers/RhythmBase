namespace RhythmBase.Rizline.Components;

public struct TickTime : ITickTime<TickTime>
{
	public TimeSpan TimeSpan { get; }
	public float Tick { get; }

	public int CompareTo(TickTime other)
	{
		return Tick.CompareTo(other.Tick);
	}

	public bool Equals(TickTime other)
	{
		return Tick.Equals(other.Tick);
	}
	public TickTime(float tick)
	{
		Tick = tick;
	}
}
public struct Range : ITickRange<TickTime>
{
	public TickTime? Start { get; }
	public TickTime? End { get; }
	public Range(TickTime? start, TickTime? end)
	{
		Start = start;
		End = end;
	}
	public bool Contains(TickTime b)
	{
		throw new NotImplementedException();
	}
	public (TickTime Start, TickTime End) GetStartAndEnd(TickTime endTime)
	{
		throw new NotImplementedException();
	}

	public ITickRange<TickTime> Intersect(ITickRange<TickTime> other)
	{
		throw new NotImplementedException();
	}

	public ITickRange<TickTime> Union(ITickRange<TickTime> other)
	{
		throw new NotImplementedException();
	}
#if NET8_0_OR_GREATER
	static ITickRange<TickTime> ITickRange<TickTime>.Infinity => new Range(null, null);
	static ITickRange<TickTime> ITickRange<TickTime>.Empty => new Range(new(), new());
#endif
}
