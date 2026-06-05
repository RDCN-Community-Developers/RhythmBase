namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Drawing EnhancedComics</b>.
/// </summary>
[JsonObjectSerializable]
public struct DrawingEnhancedComics : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.DrawingEnhancedComics;
	/// <summary>
	/// Gets or sets the value of the <b>DotSize</b>.
	/// </summary>
	[JsonAlias("DotSize")]
	public float DotSize { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_ColorR</b>.
	/// </summary>
	[JsonAlias("_ColorR")]
	public float ColorR { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_ColorG</b>.
	/// </summary>
	[JsonAlias("_ColorG")]
	public float ColorG { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_ColorB</b>.
	/// </summary>
	[JsonAlias("_ColorB")]
	public float ColorB { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_Blood</b>.
	/// </summary>
	[JsonAlias("_Blood")]
	public float Blood { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_SmoothStart</b>.
	/// </summary>
	[JsonAlias("_SmoothStart")]
	public float SmoothStart { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_SmoothEnd</b>.
	/// </summary>
	[JsonAlias("_SmoothEnd")]
	public float SmoothEnd { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>ColorRGB</b>.
	/// </summary>
	[JsonAlias("ColorRGB")]
	public Color ColorRGB { get; set; }
}