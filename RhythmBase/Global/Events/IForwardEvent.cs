namespace RhythmBase.Global.Events;

/// <summary>
/// Represents a forward-compatible event whose actual type is stored as a string, enabling round-trip
/// serialization of unknown event types.
/// </summary>
public interface IForwardEvent
{
    /// <summary>
    /// Gets or sets the original event type name as it appears in the level file.
    /// </summary>
    string ActualType { get; }
}
