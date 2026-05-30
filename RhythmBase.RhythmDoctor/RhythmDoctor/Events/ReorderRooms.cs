using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event to reorder rooms.
/// </summary>
[JsonObjectSerializable]
public record class ReorderRooms : BaseEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReorderRooms"/> class.
    /// </summary>
    public ReorderRooms()
    {
        Tab = Tab.Rooms;
    }
    /// <summary>
    /// Gets or sets the order of the rooms.
    /// </summary>
    public Order Order
    {
        get; set
        {
            if (value.Length != 4)
                throw new ArgumentException("The order must contain exactly 4 elements.", nameof(value));
            field = value;
        }
    } = [0, 1, 2, 3];
    ///<inheritdoc/>
    public override EventType Type => EventType.ReorderRooms;
    ///<inheritdoc/>
    public override Tab Tab { get; }
}
