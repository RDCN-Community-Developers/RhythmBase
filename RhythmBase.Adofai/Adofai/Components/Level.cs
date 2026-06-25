using RhythmBase.Adofai.Events;
using RhythmBase.Adofai.Extensions;
using RhythmBase.Adofai.Utils;
using static RhythmBase.Adofai.Constants;

namespace RhythmBase.Adofai.Components;

/// <summary>
/// Adofal level.
/// </summary>
public partial class Level :
    TileCollection,
    IJsonLevel<Level, IBaseEvent, EventType, TickTime>,
    ISingleFileLevel<Level, IBaseEvent, EventType, TickTime>,
    IArchiveLevel<Level, IBaseEvent, EventType, TickTime>,
    IChart<TickTime>
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
    /// Get all the events of the level.
    /// </summary>
    public override IEnumerable<IBaseEvent> Events
    {
        get
        {
            foreach (IBaseEvent tile in base.Events)
                yield return tile;
            foreach (IBaseEvent tile2 in Decorations)
                yield return tile2;
        }
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
    /// Adds a tile to the level.  
    /// Sets the parent of the tile to this level before adding it to the base collection.  
    /// </summary>  
    /// <param name="item">The tile to add to the level.</param>  
    public override void Add(Tile item) => Insert(Count, item);
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
    }
    /// <inheritdoc/>
    public void Dispose()
    {
        if (isZip)
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