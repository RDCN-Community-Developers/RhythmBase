namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Distortion ShockWave</b>.
/// </summary>
[JsonObjectSerializable]
public struct DistortionShockWave : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.DistortionShockWave;
	/// <summary>
	/// Gets or sets the value of the <b>PosX</b>.
	/// </summary>
	[JsonAlias("PosX")]
	public float PosX { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>PosY</b>.
	/// </summary>
	[JsonAlias("PosY")]
	public float PosY { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Speed</b>.
	/// </summary>
	[JsonAlias("Speed")]
	public float Speed { get; set; }
}