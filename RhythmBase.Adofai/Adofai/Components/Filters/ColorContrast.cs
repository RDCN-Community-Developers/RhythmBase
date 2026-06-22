namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Color Contrast</b>.
/// </summary>
[JsonObjectSerializable]
public struct ColorContrast : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.ColorContrast;
	/// <summary>
	/// Gets or sets the value of the <b>Contrast</b>.
	/// </summary>
	[JsonAlias("Contrast")]
	public float Contrast { get; set; }
}