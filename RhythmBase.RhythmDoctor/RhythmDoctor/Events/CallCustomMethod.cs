namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event that calls a custom method.
/// </summary>
[JsonObjectSerializable]
public partial record class CallCustomMethod : BaseEvent
{
	/// <summary>
	/// Gets or sets the name of the method to be called.
	/// </summary>
	public string MethodName { get; set; } = "";
	/// <summary>
	/// Gets or sets the execution time of the method.
	/// </summary>
	public EventExecutionTimeOption ExecutionTime { get; set; } = EventExecutionTimeOption.OnBar;
	/// <summary>
	/// Gets or sets the sort offset for the event.
	/// </summary>
	public int SortOffset { get; set; }
	/// <inheritdoc/>
	public override EventType Type => EventType.CallCustomMethod;
	/// <inheritdoc/>
	public override Tab Tab => Tab.Actions;
	/// <inheritdoc/>
	public override string ToString() => base.ToString() + $" {MethodName}";
}