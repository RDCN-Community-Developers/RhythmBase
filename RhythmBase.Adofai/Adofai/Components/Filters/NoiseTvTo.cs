namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Noise TV 2</b>.
/// </summary>
[JsonObjectSerializable]
public struct NoiseTvTo : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.NoiseTvTo;
	/// <summary>
	/// Gets or sets the value of the <b>Fade</b>.
	/// </summary>
	[JsonAlias("Fade")]
	public float Fade { get; set; }
}