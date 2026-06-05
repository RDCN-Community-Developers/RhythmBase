namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Film Grain</b>.
/// </summary>
[JsonObjectSerializable]
public struct FilmGrain : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.FilmGrain;
	/// <summary>
	/// Gets or sets the value of the <b>Value</b>.
	/// </summary>
	[JsonAlias("Value")]
	public float Value { get; set; }
}