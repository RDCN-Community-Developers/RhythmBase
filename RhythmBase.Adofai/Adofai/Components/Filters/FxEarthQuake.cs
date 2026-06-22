namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>FX EarthQuake</b>.
/// </summary>
[JsonObjectSerializable]
public struct FxEarthQuake : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.FxEarthQuake;
	/// <summary>
	/// Gets or sets the value of the <b>Speed</b>.
	/// </summary>
	[JsonAlias("Speed")]
	public float Speed { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>X</b>.
	/// </summary>
	[JsonAlias("X")]
	public float X { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Y</b>.
	/// </summary>
	[JsonAlias("Y")]
	public float Y { get; set; }
}