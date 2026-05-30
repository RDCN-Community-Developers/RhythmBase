namespace RhythmBase.Adofai.Components;

/// <summary>
/// Represents a gradient configuration for particle effects.
/// </summary>
public class Gradient
{
	/// <summary>
	/// Gets or sets the mode of the gradient.
	/// </summary>
	public GradientMode Mode { get; set; } = GradientMode.Blend;

	/// <summary>
	/// Gets or sets the alpha keys for the gradient.
	/// </summary>
	public List<AlphaKey> AlphaKeys { get; set; } = [];

	/// <summary>
	/// Gets or sets the color keys for the gradient.
	/// </summary>
	public List<ColorKey> ColorKeys { get; set; } = [];
}
