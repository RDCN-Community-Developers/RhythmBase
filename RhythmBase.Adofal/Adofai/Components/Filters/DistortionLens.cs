namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Distortion Lens</b>.
/// </summary>
[JsonObjectSerializable]
public struct DistortionLens : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.DistortionLens;
	/// <summary>
	/// Gets or sets the value of the <b>Distortion</b>.
	/// </summary>
	[JsonAlias("Distortion")]
	public float Distortion { get; set; }
}