namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event that hides or shows a specific window.
/// </summary>
[JsonObjectSerializable]
public record class HideWindow : BaseWindowEvent
{
  /// <inheritdoc />
  public override EventType Type => EventType.HideWindow;

  /// <summary>
  /// Gets or sets a value indicating whether the target window should be shown.
  /// </summary>
  public bool Show { get; set; }

  /// <summary>
  /// Gets or sets a value indicating whether the target window should be rendered as transparent.
  /// </summary>
  [JsonCondition($"$&.{nameof(Transparent)}")]
  public bool Transparent { get; set; }

  /// <summary>
  /// Gets or sets a value indicating whether the target window should be frameless (no window chrome).
  /// </summary>
  [JsonCondition($"$&.{nameof(Frameless)}")]
  public bool Frameless { get; set; }
}
