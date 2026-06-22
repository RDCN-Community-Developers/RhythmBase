namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV ARCADE 3</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvArcade3 : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.TvArcade3;
	/// <summary>
	/// Gets or sets the value of the <b>Interferance_Size</b>.
	/// </summary>
	[JsonAlias("Interferance_Size")]
	public float InterferanceSize { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Interferance_Speed</b>.
	/// </summary>
	[JsonAlias("Interferance_Speed")]
	public float InterferanceSpeed { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Contrast</b>.
	/// </summary>
	[JsonAlias("Contrast")]
	public float Contrast { get; set; }
}