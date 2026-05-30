using RhythmBase.RhythmDoctor.Events;
namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Represents the moment a beat is hit in the rhythm game.
/// </summary>
public struct Hit
{
	/// <summary>
	/// Gets the moment of pressing the beat.
	/// </summary>
	public TickTime Beat { get; }
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
		Beat = beat;
		Hold = hold;
	}
	/// <summary>
	/// Returns a string that represents the current object.
	/// </summary>
	/// <returns>A string that represents the current object.</returns>
	public readonly override string ToString() => $"{{{Beat}, {Parent}}}";
}
