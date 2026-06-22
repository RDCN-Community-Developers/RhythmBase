namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV Old Movie</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvOldMovie : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.TvOldMovie;
	/// <summary>
	/// Gets or sets the value of the <b>Distortion</b>.
	/// </summary>
	[JsonAlias("Distortion")]
	public float Distortion { get; set; }
}