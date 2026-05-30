namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Distortion Dissipation</b>.
/// </summary>
[JsonObjectSerializable]
public struct DistortionDissipation : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.DistortionDissipation;
	/// <summary>
	/// Gets or sets the value of the <b>Dissipation</b>.
	/// </summary>
	[JsonAlias("Dissipation")]
	public float Dissipation { get; set; }
}