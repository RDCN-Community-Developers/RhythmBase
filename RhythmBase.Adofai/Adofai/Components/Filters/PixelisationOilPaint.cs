namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Pixelisation OilPaint</b>.
/// </summary>
[JsonObjectSerializable]
public struct PixelisationOilPaint : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.PixelisationOilPaint;
	/// <summary>
	/// Gets or sets the value of the <b>Value</b>.
	/// </summary>
	[JsonAlias("Value")]
	public float Value { get; set; }
}