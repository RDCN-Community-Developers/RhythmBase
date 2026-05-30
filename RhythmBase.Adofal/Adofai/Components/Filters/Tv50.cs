namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV 50</b>.
/// </summary>
[JsonObjectSerializable]
public struct Tv50 : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.Tv50;
	/// <summary>
	/// Gets or sets the value of the <b>Fade</b>.
	/// </summary>
	[JsonAlias("Fade")]
	public float Fade { get; set; }
}