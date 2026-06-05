namespace RhythmBase.Adofai.Events;

/// <summary>
/// Represents a bookmark event in the ADOFAI game.
/// </summary>
[JsonObjectSerializable]
public class Bookmark : BaseTileEvent, IBeginningEvent,	ISingleEvent
{
	/// <inheritdoc/>s
	public override EventType Type => EventType.Bookmark;
}