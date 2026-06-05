namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>VHS Tracking</b>.
/// </summary>
[JsonObjectSerializable]
public struct VhsTracking : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.VhsTracking;
	/// <summary>
	/// Gets or sets the value of the <b>Tracking</b>.
	/// </summary>
	[JsonAlias("Tracking")]
	public float Tracking { get; set; }
}