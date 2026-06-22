namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blur Steam</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlurSteam : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.BlurSteam;
	/// <summary>
	/// Gets or sets the value of the <b>Radius</b>.
	/// </summary>
	[JsonAlias("Radius")]
	public float Radius { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Quality</b>.
	/// </summary>
	[JsonAlias("Quality")]
	public float Quality { get; set; }
}