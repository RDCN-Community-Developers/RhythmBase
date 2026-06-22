namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blur Movie</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlurMovie : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.BlurMovie;
	/// <summary>
	/// Gets or sets the value of the <b>Radius</b>.
	/// </summary>
	[JsonAlias("Radius")]
	public float Radius { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Factor</b>.
	/// </summary>
	[JsonAlias("Factor")]
	public float Factor { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>FastFilter</b>.
	/// </summary>
	[JsonAlias("FastFilter")]
	public int FastFilter { get; set; }
}