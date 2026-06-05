namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Drawing CellShading</b>.
/// </summary>
[JsonObjectSerializable]
public struct DrawingCellShading : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.DrawingCellShading;
	/// <summary>
	/// Gets or sets the value of the <b>EdgeSize</b>.
	/// </summary>
	[JsonAlias("EdgeSize")]
	public float EdgeSize { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>ColorLevel</b>.
	/// </summary>
	[JsonAlias("ColorLevel")]
	public float ColorLevel { get; set; }
}