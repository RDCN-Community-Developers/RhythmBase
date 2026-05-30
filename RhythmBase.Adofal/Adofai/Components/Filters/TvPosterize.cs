namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV Posterize</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvPosterize : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.TvPosterize;
	/// <summary>
	/// Gets or sets the value of the <b>Posterize</b>.
	/// </summary>
	[JsonAlias("Posterize")]
	public float Posterize { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Fade</b>.
	/// </summary>
	[JsonAlias("Fade")]
	public float Fade { get; set; }
}