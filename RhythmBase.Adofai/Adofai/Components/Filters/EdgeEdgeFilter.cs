namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Edge Edge filter</b>.
/// </summary>
[JsonObjectSerializable]
public struct EdgeEdgeFilter : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.EdgeEdgeFilter;
	/// <summary>
	/// Gets or sets the value of the <b>GreenAmplifier</b>.
	/// </summary>
	[JsonAlias("GreenAmplifier")]
	public float GreenAmplifier { get; set; }
}