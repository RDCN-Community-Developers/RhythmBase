namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV WideScreenHV</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvWideScreenHV : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.TvWideScreenHV;
	/// <summary>
	/// Gets or sets the value of the <b>Size</b>.
	/// </summary>
	[JsonAlias("Size")]
	public float Size { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Smooth</b>.
	/// </summary>
	[JsonAlias("Smooth")]
	public float Smooth { get; set; }
}