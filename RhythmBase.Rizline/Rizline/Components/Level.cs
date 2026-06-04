using RhythmBase.Rizline.Events;

namespace RhythmBase.Rizline.Components;

///// <summary>
///// Represents a generic key point used for timeline-based changes such as BPM shifts,
///// canvas speed/position and camera transforms.
///// </summary>
///// <remarks>
///// Time is expressed in ticks (1 tick = 60 / BPM seconds in Rizline format).
///// The <see cref="FloorPosition"/> field stores a precomputed cumulative vertical offset.
///// </remarks>
//public record struct KeyPoint
//{
//	/// <summary>
//	/// Time when the key point takes effect, in ticks. 
//	/// </summary>
//	public float Time { get; set; }

//	/// <summary>
//	/// Value applied at <see cref="Time"/> (meaning depends on context). 
//	/// </summary>
//	public float Value { get; set; }

//	/// <summary>
//	/// Ease type used to interpolate from this key point to the next. 
//	/// </summary>
//	public EaseType EaseType { get; set; }

//	/// <summary>
//	/// Cumulative floor position (height) at this key point. For canvas speed changes
//	/// this represents the vertical offset in the canvas at <see cref="Time"/>.
//	/// </summary>
//	public float FloorPosition { get; set; }
//}

///// <summary>
///// Represents a color transition that starts at a given time. 
///// </summary>
//public struct ColorDuration
//{
//	/// <summary>
//	/// Start color of the duration segment. 
//	/// </summary>
//	public Color StartColor { get; set; }

//	/// <summary>
//	/// End color of the duration segment. 
//	/// </summary>
//	public Color EndColor { get; set; }

//	/// <summary>
//	/// Start time of the color segment in ticks. 
//	/// </summary>
//	public float Time { get; set; }
//}

/// <summary>
/// Guide line that contains line points, notes and optional color overlays for the line
/// and its judge rings.
/// </summary>
public class Line
{
	/// <summary>
	/// Ordered list of guide line points defining the shape and color stops. 
	/// </summary>
	public List<LinePoint> LinePoints { get; } = [];

	/// <summary>
	/// Notes placed on this guide line, ordered by time. 
	/// </summary>
	public List<BaseNote> Notes { get; } = [];

	/// <summary>
	/// Judge ring color transitions for this line. 
	/// </summary>
	public List<JudgeRingColor> JudgeRingColor { get; } = [];

	/// <summary>
	/// Overall line color overlays that are mixed with node colors. 
	/// </summary>
	public List<LineColor> LineColor { get; } = [];
}

/// <summary>
/// Stores canvas-specific movement and flow-speed key points.
/// </summary>
public class CanvasMove
{
	/// <summary>
	/// Index of the canvas/track this entry applies to. 
	/// </summary>
	public int Index { get; set; }

	/// <summary>
	/// Horizontal position key points for canvas movement. 
	/// </summary>
	public List<CanvasPosition> XPosition { get; } = [];

	/// <summary>
	/// Canvas speed (flow) key points. 
	/// </summary>
	public List<CanvasSpeed> Speed { get; } = [];
}

/// <summary>
/// Camera movement and zoom key points used by the level. 
/// </summary>
public class CameraMove
{
	/// <summary>
	/// Scale (zoom) key points for the camera. 
	/// </summary>
	public List<CameraScale> Scales { get; } = [];

	/// <summary>
	/// Horizontal position key points for camera panning. 
	/// </summary>
	public List<CameraPosition> XPosition { get; } = [];
}
public class Chart : IChart<TickTime>
{
	/// <summary>
	/// Level file version identifier.
	/// </summary>
	public int FileVersion { get; set; }
	/// <summary>
	/// The name of the song.
	/// </summary>
	public string SongsName { get; set; } = string.Empty;
	/// <summary>
	/// Chart delay relative to the song, represented as a <see cref="TimeSpan"/>.
	/// </summary>
	public TimeSpan Delay { get; set; }
	/// <summary>
	/// Offset applied to the level timing.
	/// </summary>
	public TimeSpan Offset { get; set; }
	/// <summary>
	/// The collection of themes used by this level.
	/// </summary>
	public ThemeCollection Themes { get; set; } = new();
	/// <summary>
	/// List of Riztime challenge time ranges.
	/// </summary>
	public List<ChallengeTime> ChallengeTimes { get; set; } = [];
	/// <summary>
	/// Base BPM of the song.
	/// </summary>
	public float Bpm { get; set; }
	/// <summary>
	/// Ordered BPM shift key points.
	/// </summary>
	public List<BpmShift> BpmShifts { get; set; } = [];
	/// <summary>
	/// All guide lines contained in the level.
	/// </summary>
	public List<Line> Lines { get; set; } = [];
	/// <summary>
	/// Canvas movement entries for each canvas/track.
	/// </summary>
	public List<CanvasMove> CanvasMoves { get; set; } = [];
	public CameraMove CameraMove { get; set; } = new();
	IBeatCalculator<TickTime> IChart<TickTime>.Calculator { get; }
}

public enum Difficulty
{
	Easy,
	Hard,
	Insane,
	Another,
}

/// <summary>
/// Core Rizline level representation with metadata, timing and content lists.
/// </summary>
public partial class Level :
		IArchiveLevel<Level, IBaseEvent, EventType, TickTime>,
		IMultiFileLevel<Level, IBaseEvent, EventType, TickTime>
{
	public string Title { get; set; } = string.Empty;
	public string Composer { get; set; } = string.Empty;
	public int Difficulty { get; set; }
	public int Lv { get; set; }
	public int MaxHit { get; set; }
	public int MaxScore { get; set; }
	public TimeSpan PreviewTime { get; set; }
	public List<Chart> Charts { get; } = [];
	/// <summary>
	/// Original file path of the level, if any. 
	/// </summary>
	public string? Filepath { get; internal set; }

	/// <summary>
	/// Beat calculator instance used for timing calculations. 
	/// </summary>
	public IBeatCalculator<TickTime> Calculator { get; }

	/// <summary>
	/// Resolved absolute path to the level file. 
	/// </summary>
	public string ResolvedPath { get; internal set; } = string.Empty;

	/// <summary>
	/// Resolved directory containing the level file, if available. 
	/// </summary>
	public string? ResolvedDirectory { get; internal set; }

	/// <summary>
	/// Default instance used as a fallback. 
	/// </summary>
	public static Level Default => new Level();

	/// <summary>
	/// Dispose resources held by the level. 
	/// </summary>
	public void Dispose()
	{
		throw new NotImplementedException();
	}

}