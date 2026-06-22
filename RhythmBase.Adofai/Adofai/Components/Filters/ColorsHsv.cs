namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Colors HSV</b>.
/// </summary>
[JsonObjectSerializable]
public struct ColorsHsv : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.ColorsHsv;
	/// <summary>
	/// Gets or sets the value of the <b>_HueShift</b>.
	/// </summary>
	[JsonAlias("_HueShift")]
	public float HueShift { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_Saturation</b>.
	/// </summary>
	[JsonAlias("_Saturation")]
	public float Saturation { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_ValueBrightness</b>.
	/// </summary>
	[JsonAlias("_ValueBrightness")]
	public float ValueBrightness { get; set; }
}