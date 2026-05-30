namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Colors Threshold</b>.
/// </summary>
[JsonObjectSerializable]
public struct ColorsThreshold : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.ColorsThreshold;
	/// <summary>
	/// Gets or sets the value of the <b>Threshold</b>.
	/// </summary>
	[JsonAlias("Threshold")]
	public float Threshold { get; set; }
}