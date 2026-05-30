namespace RhythmBase.Adofai.Events;

/// <summary>
/// Represents the ScaleRadius event in the ADOFAI editor.
/// </summary>
[JsonObjectSerializable]
public class ScaleRadius : BaseTileEvent, ISingleEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.ScaleRadius;
	/// <summary>
	/// Gets or sets the scale value for the radius adjustment.
	/// </summary>
	public float Scale { get; set; } = 100f;
}
