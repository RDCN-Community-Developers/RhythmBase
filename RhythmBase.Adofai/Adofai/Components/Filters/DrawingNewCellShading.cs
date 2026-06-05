namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Drawing NewCellShading</b>.
/// </summary>
[JsonObjectSerializable]
public struct DrawingNewCellShading : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.DrawingNewCellShading;
	/// <summary>
	/// Gets or sets the value of the <b>Threshold</b>.
	/// </summary>
	[JsonAlias("Threshold")]
	public float Threshold { get; set; }
}