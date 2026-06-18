namespace RhythmBase.Rizline;

/// <summary>
/// Types of notes supported by Rizline levels. 
/// </summary>
public enum EventType
{
	/// <summary>
	/// Tap note (single hit). 
	/// </summary>
	Tap,
	/// <summary>
	/// Drag note (short slide). 
	/// </summary>
	Drag,
	/// <summary>
	/// Hold note (sustained). 
	/// </summary>
	Hold,
	/// <summary>
	/// A Riztime challenge section with start/end times and transition duration.
	/// </summary>
	ChallengeTime,
	/// <summary>
	/// A point on a guide line defining its shape, color and position.
	/// </summary>
	LinePoint,
	/// <summary>
	/// A color transition applied to judge rings on a guide line.
	/// </summary>
	JudgeRingColor,
	/// <summary>
	/// An overall line color overlay for a guide line.
	/// </summary>
	LineColor,
	/// <summary>
	/// A horizontal position key point for canvas movement.
	/// </summary>
	CanvasPosition,
	/// <summary>
	/// A canvas flow speed key point.
	/// </summary>
	CanvasSpeed,
	/// <summary>
	/// A camera zoom/scale key point.
	/// </summary>
	CameraScale,
	/// <summary>
	/// A camera horizontal position key point.
	/// </summary>
	CameraPosition,
	/// <summary>
	/// A BPM (beats per minute) shift key point.
	/// </summary>
	BpmShift,
	/// <summary>
	/// A forwarding event used internally.
	/// </summary>
	ForwardEvent,
}
