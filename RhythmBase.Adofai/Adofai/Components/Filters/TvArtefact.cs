namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV Artefact</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvArtefact : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.TvArtefact;
	/// <summary>
	/// Gets or sets the value of the <b>Fade</b>.
	/// </summary>
	[JsonAlias("Fade")]
	public float Fade { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Colorisation</b>.
	/// </summary>
	[JsonAlias("Colorisation")]
	public float Colorisation { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Parasite</b>.
	/// </summary>
	[JsonAlias("Parasite")]
	public float Parasite { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Noise</b>.
	/// </summary>
	[JsonAlias("Noise")]
	public float Noise { get; set; }
}