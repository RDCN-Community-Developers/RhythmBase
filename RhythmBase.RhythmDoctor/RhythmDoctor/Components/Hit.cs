using RhythmBase.RhythmDoctor.Events;
using System.Diagnostics.CodeAnalysis;
namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Represents the moment a beat is hit in the rhythm game.
/// </summary>
public struct Hit
{
	/// <summary>
	/// Gets the moment of pressing the beat.
	/// </summary>
	public TickTime TickTime { get; }
	/// <summary>
	/// Gets the length of time the player held the beat.
	/// </summary>
	public float Hold { get; }
	/// <summary>
	/// Gets the source event for this hit.
	/// </summary>
	public BaseBeat Parent { get; }
	/// <summary>
	/// Gets a value indicating whether this hit needs to be held down continuously.
	/// </summary>
	public readonly bool Holdable => Hold > 0f;
	/// <summary>
	/// Initializes a new instance of the <see cref="Hit"/> struct.
	/// </summary>
	/// <param name="parent">The source event for this hit.</param>
	/// <param name="beat">The moment of pressing the beat.</param>
	/// <param name="hold">The length of time the player held the beat.</param>
	public Hit(BaseBeat parent, TickTime beat, float hold = 0f)
	{
		this = default;
		Parent = parent;
		TickTime = beat;
		Hold = hold;
	}
	public bool IsCompatible(Hit other)
	{
		if (Hold == 0 || other.Hold == 0)
			return true;
		float start1 = TickTime.Tick;
		float end1 = TickTime.Tick + Hold;
		float start2 = other.TickTime.Tick;
		float end2 = other.TickTime.Tick + other.Hold;
		if (end1 < start2 || end2 < start1)
			return true;
		if ((start1 <= start2 && end1 >= end2) || (start2 <= start1 && end2 >= end1))
			return true;
		return false;
	}
	public bool Merge(Hit other, [MaybeNullWhen(false)] out Hit result)
	{
		if (!IsCompatible(other))
		{
			result = default;
			return false;
		}
		if (Hold == 0 && other.Hold == 0)
		{
			if (TickTime == other.TickTime)
			{
				result = new Hit(Parent, TickTime, 0);
				return true;
			}
			result = default;
			return false;
		}
		if (Hold == 0 || other.Hold == 0)
		{
			Hit pointHit = Hold == 0 ? this : other;
			Hit rangeHit = Hold == 0 ? other : this;

			float pointTick = pointHit.TickTime.Tick;
			float rangeStart = rangeHit.TickTime.Tick;
			float rangeEnd = rangeHit.TickTime.Tick + rangeHit.Hold;
			if (pointTick >= rangeStart && pointTick <= rangeEnd)
			{
				result = new Hit(Parent, rangeHit.TickTime, rangeHit.Hold);
				return true;
			}
			result = default;
			return false;
		}
		float start1 = TickTime.Tick;
		float end1 = TickTime.Tick + Hold;
		float start2 = other.TickTime.Tick;
		float end2 = other.TickTime.Tick + other.Hold;
		if (start1 <= start2 && end1 >= end2)
		{
			result = new Hit(Parent, TickTime, Hold);
			return true;
		}
		if (start2 <= start1 && end2 >= end1)
		{
			result = new Hit(Parent, other.TickTime, other.Hold);
			return true;
		}
		result = default;
		return false;
	}
	/// <summary>
	/// Returns a string that represents the current object.
	/// </summary>
	/// <returns>A string that represents the current object.</returns>
	public readonly override string ToString() => $"{{{TickTime}, {Parent}}}";
}
