namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blur Bloom</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlurBloom : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.BlurBloom;
	/// <summary>
	/// Gets or sets the value of the <b>Amount</b>.
	/// </summary>
	[JsonAlias("Amount")]
	public float Amount { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Glow</b>.
	/// </summary>
	[JsonAlias("Glow")]
	public float Glow { get; set; }
}