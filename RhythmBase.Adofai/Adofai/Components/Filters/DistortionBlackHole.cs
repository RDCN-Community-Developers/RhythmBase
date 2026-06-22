namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Distortion BlackHole</b>.
/// </summary>
[JsonObjectSerializable]
public struct DistortionBlackHole : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.DistortionBlackHole;
	/// <summary>
	/// Gets or sets the value of the <b>Size</b>.
	/// </summary>
	[JsonAlias("Size")]
	public float Size { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Distortion</b>.
	/// </summary>
	[JsonAlias("Distortion")]
	public float Distortion { get; set; }
}