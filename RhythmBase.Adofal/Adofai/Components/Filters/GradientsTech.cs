namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Gradients Tech</b>.
/// </summary>
[JsonObjectSerializable]
public struct GradientsTech : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.GradientsTech;
	/// <summary>
	/// Gets or sets the value of the <b>Switch</b>.
	/// </summary>
	[JsonAlias("Switch")]
	public float Switch { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Fade</b>.
	/// </summary>
	[JsonAlias("Fade")]
	public float Fade { get; set; }
}