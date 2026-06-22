namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>NightVisionFX</b>.
/// </summary>
[JsonObjectSerializable]
public struct NightVisionFX : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.NightVisionFX;
	/// <summary>
	/// Gets or sets the value of the <b>Greenness</b>.
	/// </summary>
	[JsonAlias("Greenness")]
	public float Greenness { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Vignette</b>.
	/// </summary>
	[JsonAlias("Vignette")]
	public float Vignette { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Vignette_Alpha</b>.
	/// </summary>
	[JsonAlias("Vignette_Alpha")]
	public float VignetteAlpha { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Distortion</b>.
	/// </summary>
	[JsonAlias("Distortion")]
	public float Distortion { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Noise</b>.
	/// </summary>
	[JsonAlias("Noise")]
	public float Noise { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Intensity</b>.
	/// </summary>
	[JsonAlias("Intensity")]
	public float Intensity { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Light</b>.
	/// </summary>
	[JsonAlias("Light")]
	public float Light { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Light2</b>.
	/// </summary>
	[JsonAlias("Light2")]
	public float Light2 { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Line</b>.
	/// </summary>
	[JsonAlias("Line")]
	public float Line { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_Binocular_Size</b>.
	/// </summary>
	[JsonAlias("_Binocular_Size")]
	public float BinocularSize { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_Binocular_Smooth</b>.
	/// </summary>
	[JsonAlias("_Binocular_Smooth")]
	public float BinocularSmooth { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_Binocular_Dist</b>.
	/// </summary>
	[JsonAlias("_Binocular_Dist")]
	public float BinocularDist { get; set; }
}