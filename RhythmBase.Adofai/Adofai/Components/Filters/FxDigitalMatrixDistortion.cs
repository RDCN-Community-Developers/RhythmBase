namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>FX DigitalMatrixDistortion</b>.
/// </summary>
[JsonObjectSerializable]
public struct FxDigitalMatrixDistortion : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.FxDigitalMatrixDistortion;
	/// <summary>
	/// Gets or sets the value of the <b>Size</b>.
	/// </summary>
	[JsonAlias("Size")]
	public float Size { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Speed</b>.
	/// </summary>
	[JsonAlias("Speed")]
	public float Speed { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Distortion</b>.
	/// </summary>
	[JsonAlias("Distortion")]
	public float Distortion { get; set; }
}