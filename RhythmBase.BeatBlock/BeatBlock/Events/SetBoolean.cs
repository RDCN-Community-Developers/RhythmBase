namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Set Boolean
/// </summary>
/// <remarks>
/// Sets a boolean variable
/// </remarks>
[JsonObjectSerializable]
public record class SetBoolean : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.SetBoolean;
    /// <summary>
    /// Variable to set (must be a child of cs)
    /// </summary>
    public string Var { get; set; } = string.Empty;
    /// <summary>
    /// The value
    /// </summary>
    public bool Enable { get; set; }
}
