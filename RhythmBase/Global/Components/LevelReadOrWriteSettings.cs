using System.Text.Json;
namespace RhythmBase.Global.Components;

/// <summary>
/// Controls how ZIP archives are processed during import.
/// </summary>
public enum ZipFileProcessMethod
{
    /// <summary>
    /// Only process the primary level file inside the archive.
    /// </summary>
    LevelFileOnly,
    /// <summary>
    /// Process all files contained in the archive (including nested files).
    /// </summary>
    AllFiles,
}
public class UnreadableEventArgs(JsonElement Item, string Reason) : EventArgs
{
    /// <summary>
    /// The JSON element representing the unreadable event.
    /// </summary>
    public JsonElement Item { get; } = Item;
    /// <summary>
    /// A short explanation of why the event could not be parsed.
    /// </summary>
    public string Reason { get; } = Reason;
}
public class InactiveEventArgs(IEvent Item) : EventArgs
{
    /// <summary>
    /// The inactive event that was encountered.
    /// </summary>
    public IEvent Item { get; } = Item;
}
public class FileReferenceArgs(FileReference Reference) : EventArgs
{
    /// <summary>
    /// The file reference that was encountered.
    /// </summary>
    public FileReference Reference { get; } = Reference;
}
/// <summary>
/// Settings used when reading or writing level data.
/// </summary>
/// <remarks>
/// Controls JSON formatting, asset preloading, handling of inactive or unreadable events,
/// collection of file references, and arbitrary custom data. Implementations are expected
/// to honor these settings during import/export operations.
/// </remarks>
public class LevelReadOrWriteSettings
{
    private Dictionary<string, object?> _customData = new();
    /// <summary>
    /// Gets or sets a custom data value by key.
    /// </summary>
    /// <remarks>
    /// Returns <c>null</c> when the key does not exist. Set operations overwrite any existing value.
    /// Implementations may use this indexer to expose additional per-operation metadata.
    /// </remarks>
    /// <param name="key">Custom data key. Must not be <c>null</c>.</param>
    public object? this[string key]
    {
        get => _customData.TryGetValue(key, out object? value) ? value : null;
        set => _customData[key] = value;
    }
    /// <summary>
    /// When <c>true</c>, referenced resources (images, audio, etc.) will be preloaded during read operations.
    /// </summary>
    /// <remarks>
    /// Enabling this may increase read time and memory usage but ensures assets are available immediately
    /// after import for consumers that require them.
    /// </remarks>
    public bool LoadAssets { get; set; }
    /// <summary>
    /// Specifies how inactive events should be treated during read/write operations.
    /// </summary>
    /// <remarks>
    /// Typical values include retaining inactive events, discarding them, or storing them for later inspection.
    /// Default behavior is to retain them unless otherwise configured.
    /// </remarks>
    public InactiveEventsHandling InactiveEventsHandling { get; set; }
    /// <summary>
    /// Defines how unreadable events (malformed or unknown data) are handled.
    /// </summary>
    /// <remarks>
    /// Common behaviors are to throw an exception, ignore the event, or store the raw JSON for later analysis.
    /// Default is to throw an exception unless configured otherwise.
    /// </remarks>
    public UnreadableEventHandling UnreadableEventsHandling { get; set; }
    /// <summary>
    /// Apply handling logic for a single inactive event according to <see cref="InactiveEventsHandling"/>.
    /// </summary>
    /// <param name="level">The level containing the inactive event.</param>
    /// <param name="item">The inactive event to process.</param>
    /// <returns><c>true</c> if the event was handled (stored, removed, or otherwise processed); otherwise <c>false</c>.</returns>
    public void OnInactiveEventEncountered(ILevel level, IEvent item)
        => InactiveEventEncountered?.Invoke(level, new InactiveEventArgs(item));
    /// <summary>
    /// Handle a single unreadable JSON event according to <see cref="UnreadableEventsHandling"/>.
    /// </summary>
    /// <param name="level">The level containing the unreadable event.</param>
    /// <param name="item">The JSON element representing the unreadable event.</param>
    /// <param name="reason">A short explanation why the event could not be parsed.</param>
    /// <remarks>
    /// When configured to store unreadable events, implementations should add an entry to <see cref="UnreadableEvents"/>.
    /// </remarks>
    public void OnUnreadableEventEncountered(ILevel level, JsonElement item, string reason)
        => UnreadableEventEncountered?.Invoke(level, new UnreadableEventArgs(item, reason));
    public void OnFileReferenceEncountered(ILevel level, FileReference reference)
        => FileReferenceEncountered?.Invoke(level, new FileReferenceArgs(reference));
    public event EventHandler<InactiveEventArgs>? InactiveEventEncountered;
    public event EventHandler<UnreadableEventArgs>? UnreadableEventEncountered;
    public event EventHandler<FileReferenceArgs>? FileReferenceEncountered;
}
/// <summary>
/// Level export settings.
/// </summary>
public class LevelWriteSettings : LevelReadOrWriteSettings
{
    /// <summary>
    /// When <c>true</c>, JSON serialization will allow certain characters to remain unescaped (unsafe relaxed escaping).
    /// </summary>
    /// <remarks>
    /// This may improve human readability and compatibility with some consumers but can introduce security risks
    /// (for example XSS) if untrusted data is serialized. Validate and sanitize input when enabling this flag.
    /// </remarks>
    public bool EnableUnsafeRelaxedJsonEscaping { get; set; }
    /// <summary>
    /// When <c>true</c>, JSON is written with human-friendly line breaks and spacing.
    /// </summary>
    /// <remarks>
    /// Improves readability for most structures. Small inline values and very large lists/arrays
    /// may be left compact (no per-item indentation) to avoid excessive whitespace and keep
    /// compact data readable; this option focuses on human-friendly output rather than forcing
    /// uniform indentation for every tiny or very large element.
    /// </remarks>
    public bool WriteIndented { get; set; }
    /// <summary>
    /// When <c>true</c>, property values within arrays/objects are aligned for improved human readability.
    /// </summary>
    /// <remarks>
    /// Alignment attempts to line up numeric and string values across elements (column-style alignment).
    /// The concrete serializer uses a custom no-indent writer (see <see cref="NoIndentScope"/>)
    /// to produce aligned output without losing control of structural formatting. Enabling alignment may increase
    /// output size and complexity of the write operation.
    /// </remarks>
    public bool WriteAligned { get; set; }
}
/// <summary>
/// Level import settings.
/// </summary>
public class LevelReadSettings : LevelReadOrWriteSettings
{
    /// <summary>
    /// Gets or sets the method used to process zip files.
    /// </summary>
    public ZipFileProcessMethod ZipFileProcessMethod { get; set; }
}