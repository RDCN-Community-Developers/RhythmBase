namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Distortion Heat</b>.
/// </summary>
[JsonObjectSerializable]
public struct DistortionHeat : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.DistortionHeat;
	/// <summary>
	/// Gets or sets the value of the <b>Distortion</b>.
	/// </summary>
	[JsonAlias("Distortion")]
	public float Distortion { get; set; }
}