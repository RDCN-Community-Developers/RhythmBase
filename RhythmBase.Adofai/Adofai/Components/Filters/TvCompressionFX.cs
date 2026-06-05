namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV CompressionFX</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvCompressionFX : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.TvCompressionFX;
	/// <summary>
	/// Gets or sets the value of the <b>Parasite</b>.
	/// </summary>
	[JsonAlias("Parasite")]
	public float Parasite { get; set; }
}