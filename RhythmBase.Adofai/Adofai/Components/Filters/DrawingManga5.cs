namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Drawing Manga5</b>.
/// </summary>
[JsonObjectSerializable]
public struct DrawingManga5 : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.DrawingManga5;
	/// <summary>
	/// Gets or sets the value of the <b>DotSize</b>.
	/// </summary>
	[JsonAlias("DotSize")]
	public float DotSize { get; set; }
}