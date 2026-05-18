namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Init Object
/// </summary>
/// <remarks>
/// Initializes an object
/// </remarks>
[RDJsonObjectSerializable]
public record class InitObject : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.InitObject;
    /// <summary>
    /// Name of object
    /// </summary>
    public string ObjectName { get; set; } = string.Empty;
    /// <summary>
    /// If set, obj is inited to vfx.objects[variableName].\nAlso allows the object to be inited at level start
    /// </summary>
    public string? VariableName { get; set; } = string.Empty;
    /// <summary>
    /// start hidden
    /// </summary>
    public bool SkipRender { get; set; } 
}
