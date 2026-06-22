namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Drawing Comics</b>.
/// </summary>
[JsonObjectSerializable]
public struct DrawingComics : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.DrawingComics;
	/// <summary>
	/// Gets or sets the value of the <b>DotSize</b>.
	/// </summary>
	[JsonAlias("DotSize")]
	public float DotSize { get; set; }
}