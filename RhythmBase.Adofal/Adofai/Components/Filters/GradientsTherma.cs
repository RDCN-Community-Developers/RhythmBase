namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Gradients Therma</b>.
/// </summary>
[JsonObjectSerializable]
public struct GradientsTherma : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.GradientsTherma;
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