namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Color Sepia</b>.
/// </summary>
[JsonObjectSerializable]
public struct ColorSepia : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.ColorSepia;
	/// <summary>
	/// Gets or sets the value of the <b>_Fade</b>.
	/// </summary>
	[JsonAlias("_Fade")]
	public float Fade { get; set; }
}