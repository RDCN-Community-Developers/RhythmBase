namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Toggle Particles
/// </summary>
/// <remarks>
/// Toggle whether various particle effects are spawned in by notes
/// </remarks>
[RDJsonObjectSerializable]
public record class ToggleParticles : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.ToggleParticles;
    /// <summary>
    /// Block/Hold particles
    /// </summary>
    public bool Block { get; set; }
    /// <summary>
    /// Miss particles
    /// </summary>
    public bool Miss { get; set; }
    /// <summary>
    /// Mine particles
    /// </summary>
    public bool Mine { get; set; }
    /// <summary>
    /// MineHold particles
    /// </summary>
    public bool MineHold { get; set; }
    /// <summary>
    /// MineHold particles (on hit)
    /// </summary>
    public bool MineHoldHit { get; set; }
    /// <summary>
    /// MineHold tail particle
    /// </summary>
    public bool MineHoldEnd { get; set; }
    /// <summary>
    /// Side particles
    /// </summary>
    public bool Side { get; set; }
}
