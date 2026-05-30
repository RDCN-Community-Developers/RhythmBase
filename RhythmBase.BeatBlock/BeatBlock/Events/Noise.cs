using RhythmBase.BeatBlock.Components;
namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Noise
/// </summary>
/// <remarks>
/// Enables a noise effect on the background
/// </remarks>
[JsonObjectSerializable]
public record class Noise : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.Noise;
    /// <summary>
    /// Chance per pixel, from 0 to 1, to be clear. 0 completely disables.
    /// </summary>
    public float Chance { get; set; }
    /// <summary>
    /// Color index
    /// </summary>
    public ColorIndex Color { get; set; }
    /// <summary>
    /// Time step factor for the noise
    /// </summary>
    public float TimeStep { get; set; }
    /// <summary>
    /// Pixelation factor for the noise
    /// </summary>
    public float Pixelate { get; set; }
}
