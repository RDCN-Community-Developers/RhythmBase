namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV 80</b>.
/// </summary>
[JsonObjectSerializable]
public struct Tv80 : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.Tv80;
	/// <summary>
	/// Gets or sets the value of the <b>Fade</b>.
	/// </summary>
	[JsonAlias("Fade")]
	public float Fade { get; set; }
}