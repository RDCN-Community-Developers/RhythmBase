namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV WideScreenCircle</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvWideScreenCircle : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.TvWideScreenCircle;
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