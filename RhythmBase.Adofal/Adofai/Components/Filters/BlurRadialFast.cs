namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blur Radial Fast</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlurRadialFast : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.BlurRadialFast;
	/// <summary>
	/// Gets or sets the value of the <b>Intensity</b>.
	/// </summary>
	[JsonAlias("Intensity")]
	public float Intensity { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>MovX</b>.
	/// </summary>
	[JsonAlias("MovX")]
	public float MovX { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>MovY</b>.
	/// </summary>
	[JsonAlias("MovY")]
	public float MovY { get; set; }
}