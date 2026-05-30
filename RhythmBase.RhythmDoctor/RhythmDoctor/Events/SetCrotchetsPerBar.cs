namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event to set the number of crotchets per bar.
/// </summary>
[JsonObjectSerializable]
public record class SetCrotchetsPerBar : BaseEvent, IBarBeginningEvent
{
	/// <summary>
	/// Gets the type of the event.
	/// </summary>
	public override EventType Type => EventType.SetCrotchetsPerBar;
	/// <summary>
	/// Gets the tab associated with the event.
	/// </summary>
	public override Tab Tab => Tab.Sounds;
	/// <summary>
	/// Gets or sets the visual beat multiplier.
	/// </summary>
	/// <remarks>
	/// Must be a value greater than 1.
	/// </remarks>
	public float VisualBeatMultiplier { get; set; } = 1;
	/// <summary>
	/// Gets or sets the number of crotchets per bar.
	/// </summary>
	/// <remarks>
	/// Must be a value greater than 1.
	/// </remarks>
	public int CrotchetsPerBar
	{
		get => _crotchetsPerBar + 1;
		set
		{
			_crotchetsPerBar = Math.Max(0, value - 1);
			if (_beat._calculator != null)
			{
				TickTime += 0f;
			}
		}
	}
	/// <summary>
	/// Returns a string that represents the current object.
	/// </summary>
	public override string ToString() => base.ToString() + $" CPB:{_crotchetsPerBar + 1}";
	internal int _crotchetsPerBar = 7;
}
