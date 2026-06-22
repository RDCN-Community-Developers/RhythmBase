namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>FX Ascii</b>.
/// </summary>
[JsonObjectSerializable]
public struct FxAscii : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.FxAscii;
	/// <summary>
	/// Gets or sets the value of the <b>Value</b>.
	/// </summary>
	[JsonAlias("Value")]
	public float Value { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Fade</b>.
	/// </summary>
	[JsonAlias("Fade")]
	public float Fade { get; set; }
}