namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV Chromatical2</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvChromaticalTo : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.TvChromaticalTo;
	/// <summary>
	/// Gets or sets the value of the <b>Aberration</b>.
	/// </summary>
	[JsonAlias("Aberration")]
	public float Aberration { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Fade</b>.
	/// </summary>
	[JsonAlias("Fade")]
	public float Fade { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>ZoomFade</b>.
	/// </summary>
	[JsonAlias("ZoomFade")]
	public float ZoomFade { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>ZoomSpeed</b>.
	/// </summary>
	[JsonAlias("ZoomSpeed")]
	public float ZoomSpeed { get; set; }
}