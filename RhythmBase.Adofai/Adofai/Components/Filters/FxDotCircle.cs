namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>FX Dot Circle</b>.
/// </summary>
[JsonObjectSerializable]
public struct FxDotCircle : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.FxDotCircle;
	/// <summary>
	/// Gets or sets the value of the <b>Value</b>.
	/// </summary>
	[JsonAlias("Value")]
	public float Value { get; set; }
}