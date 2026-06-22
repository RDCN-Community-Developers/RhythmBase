namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Gradients Rainbow</b>.
/// </summary>
[JsonObjectSerializable]
public struct GradientsRainbow : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.GradientsRainbow;
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