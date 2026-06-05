namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Drawing Lines</b>.
/// </summary>
[JsonObjectSerializable]
public struct DrawingLines : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.DrawingLines;
	/// <summary>
	/// Gets or sets the value of the <b>Number</b>.
	/// </summary>
	[JsonAlias("Number")]
	public float Number { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Random</b>.
	/// </summary>
	[JsonAlias("Random")]
	public float Random { get; set; }
}