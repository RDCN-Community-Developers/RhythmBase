namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Sharpen Sharpen</b>.
/// </summary>
[JsonObjectSerializable]
public struct SharpenSharpen : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.SharpenSharpen;
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
}