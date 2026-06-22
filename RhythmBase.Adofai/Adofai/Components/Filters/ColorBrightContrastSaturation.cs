namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Color BrightContrastSaturation</b>.
/// </summary>
[JsonObjectSerializable]
public struct ColorBrightContrastSaturation : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.ColorBrightContrastSaturation;
	/// <summary>
	/// Gets or sets the value of the <b>Brightness</b>.
	/// </summary>
	[JsonAlias("Brightness")]
	public float Brightness { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Saturation</b>.
	/// </summary>
	[JsonAlias("Saturation")]
	public float Saturation { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Contrast</b>.
	/// </summary>
	[JsonAlias("Contrast")]
	public float Contrast { get; set; }
}