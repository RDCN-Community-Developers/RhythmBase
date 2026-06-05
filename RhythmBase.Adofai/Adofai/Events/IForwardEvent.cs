using System.Text.Json;

namespace RhythmBase.Adofai.Events;

/// <summary>
/// Represents a forward event in the Adofai system, used to preserve unknown or user-defined event data during serialization and deserialization.
/// </summary>
public interface IForwardEvent : IBaseEvent, Global.Events.IForwardEvent
{

    /// <summary>
    /// Gets or sets the raw event data as a <see cref="JsonElement"/>.
    /// </summary>
    JsonElement Data { get; set; }
}