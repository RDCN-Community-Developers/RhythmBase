namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Subtypes of sound effects.
/// </summary>
public class SoundSubType : Audio
{
	/// <summary>
	/// Gets or sets the sound effect name.
	/// </summary>
	public SoundType GroupSubtype { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="SoundSubType"/> is used.
	/// </summary>
	public bool Used { get; set; }
}