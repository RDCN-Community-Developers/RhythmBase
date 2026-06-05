namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV Old Movie 2</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvOldMovieTo : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.TvOldMovieTo;
	/// <summary>
	/// Gets or sets the value of the <b>FramePerSecond</b>.
	/// </summary>
	[JsonAlias("FramePerSecond")]
	public float FramePerSecond { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Contrast</b>.
	/// </summary>
	[JsonAlias("Contrast")]
	public float Contrast { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>SceneCut</b>.
	/// </summary>
	[JsonAlias("SceneCut")]
	public float SceneCut { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Fade</b>.
	/// </summary>
	[JsonAlias("Fade")]
	public float Fade { get; set; }
}