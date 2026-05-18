using RhythmBase.BeatBlock.Events;
using RhythmBase.BeatBlock.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace RhythmBase.BeatBlock.Settings;


/// <summary>
/// Level import settings.
/// </summary>
public record LevelReadSettings : ILevelReadSettings<IBaseEvent, EventType, BBBeat>
{
    private readonly Dictionary<string, object> CustomData = [];
    /// <summary>
    /// Event triggered before reading.
    /// </summary>
    public event EventHandler? BeforeReading;
    /// <summary>
    /// Event triggered after reading.
    /// </summary>
    public event EventHandler? AfterReading;
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
    public LevelReadSettings()
    {
    }
    /// <summary>
    /// Enable resource preloading. This may grow read times. 
    /// Defaults to <see langword="false" />.
    /// </summary>
    public bool LoadAssets { get; set; } = false;
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
    /// Gets or sets the method used to process zip files.
    /// </summary>
    public ZipFileProcessMethod ZipFileProcessMethod { get; set; } = ZipFileProcessMethod.AllFiles;
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
    /// <summary>
    /// Handles an inactive event according to the current settings.
    /// </summary>
    /// <param name="item">The inactive event to handle.</param>
    /// <returns><see langword="true"/> if the event should be skipped; otherwise, <see langword="false"/>.</returns>
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
    /// <summary>
    /// Handles an unreadable event according to the current settings.
    /// </summary>
    /// <param name="item">The unreadable event data.</param>
    /// <param name="reason">The reason why the event is unreadable.</param>
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
    /// Called before reading begins.
    /// </summary>
    public void OnBeforeReading()
    {
        BeforeReading?.Invoke(this, EventArgs.Empty);
    }
    /// <summary>
    /// Called after reading completes.
    /// </summary>
    public void OnAfterReading()
    {
        AfterReading?.Invoke(this, EventArgs.Empty);
    }
}