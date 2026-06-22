namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV VHS Rewind</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvVhsRewind : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.TvVhsRewind;
	/// <summary>
	/// Gets or sets the value of the <b>Cryptage</b>.
	/// </summary>
	[JsonAlias("Cryptage")]
	public float Cryptage { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Parasite</b>.
	/// </summary>
	[JsonAlias("Parasite")]
	public float Parasite { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Parasite2</b>.
	/// </summary>
	[JsonAlias("Parasite2")]
	public float Parasite2 { get; set; }
}