namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV PlanetMars</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvPlanetMars : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.TvPlanetMars;
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