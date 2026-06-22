namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Glow Glow</b>.
/// </summary>
[JsonObjectSerializable]
public struct GlowGlow : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.GlowGlow;
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
	/// <summary>
	/// Gets or sets the value of the <b>Threshold</b>.
	/// </summary>
	[JsonAlias("Threshold")]
	public float Threshold { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Intensity</b>.
	/// </summary>
	[JsonAlias("Intensity")]
	public float Intensity { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Precision</b>.
	/// </summary>
	[JsonAlias("Precision")]
	public float Precision { get; set; }
}