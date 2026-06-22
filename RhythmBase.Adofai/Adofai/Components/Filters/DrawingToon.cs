namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Drawing Toon</b>.
/// </summary>
[JsonObjectSerializable]
public struct DrawingToon : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.DrawingToon;
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