namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>FX Drunk</b>.
/// </summary>
[JsonObjectSerializable]
public struct FxDrunk : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.FxDrunk;
	/// <summary>
	/// Gets or sets the value of the <b>Value</b>.
	/// </summary>
	[JsonAlias("Value")]
	public float Value { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Speed</b>.
	/// </summary>
	[JsonAlias("Speed")]
	public float Speed { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Wavy</b>.
	/// </summary>
	[JsonAlias("Wavy")]
	public float Wavy { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Fade</b>.
	/// </summary>
	[JsonAlias("Fade")]
	public float Fade { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>ColoredSaturate</b>.
	/// </summary>
	[JsonAlias("ColoredSaturate")]
	public float ColoredSaturate { get; set; }
}