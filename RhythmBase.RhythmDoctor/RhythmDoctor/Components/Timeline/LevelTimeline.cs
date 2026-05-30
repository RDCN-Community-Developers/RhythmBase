using RhythmBase.RhythmDoctor.Components.TimeLine;

namespace RhythmBase.RhythmDoctor.Components.Timeline;

/// <summary>
/// Represents the timeline for a level, including rows, decorations, and rooms.
/// </summary>
public class LevelTimeline
{
#pragma warning disable CS8618
	/// <summary>
	/// Gets the array of row timelines for this level.
	/// </summary>
	public RowTimeline[] Rows { get; internal set; }

	/// <summary>
	/// Gets the array of decoration timelines for this level.
	/// </summary>
	public DecorationTimeline[] Decorations { get; internal set; }

	/// <summary>
	/// Gets the array of room timelines for this level.
	/// </summary>
	public RoomTimeline[] Rooms { get; internal set; }
}
