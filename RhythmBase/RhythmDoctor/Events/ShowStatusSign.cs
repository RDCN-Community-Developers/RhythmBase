using RhythmBase.Global.Components.RichText;
using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event to show a status sign.
/// </summary>
[RDJsonObjectSerializable]
public record class ShowStatusSign : BaseEvent, IDurationEvent
{
	/// <summary>
	/// Gets or sets a value indicating whether to use beats.
	/// </summary>
	public bool UseBeats { get; set; } = true;
	/// <summary>
	/// Gets or sets a value indicating whether to narrate.
	/// </summary>
	[RDJsonCondition($"$&.{nameof(Narrate)}")]
	public bool Narrate { get; set; } = true;
	/// <summary>
	/// Gets or sets the text to display.
	/// </summary>
	[RDJsonConverter(typeof(RichTextConverter<RDRichStringStyle>))]
	public RDLine<RDRichStringStyle> Text { get; set; } = "";
	///<inheritdoc/>
	public float Duration { get; set; }
	/// <summary>
	/// Gets or sets the duration of the status sign as a <see cref="TimeSpan"/>.
	/// </summary>
	[RDJsonIgnore]
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
