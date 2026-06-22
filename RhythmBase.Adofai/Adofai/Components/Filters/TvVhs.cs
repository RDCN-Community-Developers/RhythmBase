namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV VHS</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvVhs : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.TvVhs;
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
	/// Gets or sets the value of the <b>WhiteParasite</b>.
	/// </summary>
	[JsonAlias("WhiteParasite")]
	public float WhiteParasite { get; set; }
}