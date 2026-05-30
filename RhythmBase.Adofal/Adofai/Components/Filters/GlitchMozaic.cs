namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Glitch Mozaic</b>.
/// </summary>
[JsonObjectSerializable]
public struct GlitchMozaic : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.GlitchMozaic;
	/// <summary>
	/// Gets or sets the value of the <b>Intensity</b>.
	/// </summary>
	[JsonAlias("Intensity")]
	public float Intensity { get; set; }
}