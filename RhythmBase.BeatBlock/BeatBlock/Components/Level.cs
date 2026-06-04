using RhythmBase.BeatBlock.Events;

namespace RhythmBase.BeatBlock.Components;

/// <summary>
/// BeatBlock level.
/// </summary>
public partial class Level :
    //OrderedEventCollection<IBaseEvent, EventType, BBBeat>,
    IArchiveLevel<Level, IBaseEvent, EventType, TickTime>,
    IMultiFileLevel<Level, IBaseEvent, EventType, TickTime>,
    IDisposable
{
    internal bool isZip;
    internal bool isExtracted;
    ///<inheritdoc/>
    public static Level Default => new Level();
    /// <summary>
    /// Gets or sets the properties of the level.
    /// </summary>
    public Properties Properties { get; set; } = new Properties();
    /// <summary>
    /// Gets or sets the default variant of the level.
    /// </summary>
    public string? DefaultVariant { get; set; }
    /// <summary>
    /// Gets or sets the metadata of the level.
    /// </summary>
    public Metadata Metadata { get; set; } = new Metadata();
    public Dictionary<string, TagEventCollection> TagEvents { get; } = [];
    /// <summary>
    /// Gets the collection of variants for the level.
    /// </summary>
    public ChartCollection Variants { get; }
    /// <summary>
    /// Gets the resolved path of the level file.
    /// </summary>
    public string ResolvedPath { get; internal set; } = string.Empty;
    /// <summary>
    /// Gets the file path of the level.
    /// </summary>
    public string? Filepath { get; internal set; }
    /// <summary>
    /// Gets the resolved directory of the level file.
    /// </summary>
    public string? ResolvedDirectory { get; internal set; }
    public Level()
    {
        Variants = new ChartCollection(this);
    }
    protected internal void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Dispose managed resources here
        }
        // Dispose unmanaged resources here
    }

    /// <summary>
    /// Releases all resources used by the <see cref="Level"/>.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
/// <summary>
/// Provides methods for calculating beats and time spans in a <see cref="Level"/>.
/// </summary>
public class BeatCalculator : IBeatCalculator<TickTime>
{
    /// <summary>
    /// Creates a <see cref="TickTime"/> from a beat value.
    /// </summary>
    /// <param name="beatOnly">The beat value.</param>
    /// <returns>A <see cref="TickTime"/> representing the specified beat.</returns>
    public TickTime BeatOf(float beatOnly)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Creates a <see cref="TickTime"/> from a <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="timeSpan">The time span.</param>
    /// <returns>A <see cref="TickTime"/> representing the specified time span.</returns>
    public TickTime BeatOf(TimeSpan timeSpan)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Converts a beat value to a <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="beat">The beat value.</param>
    /// <returns>The <see cref="TimeSpan"/> representing the beat.</returns>
    public TimeSpan BeatOnlyToTimeSpan(float beat)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Gets the beats per minute at the specified beat.
    /// </summary>
    /// <param name="beat">The beat.</param>
    /// <returns>The beats per minute.</returns>
    public float BeatsPerMinuteOf(TickTime beat)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Calculates the beats for the specified level.
    /// </summary>
    /// <param name="level">The level to calculate beats for.</param>
    public void CalculateBeats(Level level)
    {
        // Implement beat calculation logic here
    }
    /// <summary>
    /// Gets the interval between two beats.
    /// </summary>
    /// <param name="beat1">The first beat.</param>
    /// <param name="beat2">The second beat.</param>
    /// <returns>The interval between the two beats.</returns>
    public ITickRange<TickTime> IntervalOf(TickTime beat1, TickTime beat2)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Gets the interval between two <see cref="TimeSpan"/> values.
    /// </summary>
    /// <param name="timeSpan1">The first time span.</param>
    /// <param name="timeSpan2">The second time span.</param>
    /// <returns>The interval between the two time spans.</returns>
    public ITickRange<TickTime> IntervalOf(TimeSpan timeSpan1, TimeSpan timeSpan2)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Refreshes the calculator state.
    /// </summary>
    public void Refresh()
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to a beat value.
    /// </summary>
    /// <param name="timeSpan">The time span.</param>
    /// <returns>The beat value.</returns>
    public float TimeSpanToBeatOnly(TimeSpan timeSpan)
    {
        throw new NotImplementedException();
    }
}
