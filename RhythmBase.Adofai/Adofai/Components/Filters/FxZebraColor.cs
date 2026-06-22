namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>FX ZebraColor</b>.
/// </summary>
[JsonObjectSerializable]
public struct FxZebraColor : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.FxZebraColor;
	/// <summary>
	/// Gets or sets the value of the <b>Value</b>.
	/// </summary>
	[JsonAlias("Value")]
	public float Value { get; set; }
}