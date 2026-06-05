namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Film ColorPerfection</b>.
/// </summary>
[JsonObjectSerializable]
public struct FilmColorPerfection : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.FilmColorPerfection;
	/// <summary>
	/// Gets or sets the value of the <b>Gamma</b>.
	/// </summary>
	[JsonAlias("Gamma")]
	public float Gamma { get; set; }
}