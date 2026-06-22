namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Drawing Manga</b>.
/// </summary>
[JsonObjectSerializable]
public struct DrawingManga : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.DrawingManga;
	/// <summary>
	/// Gets or sets the value of the <b>DotSize</b>.
	/// </summary>
	[JsonAlias("DotSize")]
	public float DotSize { get; set; }
}