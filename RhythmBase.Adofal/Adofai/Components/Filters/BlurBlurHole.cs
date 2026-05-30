namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blur BlurHole</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlurBlurHole : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.BlurBlurHole;
	/// <summary>
	/// Gets or sets the value of the <b>Size</b>.
	/// </summary>
	[JsonAlias("Size")]
	public float Size { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_Radius</b>.
	/// </summary>
	[JsonAlias("_Radius")]
	public float Radius { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_SpotSize</b>.
	/// </summary>
	[JsonAlias("_SpotSize")]
	public float SpotSize { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_CenterX</b>.
	/// </summary>
	[JsonAlias("_CenterX")]
	public float CenterX { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_CenterY</b>.
	/// </summary>
	[JsonAlias("_CenterY")]
	public float CenterY { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_AlphaBlur</b>.
	/// </summary>
	[JsonAlias("_AlphaBlur")]
	public float AlphaBlur { get; set; }
}