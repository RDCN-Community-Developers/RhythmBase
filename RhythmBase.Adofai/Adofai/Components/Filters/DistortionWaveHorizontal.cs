namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Distortion Wave Horizontal</b>.
/// </summary>
[JsonObjectSerializable]
public struct DistortionWaveHorizontal : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.DistortionWaveHorizontal;
	/// <summary>
	/// Gets or sets the value of the <b>WaveIntensity</b>.
	/// </summary>
	[JsonAlias("WaveIntensity")]
	public float WaveIntensity { get; set; }
}