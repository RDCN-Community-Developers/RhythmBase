namespace RhythmBase.Adofai.Events;

/// <summary>
/// Represents a checkpoint event in the ADOFAI game.
/// </summary>
[JsonObjectSerializable]
public class Checkpoint : BaseTileEvent, ISingleEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.Checkpoint;
	/// <summary>
	/// Gets or sets the tile offset for the checkpoint.
	/// </summary>
	public int TileOffset { get; set; } = 0;
}
