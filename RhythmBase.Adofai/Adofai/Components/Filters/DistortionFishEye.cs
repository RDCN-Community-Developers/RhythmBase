namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Distortion FishEye</b>.
/// </summary>
[JsonObjectSerializable]
public struct DistortionFishEye : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.DistortionFishEye;
	/// <summary>
	/// Gets or sets the value of the <b>Distortion</b>.
	/// </summary>
	[JsonAlias("Distortion")]
	public float Distortion { get; set; }
}