namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Drawing Manga Color</b>.
/// </summary>
[JsonObjectSerializable]
public struct DrawingMangaColor : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.DrawingMangaColor;
	/// <summary>
	/// Gets or sets the value of the <b>DotSize</b>.
	/// </summary>
	[JsonAlias("DotSize")]
	public float DotSize { get; set; }
}