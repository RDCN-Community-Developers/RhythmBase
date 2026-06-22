using RhythmBase.BeatBlock.Converters;
using RhythmBase.BeatBlock.Events;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using static RhythmBase.BeatBlock.Constants;

namespace RhythmBase.BeatBlock.Components;
/// <summary>
/// Represents the metadata of a BeatBlock level.
/// </summary>
public record class Metadata
{
    /// <summary>
    /// Gets or sets the name of the song.
    /// </summary>
    public string SongName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the artist of the song.
    /// </summary>
    public string Artist { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the artist link.
    /// </summary>
    public string ArtistLink { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the beats per minute of the song.
    /// </summary>
    public float Bpm { get; set; }
    /// <summary>
    /// Gets or sets the description of the level.
    /// </summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the charter of the level.
    /// </summary>
    public string Charter { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the difficulty of the level.
    /// </summary>
    public int Difficulty { get; set; } = 0;
    /// <summary>
    /// Gets or sets a value indicating whether a light warning is enabled.
    /// </summary>
    public bool LightWarning { get; set; } = false;
    /// <summary>
    /// Gets or sets a value indicating whether loop points are enabled.
    /// </summary>
    public bool LoopPointsEnable { get; set; } = false;
    /// <summary>
    /// Gets or sets a value indicating whether a lyrics warning is enabled.
    /// </summary>
    public bool LyricsWarning { get; set; } = false;
    /// <summary>
    /// Gets or sets the source of the level.
    /// </summary>
    public string Source { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets a value indicating whether the background is enabled.
    /// </summary>
    public bool IsBackgroundEnabled { get; set; } = true;
    /// <summary>
    /// Gets or sets the start loop point.
    /// </summary>
    public float StartLoop { get; set; } = 0;
    /// <summary>
    /// Gets or sets the end loop point.
    /// </summary>
    public float EndLoop { get; set; } = 0;
    /// <summary>
    /// Gets or sets the background data.
    /// </summary>
    public BackgroundData BackgroundData { get; set; } = new BackgroundData();
    /// <summary>
    /// Initializes a new instance of the <see cref="Metadata"/> class.
    /// </summary>
    public Metadata()
    {

    }
}

/// <summary>
/// Represents the background data for a BeatBlock level.
/// </summary>
public record class BackgroundData
{
    /// <summary>
    /// Gets or sets the red channel color.
    /// </summary>
    public Color RedChannel { get; set; } = Color.Red;
    /// <summary>
    /// Gets or sets the green channel color.
    /// </summary>
    public Color GreenChannel { get; set; } = Color.Green;
    /// <summary>
    /// Gets or sets the blue channel color.
    /// </summary>
    public Color BlueChannel { get; set; } = Color.Blue;
    /// <summary>
    /// Gets or sets the yellow channel color.
    /// </summary>
    public Color YellowChannel { get; set; } = Color.Yellow;
    /// <summary>
    /// Gets or sets the magenta channel color.
    /// </summary>
    public Color MagentaChannel { get; set; } = Color.Magenta;
    /// <summary>
    /// Gets or sets the cyan channel color.
    /// </summary>
    public Color CyanChannel { get; set; } = Color.Cyan;
    /// <summary>
    /// Gets or sets a value indicating whether the Cranky effect is hidden.
    /// </summary>
    public bool HideCranky { get; set; } = false;
    /// <summary>
    /// Gets or sets the background image reference.
    /// </summary>
    public FileReference Image { get; set; } = new FileReference();
    /// <summary>
    /// Gets or sets the results image reference.
    /// </summary>
    public FileReference ResultsImage { get; set; } = new FileReference();
    /// <summary>
    /// Initializes a new instance of the <see cref="BackgroundData"/> class.
    /// </summary>
    public BackgroundData()
    {
    }
}
/// <summary>
/// Represents a chart variant in a BeatBlock level.
/// </summary>
public class Chart :
    OrderedEventCollection<IBaseEvent, EventType, TickTime>,
    IChart<TickTime>
{
    internal bool isDefault = false;
    /// <summary>
    /// Gets or sets the charter of the chart.
    /// </summary>
    public string Charter { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the difficulty of the chart.
    /// </summary>
    public float Difficulty { get; set; }
    /// <summary>
    /// Gets or sets the display name of the chart.
    /// </summary>
    public string Display { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets a value indicating whether the chart is an extra chart.
    /// </summary>
    public bool Extra { get; set; } = false;
    /// <summary>
    /// Gets or sets a value indicating whether the chart is hidden.
    /// </summary>
    public bool Hidden { get; set; } = false;
    /// <summary>
    /// Gets or sets the name of the chart.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    IBeatCalculator<TickTime> IChart<TickTime>.Calculator => Calculator;
    /// <summary>
    /// Gets the beat calculator for the level.
    /// </summary>
    public BeatCalculator Calculator { get; } = new BeatCalculator();
    /// <summary>
    /// Gets the slot index of this chart in the parent collection.
    /// </summary>
    public int Slot => _parent?._variants.IndexOf(this) ?? -1;
    /// <summary>
    /// Gets a value indicating whether the chart is using the default level data.
    /// </summary>
    [MemberNotNullWhen(false, nameof(LevelFile))]
    public bool IsUsingDefaultLevel { get; private set; } = true;
    /// <summary>
    /// Gets the format version of the chart.
    /// </summary>
    public int FormatVersion => DefaultVersion;
    /// <summary>
    /// Gets or sets the level file path for this chart variant.
    /// </summary>
    public string? LevelFile { get; set; }
    internal Chart() { }
    /// <summary>
    /// Initializes a new instance of the <see cref="Chart"/> class with the specified name.
    /// </summary>
    /// <param name="name">The name of the chart.</param>
    public Chart(string name)
    {
        Name = name;
    }
    /// <inheritdoc/>
	protected override ReadOnlyEnumCollection<EventType> Types => EventTypeRegistry.ToEnums<IChartEvent>();

    internal ChartCollection? _parent;
    /// <inheritdoc/>
    protected override TickTime CreateInstance(float beat) => new(beat);

    /// <inheritdoc/>
	protected override ITickRange<TickTime> CreateRange(float? start, float? end) => new Range(start is null ? null : new TickTime( start.Value), end is null ? null : new TickTime(end.Value));

    /// <inheritdoc/>
	protected override ReadOnlyEnumCollection<EventType> TypesOf<TTarget>()
    {
        return EventTypeRegistry.ToEnums(typeof(TTarget));
    }
    /// <summary>
    /// Separates this chart from the default level, copying shared events to create an independent chart.
    /// </summary>
    [MemberNotNull(nameof(LevelFile))]
    public void SeparateFromDefaultLevel()
    {
        if (_parent == null)
            throw new InvalidOperationException($"Variant '{Name}' is not associated with any level.");
        if (!IsUsingDefaultLevel)
            throw new InvalidOperationException($"Variant '{Name}' is already separated from the default level.");
        foreach(var e in _parent.Default)
        {
            if (!Contains(e) && e is not IChartEvent && e is BaseEvent eb)
                Add(eb with { });
        }
        IsUsingDefaultLevel = false;
        LevelFile = $"level-{Name}.json";
    }
    /// <inheritdoc/>
    public override bool Add(IBaseEvent item)
    {
        if (isDefault && item is IChartEvent)
            return false;
        return base.Add(item);
    }
    /// <inheritdoc/>
    public override bool Remove(IBaseEvent item)
    {        
        if (isDefault && item is IChartEvent)
            return false;
        return base.Remove(item);
    }
    /// <inheritdoc/>
    public override bool Contains(IBaseEvent item)
    {
        if (isDefault && item is IChartEvent) 
            return false;
        return base.Contains(item);
    }
}
/// <summary>
/// Represents a collection of chart variants in a BeatBlock level.
/// </summary>
public class ChartCollection : ICollection<Chart>
{
    internal Level _parent;
    /// <summary>
    /// Gets the default chart variant.
    /// </summary>
    public Chart Default { get; private set; } = new() { isDefault = true };
    internal readonly List<Chart> _variants = [];
    /// <inheritdoc/>
    public int Count => _variants.Count;
    /// <inheritdoc/>
    public bool IsReadOnly { get; } = false;
    /// <summary>
    /// Gets or sets the chart at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the chart.</param>
    /// <returns>The <see cref="Chart"/> at the specified index.</returns>
    public Chart this[int index]
    {
        get => _variants[index];
        set
        {
            value._parent = this;
            _variants[index] = value;
        }
    }
    /// <summary>
    /// Gets or sets the chart with the specified name.
    /// </summary>
    /// <param name="name">The name of the chart.</param>
    /// <returns>The <see cref="Chart"/> with the specified name.</returns>
    public Chart this[string name]
    {
        get => _variants.FirstOrDefault(v => v.Name == name)
            ?? throw new KeyNotFoundException($"No variant with the name '{name}' was found.");
        set
        {
            var index = _variants.FindIndex(v => v.Name == name);
            value._parent = this;
            if (index == -1)
                _variants.Add(value);
            else
                _variants[index] = value;
        }
    }
    /// <summary>
    /// Tries to get a chart variant by name.
    /// </summary>
    /// <param name="name">The name of the variant to find.</param>
    /// <param name="variant">When this method returns, contains the chart with the specified name, or <see langword="null"/> if no such chart was found.</param>
    /// <returns><see langword="true"/> if a chart with the specified name was found; otherwise, <see langword="false"/>.</returns>
    public bool TryGetVariant(string name, [NotNullWhen(true)] out Chart? variant)
    {
        variant = _variants.FirstOrDefault(v => v.Name == name);
        return variant != null;
    }
    /// <inheritdoc/>
    public ChartCollection(Level level)
    {
        _parent = level;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _variants.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(Chart item)
    {
        return _variants.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(Chart[] array, int arrayIndex)
    {
        _variants.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public bool Remove(Chart item)
    {
        item._parent = null;
        return _variants.Remove(item);
    }

    /// <inheritdoc/>
    public IEnumerator<Chart> GetEnumerator()
    {
        return _variants.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <inheritdoc/>
    public void Add(Chart item)
    {
        item._parent = this;
        _variants.Add(item);
    }
}