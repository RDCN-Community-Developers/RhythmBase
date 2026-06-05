namespace RhythmBase.Adofai.Events;

/// <summary>
/// An event that can only have one instance in a tile. This is used to prevent multiple instances of the same event type from being added to a tile, which can cause conflicts and unintended behavior in the game. Events that implement this interface will be treated as single-instance events, and any attempt to add another instance of the same event type to the same tile will be ignored or result in an error, depending on the implementation of the event system.
/// </summary>
public interface ISingleEvent
{
}
