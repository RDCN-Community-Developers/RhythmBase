namespace RhythmBase.Adofai.Events;

/// <summary>
/// Represents the ScaleMargin event, which adjusts the margin scaling in the level.
/// </summary>
[JsonObjectSerializable]
public class ScaleMargin : BaseTileEvent, ISingleEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.ScaleMargin;
	/// <summary>
	/// Gets or sets the scale value for the margin adjustment.
	/// </summary>
	public float Scale { get; set; } = 100f;
}
