namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV Tiles</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvTiles : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.TvTiles;
	/// <summary>
	/// Gets or sets the value of the <b>Size</b>.
	/// </summary>
	[JsonAlias("Size")]
	public float Size { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Intensity</b>.
	/// </summary>
	[JsonAlias("Intensity")]
	public float Intensity { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>StretchX</b>.
	/// </summary>
	[JsonAlias("StretchX")]
	public float StretchX { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>StretchY</b>.
	/// </summary>
	[JsonAlias("StretchY")]
	public float StretchY { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Fade</b>.
	/// </summary>
	[JsonAlias("Fade")]
	public float Fade { get; set; }
}