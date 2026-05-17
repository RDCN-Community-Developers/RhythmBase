using RhythmBase.Adofai.Components;
using RhythmBase.Adofai.Events;
using System.Text.Json;

namespace RhythmBase.Adofai.Settings;

/// <summary>
/// Level import settings.
/// </summary>
public record LevelWriteSettings : ILevelWriteSettings<IBaseEvent, EventType, ADBeat>
{
    private readonly Dictionary<string, object> CustomData = [];
    /// <summary>
    /// Event triggered before writing.
    /// </summary>
    public event EventHandler? BeforeWriting;
    /// <summary>
    /// Event triggered after writing.
    /// </summary>
    public event EventHandler? AfterWriting;
    /// <summary>
    /// Gets or sets the value associated with the specified key in the custom data dictionary.
    /// </summary>
    /// <remarks>If the key does not exist in the dictionary, the getter returns null. The setter will overwrite any
    /// existing value associated with the key.</remarks>
    /// <param name="key">The key used to access the value in the custom data dictionary. Must not be null.</param>
    /// <returns>The value associated with the specified key if it exists; otherwise, null.</returns>
    public object? this[string key]
    {
        get => CustomData.TryGetValue(key, out var value) ? value : null;
        set => CustomData[key] = value!;
    }
    /// <summary>
    /// Initialize.
    /// </summary>
    public LevelWriteSettings()
    {
    }
    /// <summary>
    /// Enable resource preloading. This may grow read times. 
    /// Defaults to <see langword="false" />.
    /// </summary>
    public bool LoadAssets { get; set; } = false;
    /// <summary>
    /// Gets or sets a value indicating whether to enable unsafe relaxed JSON escaping during serialization.
    /// </summary>
    /// <remarks>When set to <see langword="true"/>, this property allows certain characters in JSON strings to be
    /// serialized without escaping, which may improve readability or compatibility with some consumers. However, enabling
    /// this option can introduce security risks, such as cross-site scripting (XSS) vulnerabilities, if untrusted data is
    /// serialized. Use with caution and ensure that all data is properly validated and sanitized before
    /// serialization.</remarks>
    public bool EnableUnsafeRelaxedJsonEscaping { get; set; } = true;
    /// <summary>
    /// Action on inactive items on reads or writes.
    /// Defaults to <see cref="F:RhythmBase.Global.Settings.InactiveEventsHandling.Retain" />.
    /// </summary>
    public InactiveEventsHandling InactiveEventsHandling { get; set; } = InactiveEventsHandling.Retain;
    /// <summary>
    /// Stores unreadable event data when the <see cref="P:RhythmBase.Global.Settings.LevelReadOrWriteSettings.InactiveEventsHandling" /> is <see cref="F:RhythmBase.Global.Settings.InactiveEventsHandling.Store" />.
    /// </summary>
    public List<IBaseEvent> InactiveEvents { get; set; } = [];
    /// <summary>
    /// Action on unreadable events.
    /// Defaults to <see cref="F:RhythmBase.Global.Settings.UnreadableEventHandling.ThrowException" />.
    /// </summary>
    public UnreadableEventHandling UnreadableEventsHandling { get; set; } = UnreadableEventHandling.ThrowException;
    /// <summary>
    /// Gets the collection of file references associated with this instance.
    /// </summary>
    /// <remarks>The returned collection is read-only and reflects the current set of file references.
    /// Modifications to the collection itself are not supported; to update the set of file references, use the
    /// appropriate methods provided by the class.</remarks>
    public HashSet<FileReference> FileReferences { get; } = [];
    /// <summary>
    /// Stores unreadable event data when the <see cref="P:RhythmBase.Global.Settings.LevelReadOrWriteSettings.UnreadableEventsHandling" /> is <see cref="F:RhythmBase.Global.Settings.UnreadableEventHandling.Store" />.
    /// </summary>
    /// <returns></returns>
    public List<(JsonElement item, string reason)> UnreadableEvents { get; set; } = [];
    public bool HandleInactiveEvent(IBaseEvent item)
    {
        switch (InactiveEventsHandling)
        {
            case InactiveEventsHandling.Store:
                InactiveEvents.Add(item);
                break;
            case InactiveEventsHandling.Retain:
                return false;
        }
        return true;
    }
    public void HandleUnreadableEvent(JsonElement item, string reason)
    {
        switch (UnreadableEventsHandling)
        {
            case UnreadableEventHandling.ThrowException:
                throw new InvalidOperationException($"Unreadable event: {reason}");
            case UnreadableEventHandling.Store:
                UnreadableEvents.Add((item, reason));
                break;
        }
    }
    /// <summary>
    /// Use indentation. 
    /// Defaults to <see langword="true" />.
    /// </summary>
    public bool Indented { get; set; } = true;
    public void OnBeforeWriting()
    {
        BeforeWriting?.Invoke(this, EventArgs.Empty);
    }
    public void OnAfterWriting()
    {
        AfterWriting?.Invoke(this, EventArgs.Empty);
    }
}