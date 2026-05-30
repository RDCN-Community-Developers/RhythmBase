namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Distortion Flag</b>.
/// </summary>
[JsonObjectSerializable]
public struct DistortionFlag : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.DistortionFlag;
	/// <summary>
	/// Gets or sets the value of the <b>Distortion</b>.
	/// </summary>
	[JsonAlias("Distortion")]
	public float Distortion { get; set; }
}