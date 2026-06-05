namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Pixelisation OilPaintHQ</b>.
/// </summary>
[JsonObjectSerializable]
public struct PixelisationOilPaintHQ : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.PixelisationOilPaintHQ;
	/// <summary>
	/// Gets or sets the value of the <b>Value</b>.
	/// </summary>
	[JsonAlias("Value")]
	public float Value { get; set; }
}