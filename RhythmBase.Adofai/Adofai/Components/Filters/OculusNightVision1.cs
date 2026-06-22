namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Oculus NightVision1</b>.
/// </summary>
[JsonObjectSerializable]
public struct OculusNightVision1 : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.OculusNightVision1;
	/// <summary>
	/// Gets or sets the value of the <b>Vignette</b>.
	/// </summary>
	[JsonAlias("Vignette")]
	public float Vignette { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Linecount</b>.
	/// </summary>
	[JsonAlias("Linecount")]
	public float Linecount { get; set; }
}