namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Gradients Hue</b>.
/// </summary>
[JsonObjectSerializable]
public struct GradientsHue : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.GradientsHue;
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