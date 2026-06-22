namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV Horror</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvHorror : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.TvHorror;
	/// <summary>
	/// Gets or sets the value of the <b>Fade</b>.
	/// </summary>
	[JsonAlias("Fade")]
	public float Fade { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Distortion</b>.
	/// </summary>
	[JsonAlias("Distortion")]
	public float Distortion { get; set; }
}