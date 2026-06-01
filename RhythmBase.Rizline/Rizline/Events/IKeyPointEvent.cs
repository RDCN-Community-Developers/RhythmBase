namespace RhythmBase.Rizline.Rizline.Events;

public interface IKeyPointEvent : IFloorPositionEvent
{
	public float Value { get; set; }
}
