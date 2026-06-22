namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>OldFilm Cutting2</b>.
/// </summary>
[JsonObjectSerializable]
public struct OldFilmCutting2 : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.OldFilmCutting2;
	/// <summary>
	/// Gets or sets the value of the <b>Speed</b>.
	/// </summary>
	[JsonAlias("Speed")]
	public float Speed { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Luminosity</b>.
	/// </summary>
	[JsonAlias("Luminosity")]
	public float Luminosity { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Vignette</b>.
	/// </summary>
	[JsonAlias("Vignette")]
	public float Vignette { get; set; }
}