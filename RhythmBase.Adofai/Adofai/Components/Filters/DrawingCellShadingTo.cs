namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Drawing CellShading2</b>.
/// </summary>
[JsonObjectSerializable]
public struct DrawingCellShadingTo : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.DrawingCellShadingTo;
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
	/// <summary>
	/// Gets or sets the value of the <b>Blur</b>.
	/// </summary>
	[JsonAlias("Blur")]
	public float Blur { get; set; }
}