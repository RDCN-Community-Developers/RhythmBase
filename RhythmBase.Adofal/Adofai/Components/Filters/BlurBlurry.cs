namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blur Blurry</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlurBlurry : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.BlurBlurry;
	/// <summary>
	/// Gets or sets the value of the <b>Amount</b>.
	/// </summary>
	[JsonAlias("Amount")]
	public float Amount { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>FastFilter</b>.
	/// </summary>
	[JsonAlias("FastFilter")]
	public int FastFilter { get; set; }
}