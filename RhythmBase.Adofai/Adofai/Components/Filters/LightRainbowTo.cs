namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Light Rainbow2</b>.
/// </summary>
[JsonObjectSerializable]
public struct LightRainbowTo : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.LightRainbowTo;
	/// <summary>
	/// Gets or sets the value of the <b>Value</b>.
	/// </summary>
	[JsonAlias("Value")]
	public float Value { get; set; }
}