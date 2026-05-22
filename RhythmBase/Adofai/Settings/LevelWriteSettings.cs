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
    /// <inheritdoc/>
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
    /// <inheritdoc/>
    public bool LoadAssets { get; set; } = false;
    /// <inheritdoc/>
    public bool EnableUnsafeRelaxedJsonEscaping { get; set; } = true;
    /// <inheritdoc/>
    public InactiveEventsHandling InactiveEventsHandling { get; set; } = InactiveEventsHandling.Retain;
    /// <inheritdoc/>
    public List<IBaseEvent> InactiveEvents { get; set; } = [];
    /// <inheritdoc/>
    public UnreadableEventHandling UnreadableEventsHandling { get; set; } = UnreadableEventHandling.ThrowException;
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
    /// <inheritdoc/>
    public bool AlignIndented { get; set; } = true;
    ///// <summary>
    ///// Raises the <see cref="BeforeWriting"/> event before writing begins.
    ///// </summary>
    //public void OnBeforeWriting()
    //{
    //    BeforeWriting?.Invoke(this, EventArgs.Empty);
    //}
    ///// <summary>
    ///// Raises the <see cref="AfterWriting"/> event after writing completes.
    ///// </summary>
    //public void OnAfterWriting()
    //{
    //    AfterWriting?.Invoke(this, EventArgs.Empty);
    //}
}