namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV ARCADE</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvArcade : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.TvArcade;
	/// <summary>
	/// Gets or sets the value of the <b>Fade</b>.
	/// </summary>
	[JsonAlias("Fade")]
	public float Fade { get; set; }
}