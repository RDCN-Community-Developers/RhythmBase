using RhythmBase.Global.Components.Easing;
namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Shader Uniform
/// </summary>
/// <remarks>
/// Eases a shader uniform over time
/// </remarks>
[JsonObjectSerializable]
public record class ShaderUniform : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.ShaderUniform;
    /// <summary>
    /// Uniform to ease
    /// </summary>
    public string Var { get; set; } = string.Empty;
    /// <summary>
    /// Starting value
    /// </summary>
    public float? Start { get; set; }
    /// <summary>
    /// Ending value
    /// </summary>
    public float Value { get; set; }
    /// <summary>
    /// Length of ease
    /// </summary>
    public float? Duration { get; set; }
    /// <summary>
    /// Ease function to use
    /// </summary>
    public EaseType? Ease { get; set; }
    /// <summary>
    /// Times to repeat
    /// </summary>
    public int? Repeats { get; set; }
    /// <summary>
    /// Beats between repeats
    /// </summary>
    public float? RepeatDelay { get; set; }
}
