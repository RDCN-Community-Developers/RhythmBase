namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV Distorted</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvDistorted : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.TvDistorted;
	/// <summary>
	/// Gets or sets the value of the <b>Distortion</b>.
	/// </summary>
	[JsonAlias("Distortion")]
	public float Distortion { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>RGB</b>.
	/// </summary>
	[JsonAlias("RGB")]
	public float Rgb { get; set; }
}