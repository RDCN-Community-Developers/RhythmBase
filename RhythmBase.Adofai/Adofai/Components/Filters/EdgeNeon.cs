namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Edge Neon</b>.
/// </summary>
[JsonObjectSerializable]
public struct EdgeNeon : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.EdgeNeon;
	/// <summary>
	/// Gets or sets the value of the <b>EdgeWeight</b>.
	/// </summary>
	[JsonAlias("EdgeWeight")]
	public float EdgeWeight { get; set; }
}