namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Distortion BigFace</b>.
/// </summary>
[JsonObjectSerializable]
public struct DistortionBigFace : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.DistortionBigFace;
	/// <summary>
	/// Gets or sets the value of the <b>_Size</b>.
	/// </summary>
	[JsonAlias("_Size")]
	public float Size { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Distortion</b>.
	/// </summary>
	[JsonAlias("Distortion")]
	public float Distortion { get; set; }
}