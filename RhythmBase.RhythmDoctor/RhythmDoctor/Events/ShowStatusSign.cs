using RhythmBase.Global.Components.RichText;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event to show a status sign.
/// </summary>
[JsonObjectSerializable]
public record class ShowStatusSign : BaseEvent, IDurationEvent
{
	/// <summary>
	/// Gets or sets a value indicating whether to use beats.
	/// </summary>
	public bool UseBeats { get; set; } = true;
	/// <summary>
	/// Gets or sets a value indicating whether to narrate.
	/// </summary>
	[JsonCondition($"$&.{nameof(Narrate)}")]
	public bool Narrate { get; set; } = true;
	/// <summary>
	/// Gets or sets the text to display.
	/// </summary>
	public RichLine<RichStringStyle> Text { get; set; } = "";
	///<inheritdoc/>
	public float Duration { get; set; }
	/// <summary>
	/// Gets or sets the duration of the status sign as a <see cref="TimeSpan"/>.
	/// </summary>
	[JsonIgnore]
	public TimeSpan TimeDuration
	{
		get
		{
			bool useBeats = UseBeats;
			TimeSpan timeDuration;
			timeDuration = useBeats ? TimeSpan.Zero : TimeSpan.FromSeconds((double)Duration);
			return timeDuration;
		}
		set
		{
			UseBeats = false;
			Duration = (float)value.TotalSeconds;
		}
	}
	///<inheritdoc/>
	public override EventType Type { get; } = EventType.ShowStatusSign;
	///<inheritdoc/>
	public override Tab Tab { get; } =  Tab.Actions;
	///<inheritdoc/>
	public override string ToString() => base.ToString() + $" {Text}";
}
