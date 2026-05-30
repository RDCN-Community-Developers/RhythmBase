namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV Old</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvOld : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.TvOld;
	/// <summary>
	/// Gets or sets the value of the <b>Distortion</b>.
	/// </summary>
	[JsonAlias("Distortion")]
	public float Distortion { get; set; }
}