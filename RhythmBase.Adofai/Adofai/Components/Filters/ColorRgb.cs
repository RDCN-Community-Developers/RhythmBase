namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Color RGB</b>.
/// </summary>
[JsonObjectSerializable]
public struct ColorRgb : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.ColorRgb;
	/// <summary>
	/// Gets or sets the value of the <b>ColorRGB</b>.
	/// </summary>
	[JsonAlias("ColorRGB")]
	public Color ColorRGB { get; set; }
}