namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Deco Shader
/// </summary>
/// <remarks>
/// Create a new shader that can be applied to decorations
/// </remarks>
[JsonObjectSerializable]
public record class DecorationShader : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.DecorationShader;
    /// <summary>
    /// name of shader
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Is this shader stored in a file, or in the level?
    /// </summary>
    public ShaderSource Source { get; set; }
    /// <summary>
    /// filename of shader
    /// </summary>
    public string File { get; set; } = string.Empty;
    /// <summary>
    /// shader code
    /// </summary>
    public string ShaderCode { get; set; } = string.Empty;
    /// <summary>
    /// Uniform Code
    /// </summary>
    public string UniformCode { get; set; } = string.Empty;
}
