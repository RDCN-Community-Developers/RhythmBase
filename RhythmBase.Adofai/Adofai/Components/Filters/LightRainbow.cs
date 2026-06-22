namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Light Rainbow</b>.
/// </summary>
[JsonObjectSerializable]
public struct LightRainbow : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.LightRainbow;
	/// <summary>
	/// Gets or sets the value of the <b>Value</b>.
	/// </summary>
	[JsonAlias("Value")]
	public float Value { get; set; }
}