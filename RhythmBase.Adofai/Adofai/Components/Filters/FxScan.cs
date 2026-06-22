namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>FX Scan</b>.
/// </summary>
[JsonObjectSerializable]
public struct FxScan : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.FxScan;
	/// <summary>
	/// Gets or sets the value of the <b>Size</b>.
	/// </summary>
	[JsonAlias("Size")]
	public float Size { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Speed</b>.
	/// </summary>
	[JsonAlias("Speed")]
	public float Speed { get; set; }
}