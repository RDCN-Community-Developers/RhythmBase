namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>AAA WaterDrop</b>.
/// </summary>
[JsonObjectSerializable]
public struct AaaWaterDrop : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.AaaWaterDrop;
	/// <summary>
	/// Gets or sets the value of the <b>Distortion</b>.
	/// </summary>
	[JsonAlias("Distortion")]
	public float Distortion { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>SizeX</b>.
	/// </summary>
	[JsonAlias("SizeX")]
	public float SizeX { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>SizeY</b>.
	/// </summary>
	[JsonAlias("SizeY")]
	public float SizeY { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Speed</b>.
	/// </summary>
	[JsonAlias("Speed")]
	public float Speed { get; set; }
}