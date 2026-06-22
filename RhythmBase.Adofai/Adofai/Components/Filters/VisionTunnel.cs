namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Vision Tunnel</b>.
/// </summary>
[JsonObjectSerializable]
public struct VisionTunnel : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.VisionTunnel;
	/// <summary>
	/// Gets or sets the value of the <b>Value</b>.
	/// </summary>
	[JsonAlias("Value")]
	public float Value { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Value2</b>.
	/// </summary>
	[JsonAlias("Value2")]
	public float Value2 { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Intensity</b>.
	/// </summary>
	[JsonAlias("Intensity")]
	public float Intensity { get; set; }
}