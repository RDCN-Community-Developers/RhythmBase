using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event that reorders the windows (window slots) in the Rhythm Doctor UI.
/// </summary>
[JsonObjectSerializable]
public record class ReorderWindows : BaseWindowEvent
{
    /// <inheritdoc/>
    public override int Y => 0;

    /// <summary>
    /// Gets or sets the order of rooms used to rearrange windows.
    /// </summary>
    public Order Order { get; set; } = [0, 1, 2, 3];
    ///<inheritdoc/>
    public override EventType Type => EventType.ReorderWindows;
}
