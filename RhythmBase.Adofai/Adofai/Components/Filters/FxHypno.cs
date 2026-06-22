namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>FX Hypno</b>.
/// </summary>
[JsonObjectSerializable]
public struct FxHypno : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.FxHypno;
	/// <summary>
	/// Gets or sets the value of the <b>Speed</b>.
	/// </summary>
	[JsonAlias("Speed")]
	public float Speed { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Green</b>.
	/// </summary>
	[JsonAlias("Green")]
	public float Green { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Blue</b>.
	/// </summary>
	[JsonAlias("Blue")]
	public float Blue { get; set; }
}