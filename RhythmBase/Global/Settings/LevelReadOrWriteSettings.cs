using RhythmBase.RhythmDoctor.Events;
using System.Text.Json;
namespace RhythmBase.Global.Settings;

/// <summary>
/// Specifies the method used to process files within a ZIP archive.
/// </summary>
/// <remarks>This enumeration defines the scope of file processing when working with ZIP archives.  Use <see
/// cref="LevelFileOnly"/> to process only the top-level file, or <see cref="AllFiles"/>  to process all files within
/// the archive, including nested files.</remarks>
public enum ZipFileProcessMethod
{
	/// <summary>
	/// Specifies that logging should be restricted to level-specific files only,  without writing to a general 
	/// file.
	/// </summary>
	LevelFileOnly,
	/// <summary>
	/// Represents a collection of all files in the current context.
	/// </summary>
	AllFiles,
}
/// <summary>
/// Level import or export settings.
/// </summary>
public interface ILevelReadOrWriteSettings<TEvent, TType, TBeat>
	where TEvent : IEvent<TType, TBeat>
	where TType : struct, Enum
	where TBeat : struct, IBeat<TBeat>
{
	/// <summary>
	/// Gets or sets the value associated with the specified key in the custom data dictionary.
	/// </summary>
	/// <remarks>If the key does not exist in the dictionary, the getter returns null. The setter will overwrite any
	/// existing value associated with the key.</remarks>
	/// <param name="key">The key used to access the value in the custom data dictionary. Must not be null.</param>
	/// <returns>The value associated with the specified key if it exists; otherwise, null.</returns>
    object? this[string key] { get; set; }
	/// <summary>
	/// Enable resource preloading. This may grow read times. 
	/// Defaults to <see langword="false" />.
	/// </summary>
    bool LoadAssets { get; set; }
	/// <summary>
	/// Action on inactive items on reads or writes.
	/// Defaults to <see cref="F:RhythmBase.Global.Settings.InactiveEventsHandling.Retain" />.
	/// </summary>
	InactiveEventsHandling InactiveEventsHandling { get; set; }
    /// <summary>
    /// Stores unreadable event data when the <see cref="P:RhythmBase.Global.Settings.LevelReadOrWriteSettings.InactiveEventsHandling" /> is <see cref="F:RhythmBase.Global.Settings.InactiveEventsHandling.Store" />.
    /// </summary>
    List<TEvent> InactiveEvents { get; set; }
    /// <summary>
    /// Action on unreadable events.
    /// Defaults to <see cref="F:RhythmBase.Global.Settings.UnreadableEventHandling.ThrowException" />.
    /// </summary>
    UnreadableEventHandling UnreadableEventsHandling { get; set; }
    /// <summary>
    /// Gets the collection of file references associated with this instance.
    /// </summary>
    /// <remarks>The returned collection is read-only and reflects the current set of file references.
    /// Modifications to the collection itself are not supported; to update the set of file references, use the
    /// appropriate methods provided by the class.</remarks>
    HashSet<FileReference> FileReferences { get; }
    /// <summary>
    /// Stores unreadable event data when the <see cref="P:RhythmBase.Global.Settings.LevelReadOrWriteSettings.UnreadableEventsHandling" /> is <see cref="F:RhythmBase.Global.Settings.UnreadableEventHandling.Store" />.
    /// </summary>
    /// <returns></returns>
    List<(JsonElement item, string reason)> UnreadableEvents { get; set; }

    /// <summary>
    /// Handles an inactive event according to the current settings.
    /// </summary>
    /// <param name="item">The inactive event to handle.</param>
    /// <returns>true if the event was handled; otherwise, false.</returns>
    bool HandleInactiveEvent(TEvent item);
    /// <summary>
    /// Handles an unreadable event by storing it or throwing an exception according to the current settings.
    /// </summary>
    /// <param name="item">The JSON element representing the unreadable event.</param>
    /// <param name="reason">The reason why the event was unreadable.</param>
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
    /// Gets or sets a value indicating whether to enable unsafe relaxed JSON escaping during serialization.
    /// </summary>
    /// <remarks>When set to <see langword="true"/>, this property allows certain characters in JSON strings to be
    /// serialized without escaping, which may improve readability or compatibility with some consumers. However, enabling
    /// this option can introduce security risks, such as cross-site scripting (XSS) vulnerabilities, if untrusted data is
    /// serialized. Use with caution and ensure that all data is properly validated and sanitized before
    /// serialization.</remarks>
    bool EnableUnsafeRelaxedJsonEscaping { get; set; }
    /// <summary>
    /// Use indentation. 
    /// Defaults to <see langword="true" />.
    /// </summary>
    bool Indented { get; set; }
    /// <summary>
    /// Invoked after writing is complete. This method can be used to perform any necessary post-processing or cleanup after the writing process has finished.
    /// </summary>
    void OnAfterWriting();
    /// <summary>
    /// Invoked before writing begins. This method can be used to perform any necessary setup or initialization before the writing process starts.
    /// </summary>
    void OnBeforeWriting();
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
	/// <summary>
	/// Event triggered before reading.
	/// </summary>
	event EventHandler? BeforeReading;
	/// <summary>
	/// Event triggered after reading.
	/// </summary>
	event EventHandler? AfterReading;
    /// <summary>
    /// Invoked after reading is complete. This method can be used to perform any necessary post-processing or cleanup after the reading process has finished.
    /// </summary>
    void OnAfterReading();
    /// <summary>
    /// Invoked before reading begins. This method can be used to perform any necessary setup or initialization before the reading process starts.
    /// </summary>
    void OnBeforeReading();
}