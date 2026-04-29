namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Shader Background
/// </summary>
/// <remarks>
/// No description
/// </remarks>
public record class ShaderBackground : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.ShaderBackground;
    /// <summary>
    /// Which effect canvas to draw on
    /// </summary>
    public EffectCanvasTypeWdisable? EffectCanvasType { get; set; } 
    /// <summary>
    /// shader code
    /// </summary>
    public string ShaderCode { get; set; } = string.Empty;
    /// <summary>
    /// Uniform Code
    /// </summary>
    public string UniformCode { get; set; } = string.Empty;
}
