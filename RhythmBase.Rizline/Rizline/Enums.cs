namespace RhythmBase.Rizline.Rizline;

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
	ChallengeTime,
	LinePoint,
	JudgeRingColor,
	LineColor,
	CanvasPosition,
	CanvasSpeed,
	CameraScale,
	CameraPosition,
}
