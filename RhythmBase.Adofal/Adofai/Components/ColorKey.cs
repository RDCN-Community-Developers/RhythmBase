namespace RhythmBase.Adofai.Components;

/// <summary>
/// Represents a color key in a gradient.
/// </summary>
public struct ColorKey(float time, Color color)
{
	/// <summary>
	/// Gets or sets the time value of the key.
	/// </summary>
	public float Time { get; set; } = time;

	/// <summary>
	/// Gets or sets the color value of the key.
	/// </summary>
	public Color Color { get; set; } = color;
}