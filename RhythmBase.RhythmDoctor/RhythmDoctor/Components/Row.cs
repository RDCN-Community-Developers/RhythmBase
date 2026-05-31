using RhythmBase.RhythmDoctor.Converters;
using RhythmBase.RhythmDoctor.Events;
using RhythmBase.RhythmDoctor.Extensions;
using RhythmBase.RhythmDoctor.Linq;
using RhythmBase.RhythmDoctor.Utils;
namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// A collection of row events.
/// </summary>
public class Row : OrderedEventCollection<BaseRowAction, EventType, TickTime>, IEventEnumerable<BaseRowAction>
{
    /// <summary>
    /// Gets or sets the character associated with the row.
    /// </summary>
    public RDCharacter Character { get; set; } = Characters.Samurai;
    /// <summary>
    /// Gets or sets the CPU marker character used to represent the CPU.
    /// </summary>
    public Characters CpuMarker { get; set; } = Characters.Otto;
    /// <summary>
    /// Gets or sets the type of the row.
    /// </summary>
    public RowType RowType { get; set; }
    /// <summary>
    /// Gets the index of the row.
    /// </summary>
    public int Index => Parent?.Rows.IndexOf(this) ?? throw new RhythmBaseException();
    /// <summary>
    /// Gets or sets the rooms associated with the row.
    /// </summary>
    public SingleRoom Room { get; set; } = new(RoomIndex.Room1);
    /// <summary>
    /// Gets or sets a value indicating whether the row is hidden at the start.
    /// </summary>
    public bool HideAtStart { get; set; }
    /// <summary>
    /// Gets or sets the initial player mode for the row.
    /// </summary>
    public PlayerType Player { get; set; } = PlayerType.P1;
    /// <summary>
    /// Gets the initial beat sound for the row.
    /// </summary>
    public Audio Sound { get; set; } = new Audio();
    /// <summary>
    /// Gets or sets the length of the row.
    /// </summary>
    /// <remarks>
    /// It only affects the visual length of the row and does not affect the actual timing or behavior of the events within the row.
    /// </remarks>
    public int Length { get; set; } = 7;
    /// <summary>
    /// Gets or sets a value indicating whether the beats are muted.
    /// </summary>
    [JsonCondition($"$&.{nameof(MuteBeats)}")]
    public bool MuteBeats { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether audio should be muted when the game is in single-player mode.
    /// </summary>
    [JsonAlias("muteIn1P")]
    [JsonCondition($"$&.{nameof(MuteInSinglePlayerMode)}")]
    public bool MuteInSinglePlayerMode { get; set; }
    /// <summary>
    /// Gets or sets the row to mimic.
    /// </summary>
    public sbyte RowToMimic { get; set; } = -1;
    /// <summary>
    /// The Index of the row within its room, starting from 0. Returns -1 if the row is not part of a room or if the parent level is null.
    /// </summary>
    public int Y
    {
        get
        {
            int y = 0;
            for (int i = 0; i < Parent?.Rows.Count; i++)
            {
                if (Parent.Rows[i].Room == Room)
                {
                    if (Parent.Rows[i] == this)
                        return y;
                    y++;
                }
            }
            return -1;
        }
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="Row"/> class.
    /// </summary>
    public Row() { }
    protected override TickTime CreateInstance(float beat) => new(beat);
    protected override ITickRange<TickTime> CreateRange(float? start, float? end) => new Range(
    start is float s ? new TickTime(s) : null,
    end is float e ? new TickTime(e) : null
    );
    protected override ReadOnlyEnumCollection<EventType> Types => ClassEnumMap.ToEnums<BaseRowAction>();
    protected override ReadOnlyEnumCollection<EventType> TypesOf<TTarget>() => ClassEnumMap.ToEnums(typeof(TTarget));
    /// <summary>
    /// Adds an item to the row.
    /// </summary>
    /// <param name="item">The row event to add.</param>
    public override bool Add(BaseRowAction item)
    {
        if (item._parent == this)
            return false;
        item._parent?.Remove(item);
        item._parent = this;
        bool success = base.Add(item);
        if (Parent is not null)
            success &= Parent.AddDirectlyInternal(item);
        return success;
    }
    /// <summary>
    /// Removes an item from the row.
    /// </summary>
    /// <param name="item">The row event to remove.</param>
    /// <returns>True if the item was successfully removed; otherwise, false.</returns>
    public override bool Remove(BaseRowAction item)
    {
        return (Parent?.RemoveDirectlyInternal(item) ?? true) && base.Remove(item);
    }

    [JsonIgnore]
    internal Level? Parent = null;
}
