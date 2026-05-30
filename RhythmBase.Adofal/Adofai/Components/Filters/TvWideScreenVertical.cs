namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV WideScreenVertical</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvWideScreenVertical : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.TvWideScreenVertical;
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