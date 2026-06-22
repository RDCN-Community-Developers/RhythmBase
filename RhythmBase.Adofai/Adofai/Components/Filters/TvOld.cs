namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV Old</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvOld : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.TvOld;
	/// <summary>
	/// Gets or sets the value of the <b>Distortion</b>.
	/// </summary>
	[JsonAlias("Distortion")]
	public float Distortion { get; set; }
}