namespace RhythmBase.Rizline.Events;

public interface IKeyPointEvent : IFloorPositionEvent
{
	public float Value { get; set; }
}