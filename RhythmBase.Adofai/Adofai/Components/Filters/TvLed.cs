namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV LED</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvLed : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.TvLed;
	/// <summary>
	/// Gets or sets the value of the <b>Size</b>.
	/// </summary>
	[JsonAlias("Size")]
	public int Size { get; set; }
}