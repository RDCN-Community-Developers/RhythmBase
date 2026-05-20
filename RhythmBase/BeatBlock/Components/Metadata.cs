using RhythmBase.BeatBlock.Events;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

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
    public RDColor RedChannel { get; set; } = RDColor.Red;
    public RDColor GreenChannel { get; set; } = RDColor.Green;
    public RDColor BlueChannel { get; set; } = RDColor.Blue;
    public RDColor YellowChannel { get; set; } = RDColor.Yellow;
    public RDColor MagentaChannel { get; set; } = RDColor.Magenta;
    public RDColor CyanChannel { get; set; } = RDColor.Cyan;
    public bool HideCranky { get; set; } = false;
    public FileReference Image { get; set; } = new FileReference();
    public FileReference ResultsImage { get; set; } = new FileReference();
    public BackgroundData()
    {
    }
}
public class Variant : OrderedEventCollection<IBaseEvent, EventType, BBBeat>
{
    internal bool isDefault = false;
    public string Charter { get; set; } = string.Empty;
    public int Difficulty { get; set; }
    public string Display { get; set; } = string.Empty;
    public bool Extra { get; set; } = false;
    public bool Hidden { get; set; } = false;
    public string Name { get; set; }
    public int Slot => _parent?._variants.IndexOf(this) ?? -1;
    [MemberNotNullWhen(false, nameof(LevelFile))]
    public bool IsUsingDefaultLevel { get; private set; } = true;
    public int FormatVersion => DefaultVersionBeatBlock;
    public string? LevelFile { get; set; }
    internal Variant() { }
    public Variant(string name)
    {
        Name = name;
    }
    internal override ReadOnlyEnumCollection<EventType> Types => Utils.EventTypeUtils.ToEnums<IChartEvent>();

    internal VariantCollection? _parent;
    internal override BBBeat CreateInstance(float beat) => new BBBeat(beat);

    internal override IBeatRange<BBBeat> CreateRange(float? start, float? end) => new BBRange(start, end);

    internal override ReadOnlyEnumCollection<EventType> TypesOf<TTarget>()
    {
        return Utils.EventTypeUtils.ToEnums(typeof(TTarget));
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
public class VariantCollection : ICollection<Variant>
{
    internal BBLevel _parent;
    public Variant Default { get; private set; } = new() { isDefault = true };
    internal readonly List<Variant> _variants = [];
    /// <inheritdoc/>
    public int Count => _variants.Count;
    /// <inheritdoc/>
    public bool IsReadOnly { get; } = false;
    public Variant this[int index]
    {
        get => _variants[index];
        set
        {
            value._parent = this;
            _variants[index] = value;
        }
    }
    public Variant this[string name]
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
    public bool TryGetVariant(string name, [NotNullWhen(true)] out Variant? variant)
    {
        variant = _variants.FirstOrDefault(v => v.Name == name);
        return variant != null;
    }
    /// <inheritdoc/>
    public VariantCollection(BBLevel level)
    {
        _parent = level;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _variants.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(Variant item)
    {
        return _variants.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(Variant[] array, int arrayIndex)
    {
        _variants.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public bool Remove(Variant item)
    {
        item._parent = null;
        return _variants.Remove(item);
    }

    /// <inheritdoc/>
    public IEnumerator<Variant> GetEnumerator()
    {
        return _variants.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <inheritdoc/>
    public void Add(Variant item)
    {
        item._parent = this;
        _variants.Add(item);
    }
}