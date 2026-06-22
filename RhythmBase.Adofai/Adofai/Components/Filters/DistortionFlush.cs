namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Distortion Flush</b>.
/// </summary>
[JsonObjectSerializable]
public struct DistortionFlush : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.DistortionFlush;
	/// <summary>
	/// Gets or sets the value of the <b>Value</b>.
	/// </summary>
	[JsonAlias("Value")]
	public float Value { get; set; }
}