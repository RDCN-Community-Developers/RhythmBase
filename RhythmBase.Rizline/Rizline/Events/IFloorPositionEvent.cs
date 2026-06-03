namespace RhythmBase.Rizline.Events;

public interface IFloorPositionEvent : IBaseEvent, IEaseEvent
{
	public float FloorPosition { get; set; }
}
