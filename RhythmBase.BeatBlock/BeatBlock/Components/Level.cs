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
	public static Level Default => new();
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
	/// <summary>
	/// Gets the dictionary of tag event collections for the level.
	/// </summary>
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
	/// <summary>
	/// Initializes a new instance of the <see cref="Level"/> class.
	/// </summary>
	public Level()
	{
		Variants = new ChartCollection(this);
	}
	/// <summary>
	/// Releases the unmanaged resources used by the <see cref="Level"/> and optionally releases the managed resources.
	/// </summary>
	/// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
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
