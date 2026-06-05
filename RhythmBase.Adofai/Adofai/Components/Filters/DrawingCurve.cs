namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Drawing Curve</b>.
/// </summary>
[JsonObjectSerializable]
public struct DrawingCurve : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.DrawingCurve;
	/// <summary>
	/// Gets or sets the value of the <b>Size</b>.
	/// </summary>
	[JsonAlias("Size")]
	public float Size { get; set; }
}