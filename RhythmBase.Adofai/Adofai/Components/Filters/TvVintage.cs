namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV Vintage</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvVintage : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.TvVintage;
	/// <summary>
	/// Gets or sets the value of the <b>Distortion</b>.
	/// </summary>
	[JsonAlias("Distortion")]
	public float Distortion { get; set; }
}