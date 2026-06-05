namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Noise TV</b>.
/// </summary>
[JsonObjectSerializable]
public struct NoiseTv : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.NoiseTv;
	/// <summary>
	/// Gets or sets the value of the <b>Fade</b>.
	/// </summary>
	[JsonAlias("Fade")]
	public float Fade { get; set; }
}