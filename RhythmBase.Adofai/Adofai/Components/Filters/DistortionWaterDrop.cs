namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Distortion Water Drop</b>.
/// </summary>
[JsonObjectSerializable]
public struct DistortionWaterDrop : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.DistortionWaterDrop;
	/// <summary>
	/// Gets or sets the value of the <b>WaveIntensity</b>.
	/// </summary>
	[JsonAlias("WaveIntensity")]
	public float WaveIntensity { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>NumberOfWaves</b>.
	/// </summary>
	[JsonAlias("NumberOfWaves")]
	public int NumberOfWaves { get; set; }
}