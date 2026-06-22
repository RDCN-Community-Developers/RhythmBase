namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>FX Hexagon Black</b>.
/// </summary>
[JsonObjectSerializable]
public struct FxHexagonBlack : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.FxHexagonBlack;
	/// <summary>
	/// Gets or sets the value of the <b>Value</b>.
	/// </summary>
	[JsonAlias("Value")]
	public float Value { get; set; }
}