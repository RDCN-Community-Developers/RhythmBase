namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Colors NewPosterize</b>.
/// </summary>
[JsonObjectSerializable]
public struct ColorsNewPosterize : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.ColorsNewPosterize;
	/// <summary>
	/// Gets or sets the value of the <b>Gamma</b>.
	/// </summary>
	[JsonAlias("Gamma")]
	public float Gamma { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Colors</b>.
	/// </summary>
	[JsonAlias("Colors")]
	public float Colors { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Green_Mod</b>.
	/// </summary>
	[JsonAlias("Green_Mod")]
	public float GreenMod { get; set; }
}