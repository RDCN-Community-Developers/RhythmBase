namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Colors BleachBypass</b>.
/// </summary>
[JsonObjectSerializable]
public struct ColorsBleachBypass : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.ColorsBleachBypass;
	/// <summary>
	/// Gets or sets the value of the <b>Value</b>.
	/// </summary>
	[JsonAlias("Value")]
	public float Value { get; set; }
}