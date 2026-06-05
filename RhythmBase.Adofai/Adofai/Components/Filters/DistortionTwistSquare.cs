namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Distortion Twist Square</b>.
/// </summary>
[JsonObjectSerializable]
public struct DistortionTwistSquare : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.DistortionTwistSquare;
	/// <summary>
	/// Gets or sets the value of the <b>CenterX</b>.
	/// </summary>
	[JsonAlias("CenterX")]
	public float CenterX { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>CenterY</b>.
	/// </summary>
	[JsonAlias("CenterY")]
	public float CenterY { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Distortion</b>.
	/// </summary>
	[JsonAlias("Distortion")]
	public float Distortion { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Size</b>.
	/// </summary>
	[JsonAlias("Size")]
	public float Size { get; set; }
}