namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Edge Sigmoid</b>.
/// </summary>
[JsonObjectSerializable]
public struct EdgeSigmoid : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.EdgeSigmoid;
	/// <summary>
	/// Gets or sets the value of the <b>Gain</b>.
	/// </summary>
	[JsonAlias("Gain")]
	public float Gain { get; set; }
}