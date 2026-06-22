namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Distortion Noise</b>.
/// </summary>
[JsonObjectSerializable]
public struct DistortionNoise : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.DistortionNoise;
	/// <summary>
	/// Gets or sets the value of the <b>Distortion</b>.
	/// </summary>
	[JsonAlias("Distortion")]
	public float Distortion { get; set; }
}