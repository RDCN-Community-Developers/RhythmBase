namespace RhythmBase.Adofai.Events;

/// <summary>
/// Represents a base class for tagged tile actions in the Adofai event system.
/// </summary>
[JsonObjectSerializable]
public abstract class BaseTaggedTileEvent : BaseTileEvent
{
	/// <inheritdoc/>
	public float AngleOffset { get; set; }
	/// <summary>
	/// Gets or sets the event tag associated with the tile action.
	/// </summary>
	public string EventTag { get; set; } = string.Empty;
}
