namespace RhythmBase.Adofai.Components;

/// <summary>
/// Represents a color gradient used for particle effects.
/// </summary>
public class ColorGradient
{
	/// <summary>
	/// Gets or sets the first color in the gradient.
	/// </summary>
	public Color Color1 { get; set; } = Color.White;

	/// <summary>
	/// Gets or sets the second color in the gradient.
	/// </summary>
	public Color Color2 { get; set; } = Color.White;

	/// <summary>
	/// Gets or sets the first gradient configuration.
	/// </summary>
	public Gradient Gradient1 { get; set; } = new Gradient();

	/// <summary>
	/// Gets or sets the second gradient configuration.
	/// </summary>
	public Gradient Gradient2 { get; set; } = new Gradient();

	/// <summary>
	/// Gets or sets the mode of the color gradient.
	/// </summary>
	public ColorMode Mode { get; set; } = ColorMode.Color;
}
