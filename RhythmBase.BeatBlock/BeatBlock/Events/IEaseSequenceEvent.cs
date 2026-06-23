namespace RhythmBase.BeatBlock.Events
{
	/// <summary>
	/// Interface for events that can have an ease sequence
	/// </summary>
	public interface IEaseSequenceEvent : IBaseEvent
	{
		/// <summary>
		/// Ease sequence to use, if any
		/// </summary>
		string? EaseSequence { get; set; }
	}
}