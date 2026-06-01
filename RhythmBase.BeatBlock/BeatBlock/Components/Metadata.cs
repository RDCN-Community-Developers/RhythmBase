using RhythmBase.BeatBlock.Converters;
using RhythmBase.BeatBlock.Events;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace RhythmBase.BeatBlock.Components;
public record class Metadata
{
    public string SongName { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string ArtistLink { get; set; } = string.Empty;
    public float Bpm { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Charter { get; set; } = string.Empty;
    public int Difficulty { get; set; } = 0;
    public bool LightWarning { get; set; } = false;
    public bool LoopPointsEnable { get; set; } = false;
    public bool LyricsWarning { get; set; } = false;
    public string Source { get; set; } = string.Empty;
    public bool IsBackgroundEnabled { get; set; } = true;
    public int StartLoop { get; set; } = 0;
    public int EndLoop { get; set; } = 0;
    public BackgroundData BackgroundData { get; set; } = new BackgroundData();
    public Metadata()
    {

    }
}

public record class BackgroundData
{
    public Color RedChannel { get; set; } = Color.Red;
    public Color GreenChannel { get; set; } = Color.Green;
    public Color BlueChannel { get; set; } = Color.Blue;
    public Color YellowChannel { get; set; } = Color.Yellow;
    public Color MagentaChannel { get; set; } = Color.Magenta;
    public Color CyanChannel { get; set; } = Color.Cyan;
    public bool HideCranky { get; set; } = false;
    public FileReference Image { get; set; } = new FileReference();
    public FileReference ResultsImage { get; set; } = new FileReference();
    public BackgroundData()
    {
    }
}
public class Chart :
    OrderedEventCollection<IBaseEvent, EventType, TickTime>,
    IChart<TickTime>
{
    internal bool isDefault = false;
    public string Charter { get; set; } = string.Empty;
    public int Difficulty { get; set; }
    public string Display { get; set; } = string.Empty;
    public bool Extra { get; set; } = false;
    public bool Hidden { get; set; } = false;
    public string Name { get; set; } = string.Empty;
    IBeatCalculator<TickTime> IChart<TickTime>.Calculator => Calculator;
    /// <summary>
    /// Gets the beat calculator for the level.
    /// </summary>
    public BeatCalculator Calculator { get; } = new BeatCalculator();
    public int Slot => _parent?._variants.IndexOf(this) ?? -1;
    [MemberNotNullWhen(false, nameof(LevelFile))]
    public bool IsUsingDefaultLevel { get; private set; } = true;
    public int FormatVersion => DefaultVersionBeatBlock;
    public string? LevelFile { get; set; }
    internal Chart() { }
    public Chart(string name)
    {
        Name = name;
    }
	protected override ReadOnlyEnumCollection<EventType> Types => ClassEnumMap.ToEnums<IChartEvent>();

    internal ChartCollection? _parent;
    protected override TickTime CreateInstance(float beat) => new TickTime(beat);

	protected override ITickRange<TickTime> CreateRange(float? start, float? end) => new BBRange(start, end);

	protected override ReadOnlyEnumCollection<EventType> TypesOf<TTarget>()
    {
        return ClassEnumMap.ToEnums(typeof(TTarget));
    }
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
    public override bool Add(IBaseEvent item)
    {
        if (isDefault && item is IChartEvent)
            return false;
        return base.Add(item);
    }
    public override bool Remove(IBaseEvent item)
    {        
        if (isDefault && item is IChartEvent)
            return false;
        return base.Remove(item);
    }
    public override bool Contains(IBaseEvent item)
    {
        if (isDefault && item is IChartEvent) 
            return false;
        return base.Contains(item);
    }
}
public class ChartCollection : ICollection<Chart>
{
    internal Level _parent;
    public Chart Default { get; private set; } = new() { isDefault = true };
    internal readonly List<Chart> _variants = [];
    /// <inheritdoc/>
    public int Count => _variants.Count;
    /// <inheritdoc/>
    public bool IsReadOnly { get; } = false;
    public Chart this[int index]
    {
        get => _variants[index];
        set
        {
            value._parent = this;
            _variants[index] = value;
        }
    }
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