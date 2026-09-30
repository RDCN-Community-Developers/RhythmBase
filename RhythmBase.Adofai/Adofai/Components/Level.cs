using RhythmBase.Adofai.Events;
using RhythmBase.Adofai.Extensions;
using RhythmBase.Adofai.Utils;
using RhythmBase.Global.Components;
using static RhythmBase.Adofai.Constants;

namespace RhythmBase.Adofai.Components;

/// <summary>
/// Adofal level.
/// </summary>
public partial class Level :
    TileCollection,
    IJsonChart<Level, TickTime>,
    IArchiveLevel<Level, Level>
{
    /// <summary>
    /// Level settings.
    /// </summary>
    public Settings Settings { get; set; }
    /// <summary>
    /// Level decoration collection.
    /// </summary>
    public List<IBaseEvent> Decorations { get; set; }
    /// <summary>
    /// Time-authoritative events (those carrying an <see cref="BaseEvent.AngleOffset"/>), ordered by tick.
    /// Their position is stored as <see cref="BaseEvent.TickTime"/>; the owning floor is assigned on export.
    /// </summary>
    public List<BaseEvent> FreeEvents { get; set; }
    /// <summary>
    /// Get all the events of the level.
    /// </summary>
    public override IEnumerable<IBaseEvent> Events
    {
        get
        {
            foreach (IBaseEvent tile in base.Events)
                yield return tile;
            foreach (BaseEvent free in OrderedFreeEvents)
                yield return free;
            foreach (IBaseEvent tile2 in Decorations)
                yield return tile2;
        }
    }
    /// <summary>
    /// Gets the time-authoritative events ordered by their tick, which is the canonical order for them.
    /// </summary>
    public IEnumerable<BaseEvent> OrderedFreeEvents => FreeEvents.OrderBy(GetFreeEventTick);
    private static float GetFreeEventTick(BaseEvent item)
    {
        TickTime tick = item.TickTime;
        return tick.IsEmpty ? float.MaxValue : tick.Tick;
    }
    /// <summary>
    /// The calculator that comes with the level.
    /// </summary>
    public BeatCalculator Calculator { get; }
    /// <summary>  
    /// Initializes a new instance of the <see cref="Level"/> class.  
    /// </summary>  
    public Level()
    {
        Settings = new Settings();
        Decorations = [];
        FreeEvents = [];
        Calculator = new BeatCalculator(this);
        End.Parent = this;
    }
    /// <summary>  
    /// Initializes a new instance of the <see cref="Level"/> class with a collection of tiles.  
    /// </summary>  
    /// <param name="items">The collection of tiles to initialize the level with.</param>  
    public Level(IEnumerable<Tile> items)
    {
        Settings = new Settings();
        Decorations = [];
        FreeEvents = [];
        Calculator = new BeatCalculator(this);
        End.Parent = this;
        this.InsertRange(0, items);
    }
    /// <summary>
    /// Gets the default level, which consists of 10 repeated tiles.
    /// </summary>
    public static Level Default
    {
        get
        {
            Level level = [.. new Tile().Repeat(10)];
            level.Settings = new()
            {
                Version = DefaultVersion,
            };
            return level;
        }
    }
    /// <inheritdoc/>
    public string ResolvedPath { get; internal set; } = string.Empty;
    /// <inheritdoc/>
    public string Filepath { get; internal set; } = string.Empty;
    /// <inheritdoc/>
    public string ResolvedDirectory => Path.GetDirectoryName(ResolvedPath) ?? "";
    /// <summary>
    /// Gets or sets the name of the chart (e.g. the file name within a level).
    /// </summary>
    public string Name { get; set; } = "main";
    /// <summary>
    /// Gets the charts contained in this level, keyed by chart name. An Adofai level contains a single chart.
    /// </summary>
    public ChartDictionary<Level> Charts
    {
        get
        {
            ChartDictionary<Level> dictionary = new();
            dictionary.Add(Name, this);
            return dictionary;
        }
    }
    /// <summary>  
    /// Adds a tile to the level.  
    /// Sets the parent of the tile to this level before adding it to the base collection.  
    /// </summary>  
    /// <param name="item">The tile to add to the level.</param>  
    public override void Add(Tile item) => Insert(Count, item);
    /// <summary>
    /// Adds a time-authoritative event (an event with an angle offset) to the level.
    /// Such events are not attached to a tile; their floor is assigned on export.
    /// </summary>
    /// <param name="item">The event to add.</param>
    /// <returns><see langword="true"/> when the event was added.</returns>
    public bool Add(BaseEvent item)
    {
        FreeEvents.Add(item);
        Calculator.MarkDirty();
        return true;
    }
    /// <summary>
    /// Removes a time-authoritative event from the level.
    /// </summary>
    /// <param name="item">The event to remove.</param>
    /// <returns><see langword="true"/> when the event was removed.</returns>
    public bool Remove(BaseEvent item)
    {
        bool result = FreeEvents.Remove(item);
        if (result)
            Calculator.MarkDirty();
        return result;
    }
    /// <summary>
    /// Places a time-authoritative event at a tile plus angle offset. The authoritative tick is
    /// calculated from that position and the derived floor is cleared so it is reassigned on export.
    /// </summary>
    /// <param name="item">The event to place.</param>
    /// <param name="tile">The tile to attach the position to.</param>
    /// <param name="angleOffset">The angle offset (degrees) into the tile.</param>
    public void PlaceFreeEvent(BaseEvent item, Tile tile, float angleOffset = 0f)
    {
        item.SetTick(Calculator.TickOf(tile, angleOffset));
        Calculator.MarkDirty();
    }
    /// <summary>  
    /// Inserts a tile into the level at the specified index.  
    /// Sets the parent of the tile to this level before inserting it into the base collection.  
    /// </summary>  
    /// <param name="index">The zero-based index at which the tile should be inserted.</param>  
    /// <param name="item">The tile to insert into the level.</param>  
    public override void Insert(int index, Tile item)
    {
        item.Parent = this;
        base.Insert(index, item);
        Calculator.MarkDirty();
    }
    /// <summary>
    /// Removes the tile at the specified index from the collection.
    /// </summary>
    /// <remarks>After removal, the tile's <see cref="Tile.Parent"/>, <see cref="Tile.Previous"/>, and <see
    /// cref="Tile.Next"/> properties are set to <see langword="null"/>.</remarks>
    /// <param name="index">The zero-based index of the tile to remove.</param>
    public void RemoveAt(int index)
    {
        Tile tile = this[index];
        tile.Parent = null!;
        tile.Previous = null;
        tile.Next = null!;
        tileOrder.RemoveAt(index);
        Calculator.MarkDirty();
    }
    /// <summary>  
    /// Removes all tiles from the level.  
    /// Sets the parent of each tile to null before clearing the base collection.  
    /// </summary>  
    public override void Clear()
    {
        foreach (Tile item in tileOrder)
        {
            item.Parent = null;
        }
        base.Clear();
        Calculator.MarkDirty();
    }
    /// <inheritdoc/>
    public void Dispose()
    {
        if (isZip && isExtracted && Directory.Exists(ResolvedDirectory))
        {
            Directory.Delete(ResolvedDirectory, true);
        }
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public override string ToString() => $"\"{Settings.Song}\" Count = {Count}";

    private bool isZip = false;
    private bool isExtracted = false;
}