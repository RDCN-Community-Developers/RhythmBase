namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Light Water2</b>.
/// </summary>
[JsonObjectSerializable]
public struct LightWaterTo : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.LightWaterTo;
	/// <summary>
	/// Gets or sets the value of the <b>Speed</b>.
	/// </summary>
	[JsonAlias("Speed")]
	public float Speed { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Speed_X</b>.
	/// </summary>
	[JsonAlias("Speed_X")]
	public float SpeedX { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Speed_Y</b>.
	/// </summary>
	[JsonAlias("Speed_Y")]
	public float SpeedY { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Intensity</b>.
	/// </summary>
	[JsonAlias("Intensity")]
	public float Intensity { get; set; }
}