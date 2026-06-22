namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV Noise</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvNoise : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.TvNoise;
	/// <summary>
	/// Gets or sets the value of the <b>Fade</b>.
	/// </summary>
	[JsonAlias("Fade")]
	public float Fade { get; set; }
}