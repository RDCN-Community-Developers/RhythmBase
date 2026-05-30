namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Distortion Dream</b>.
/// </summary>
[JsonObjectSerializable]
public struct DistortionDream : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.DistortionDream;
	/// <summary>
	/// Gets or sets the value of the <b>Distortion</b>.
	/// </summary>
	[JsonAlias("Distortion")]
	public float Distortion { get; set; }
}