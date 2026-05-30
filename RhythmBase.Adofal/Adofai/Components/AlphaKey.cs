namespace RhythmBase.Adofai.Components;

/// <summary>
/// Represents an alpha key in a gradient.
/// </summary>
public struct AlphaKey(float alpha, float time)
{
	/// <summary>
	/// Gets or sets the alpha value of the key.
	/// </summary>
	public float Alpha { get; set; } = alpha;

	/// <summary>
	/// Gets or sets the time value of the key.
	/// </summary>
	public float Time { get; set; } = time;
}
