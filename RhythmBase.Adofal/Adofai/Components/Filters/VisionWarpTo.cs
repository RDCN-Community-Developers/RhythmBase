namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Vision Warp2</b>.
/// </summary>
[JsonObjectSerializable]
public struct VisionWarpTo : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.VisionWarpTo;
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