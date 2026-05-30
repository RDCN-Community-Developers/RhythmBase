namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV Vcr</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvVcr : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.TvVcr;
	/// <summary>
	/// Gets or sets the value of the <b>Distortion</b>.
	/// </summary>
	[JsonAlias("Distortion")]
	public float Distortion { get; set; }
}