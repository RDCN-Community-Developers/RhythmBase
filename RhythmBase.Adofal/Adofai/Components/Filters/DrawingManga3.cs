namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Drawing Manga3</b>.
/// </summary>
[JsonObjectSerializable]
public struct DrawingManga3 : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.DrawingManga3;
	/// <summary>
	/// Gets or sets the value of the <b>DotSize</b>.
	/// </summary>
	[JsonAlias("DotSize")]
	public float DotSize { get; set; }
}