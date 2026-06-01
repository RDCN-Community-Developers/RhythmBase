using RhythmBase.Rizline.Events;

namespace RhythmBase.Rizline.Rizline.Events;

public interface IFloorPositionEvent : IBaseEvent, IEaseEvent
{
	public float FloorPosition { get; set; }
}
