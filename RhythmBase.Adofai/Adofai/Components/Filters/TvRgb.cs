namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV Rgb</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvRgb : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.TvRgb;
	/// <summary>
	/// Gets or sets the value of the <b>Distortion</b>.
	/// </summary>
	[JsonAlias("Distortion")]
	public float Distortion { get; set; }
}