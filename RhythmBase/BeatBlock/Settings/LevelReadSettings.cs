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
    /// <inheritdoc/>
    public event EventHandler? BeforeReading;
    /// <inheritdoc/>
    public event EventHandler? AfterReading;
    /// <inheritdoc/>
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
    /// <inheritdoc/>
    public bool LoadAssets { get; set; } = false;
    /// <inheritdoc/>
    public InactiveEventsHandling InactiveEventsHandling { get; set; } = InactiveEventsHandling.Retain;
    /// <inheritdoc/>
    public List<IBaseEvent> InactiveEvents { get; set; } = [];
    /// <inheritdoc/>
    public UnreadableEventHandling UnreadableEventsHandling { get; set; } = UnreadableEventHandling.ThrowException;
    /// <inheritdoc/>
    public ZipFileProcessMethod ZipFileProcessMethod { get; set; } = ZipFileProcessMethod.AllFiles;
    /// <inheritdoc/>
    public HashSet<FileReference> FileReferences { get; } = [];
    /// <inheritdoc/>
    public List<(JsonElement item, string reason)> UnreadableEvents { get; set; } = [];
    /// <inheritdoc/>
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
    /// <inheritdoc/>
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
    ///// <summary>
    ///// Called before reading begins.
    ///// </summary>
    //public void OnBeforeReading()
    //{
    //    BeforeReading?.Invoke(this, EventArgs.Empty);
    //}
    ///// <summary>
    ///// Called after reading completes.
    ///// </summary>
    //public void OnAfterReading()
    //{
    //    AfterReading?.Invoke(this, EventArgs.Empty);
    //}
}