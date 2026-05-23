using RhythmBase.RhythmDoctor.Events;
using System.Text.Json;
namespace RhythmBase.Global.Settings;

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
/// <summary>
/// Settings used when reading or writing level data.
/// </summary>
/// <remarks>
/// Controls JSON formatting, asset preloading, handling of inactive or unreadable events,
/// collection of file references, and arbitrary custom data. Implementations are expected
/// to honor these settings during import/export operations.
/// </remarks>
/// <typeparam name="TEvent">Event type stored in the level (implements <see cref="IEvent{TType, TBeat}"/>).</typeparam>
/// <typeparam name="TType">Enum type that identifies event kinds.</typeparam>
/// <typeparam name="TBeat">Beat type used by events (implements <see cref="IBeat{TBeat}"/>).</typeparam>
public interface ILevelReadOrWriteSettings<TEvent, TType, TBeat>
  where TEvent : IEvent<TType, TBeat>
  where TType : struct, Enum
  where TBeat : struct, IBeat<TBeat>
{
    /// <summary>
    /// Gets or sets a custom data value by key.
    /// </summary>
    /// <remarks>
    /// Returns <c>null</c> when the key does not exist. Set operations overwrite any existing value.
    /// Implementations may use this indexer to expose additional per-operation metadata.
    /// </remarks>
    /// <param name="key">Custom data key. Must not be <c>null</c>.</param>
    object? this[string key] { get; set; }
    /// <summary>
    /// When <c>true</c>, referenced resources (images, audio, etc.) will be preloaded during read operations.
    /// </summary>
    /// <remarks>
    /// Enabling this may increase read time and memory usage but ensures assets are available immediately
    /// after import for consumers that require them.
    /// </remarks>
    bool LoadAssets { get; set; }
    /// <summary>
    /// Specifies how inactive events should be treated during read/write operations.
    /// </summary>
    /// <remarks>
    /// Typical values include retaining inactive events, discarding them, or storing them for later inspection.
    /// Default behavior is to retain them unless otherwise configured.
    /// </remarks>
    InactiveEventsHandling InactiveEventsHandling { get; set; }
    /// <summary>
    /// When <see cref="InactiveEventsHandling"/> is set to store, unreadable or inactive events are added here.
    /// </summary>
    /// <remarks>
    /// Implementations should populate this list only when configured to store such events; otherwise it may remain empty.
    /// </remarks>
    List<TEvent> InactiveEvents { get; set; }
    /// <summary>
    /// Defines how unreadable events (malformed or unknown data) are handled.
    /// </summary>
    /// <remarks>
    /// Common behaviors are to throw an exception, ignore the event, or store the raw JSON for later analysis.
    /// Default is to throw an exception unless configured otherwise.
    /// </remarks>
    UnreadableEventHandling UnreadableEventsHandling { get; set; }
    /// <summary>
    /// Read-only collection of file references gathered during read/write operations.
    /// </summary>
    /// <remarks>
    /// The collection reflects the files observed or written for this operation. To modify the set, use the
    /// appropriate APIs on the concrete implementation rather than mutating this collection directly.
    /// </remarks>
    HashSet<FileReference> FileReferences { get; }
    /// <summary>
    /// When <see cref="UnreadableEventsHandling"/> is set to store, this collection holds the raw JSON elements
    /// together with a human-readable reason explaining why each item was unreadable.
    /// </summary>
    List<(JsonElement item, string reason)> UnreadableEvents { get; set; }
    /// <summary>
    /// Apply handling logic for a single inactive event according to <see cref="InactiveEventsHandling"/>.
    /// </summary>
    /// <param name="item">The inactive event to process.</param>
    /// <returns><c>true</c> if the event was handled (stored, removed, or otherwise processed); otherwise <c>false</c>.</returns>
    bool HandleInactiveEvent(TEvent item);
    /// <summary>
    /// Handle a single unreadable JSON event according to <see cref="UnreadableEventsHandling"/>.
    /// </summary>
    /// <param name="item">The JSON element representing the unreadable event.</param>
    /// <param name="reason">A short explanation why the event could not be parsed.</param>
    /// <remarks>
    /// When configured to store unreadable events, implementations should add an entry to <see cref="UnreadableEvents"/>.
    void HandleUnreadableEvent(JsonElement item, string reason);
}
/// <summary>
/// Level export settings.
/// </summary>
public interface ILevelWriteSettings<TEvent, TType, TBeat> : ILevelReadOrWriteSettings<TEvent, TType, TBeat>
  where TEvent : IEvent<TType, TBeat>
  where TType : struct, Enum
  where TBeat : struct, IBeat<TBeat>
{
    /// <summary>
    /// When <c>true</c>, JSON serialization will allow certain characters to remain unescaped (unsafe relaxed escaping).
    /// </summary>
    /// <remarks>
    /// This may improve human readability and compatibility with some consumers but can introduce security risks
    /// (for example XSS) if untrusted data is serialized. Validate and sanitize input when enabling this flag.
    /// </remarks>
    bool EnableUnsafeRelaxedJsonEscaping { get; set; }
    /// <summary>
    /// When <c>true</c>, JSON is written with standard indentation (line breaks and spaces) for readability.
    /// </summary>
    /// <remarks>
    /// Indentation increases file size. This flag controls the serializer's Indented option.
    /// </remarks>
    bool WriteIndented { get; set; }
    /// <summary>
    /// When <c>true</c>, property values within arrays/objects are aligned for improved human readability.
    /// </summary>
    /// <remarks>
    /// Alignment attempts to line up numeric and string values across elements (column-style alignment).
    /// The concrete serializer uses a custom no-indent writer (see <see cref="RhythmBase.Global.Converters.NoIndentScope"/>)
    /// to produce aligned output without losing control of structural formatting. Enabling alignment may increase
    /// output size and complexity of the write operation.
    /// </remarks>
    bool WriteAligned { get; set; }
}
/// <summary>
/// Level import settings.
/// </summary>
public interface ILevelReadSettings<TEvent, TType, TBeat> : ILevelReadOrWriteSettings<TEvent, TType, TBeat>
  where TEvent : IEvent<TType, TBeat>
  where TType : struct, Enum
  where TBeat : struct, IBeat<TBeat>
{
    /// <summary>
    /// Gets or sets the method used to process zip files.
    /// </summary>
    ZipFileProcessMethod ZipFileProcessMethod { get; set; }
}