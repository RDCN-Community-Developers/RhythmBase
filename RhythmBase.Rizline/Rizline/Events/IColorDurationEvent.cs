namespace RhythmBase.Rizline.Events;

public interface IColorDurationEvent : IBaseEvent
{
	public Color StartColor { get; set; }
	public Color EndColor { get; set; }
}
