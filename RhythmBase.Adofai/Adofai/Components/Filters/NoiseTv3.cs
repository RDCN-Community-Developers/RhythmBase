namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Noise TV 3</b>.
/// </summary>
[JsonObjectSerializable]
public struct NoiseTv3 : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.NoiseTv3;
	/// <summary>
	/// Gets or sets the value of the <b>Fade</b>.
	/// </summary>
	[JsonAlias("Fade")]
	public float Fade { get; set; }
}