namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Drawing Halftone</b>.
/// </summary>
[JsonObjectSerializable]
public struct DrawingHalftone : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.DrawingHalftone;
	/// <summary>
	/// Gets or sets the value of the <b>Threshold</b>.
	/// </summary>
	[JsonAlias("Threshold")]
	public float Threshold { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>DotSize</b>.
	/// </summary>
	[JsonAlias("DotSize")]
	public float DotSize { get; set; }
}