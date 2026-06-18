namespace RhythmBase.Rizline.Events;

/// <summary>
/// Interface for events that define a color transition with start and end colors.
/// </summary>
public interface IColorDurationEvent : IBaseEvent
{
	/// <summary>
	/// The starting color of the transition.
	/// </summary>
	public Color StartColor { get; set; }
	/// <summary>
	/// The ending color of the transition.
	/// </summary>
	public Color EndColor { get; set; }
}
