namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Color GrayScale</b>.
/// </summary>
[JsonObjectSerializable]
public struct ColorGrayScale : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.ColorGrayScale;
	/// <summary>
	/// Gets or sets the value of the <b>_Fade</b>.
	/// </summary>
	[JsonAlias("_Fade")]
	public float Fade { get; set; }
}